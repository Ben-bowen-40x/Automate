#!/usr/bin/env python3
"""
upload_to_gsheet.py

Upload a local data file (CSV / TSV / delimited text / Excel) into a named tab
of a Google Sheet.

Usage
-----
    python upload_to_gsheet.py \
        --data-file  C:\\exports\\roi_report.csv \
        --sheet-url  https://docs.google.com/spreadsheets/d/1AbC.../edit#gid=0 \
        --tab        "ROI Report" \
        --credentials C:\\secrets\\service_account.json

Requirements
------------
    pip install google-api-python-client google-auth
    pip install pandas openpyxl        # only needed for .xlsx / .xls sources

Credentials
-----------
Expects a Google Cloud *service account* JSON key file. The target spreadsheet
must be shared (Editor) with the service account's client_email, otherwise the
API returns 404/403 even though the URL is valid.

If --credentials is omitted, the GOOGLE_APPLICATION_CREDENTIALS environment
variable is used.

Exit codes
----------
    0  success
    1  handled failure (bad args, missing file, API error) -- message on stderr
    2  unexpected exception (traceback on stderr)
"""

from __future__ import annotations

import argparse
import csv
import datetime as _dt
import decimal
import os
import re
import sys
import traceback

SCOPES = ["https://www.googleapis.com/auth/spreadsheets"]

# Sheets API caps request payloads; keep each write well under the limit.
MAX_CELLS_PER_REQUEST = 50_000

DELIMITER_BY_EXT = {
    ".csv": ",",
    ".tsv": "\t",
    ".tab": "\t",
    ".txt": ",",
    ".psv": "|",
}
EXCEL_EXTS = {".xlsx", ".xlsm", ".xls"}


class UploadError(Exception):
    """A failure we can explain to the user without a traceback."""


# --------------------------------------------------------------------------
# Argument parsing
# --------------------------------------------------------------------------
def parse_args(argv=None) -> argparse.Namespace:
    p = argparse.ArgumentParser(
        description="Upload a local data file into a tab of a Google Sheet.",
        formatter_class=argparse.ArgumentDefaultsHelpFormatter,
    )
    p.add_argument("--data-file", required=True,
                   help="Path to the local CSV/TSV/delimited/Excel file.")
    p.add_argument("--sheet-url", required=True,
                   help="Full Google Sheets URL, or a bare spreadsheet ID.")
    p.add_argument("--tab", required=True,
                   help="Name of the destination tab (worksheet) in the sheet.")
    p.add_argument("--credentials", default=None,
                   help="Path to service account JSON key. Defaults to "
                        "$GOOGLE_APPLICATION_CREDENTIALS.")

    p.add_argument("--mode", choices=("replace", "append"), default="replace",
                   help="replace = clear the tab first; append = add below "
                        "existing rows.")
    p.add_argument("--start-cell", default="A1",
                   help="Top-left cell for replace mode.")
    p.add_argument("--value-input-option", choices=("RAW", "USER_ENTERED"),
                   default="USER_ENTERED",
                   help="RAW writes strings verbatim; USER_ENTERED lets Sheets "
                        "parse numbers, dates and formulas.")
    p.add_argument("--create-tab", action="store_true",
                   help="Create the tab if it does not already exist.")
    p.add_argument("--skip-header", action="store_true",
                   help="Drop the first row of the source file (useful with "
                        "--mode append).")
    p.add_argument("--encoding", default="utf-8-sig",
                   help="Text encoding of a delimited source file.")
    p.add_argument("--delimiter", default=None,
                   help="Field delimiter. Inferred from the file extension "
                        "when omitted.")
    p.add_argument("--source-worksheet", default=0,
                   help="For Excel sources: worksheet name or 0-based index "
                        "to read from.")
    p.add_argument("--dry-run", action="store_true",
                   help="Read and validate everything, but write nothing.")
    return p.parse_args(argv)


# --------------------------------------------------------------------------
# Reading the local file
# --------------------------------------------------------------------------
def read_rows(args) -> list[list]:
    path = args.data_file
    if not os.path.isfile(path):
        raise UploadError(f"Data file not found: {path}")
    if os.path.getsize(path) == 0:
        raise UploadError(f"Data file is empty: {path}")

    ext = os.path.splitext(path)[1].lower()

    if ext in EXCEL_EXTS:
        rows = _read_excel(path, args.source_worksheet)
    else:
        delimiter = args.delimiter or DELIMITER_BY_EXT.get(ext, ",")
        rows = _read_delimited(path, delimiter, args.encoding)

    if args.skip_header and rows:
        rows = rows[1:]
    if not rows:
        raise UploadError("Source file produced zero rows after parsing.")
    return rows


def _read_delimited(path: str, delimiter: str, encoding: str) -> list[list]:
    try:
        with open(path, "r", newline="", encoding=encoding) as fh:
            return [row for row in csv.reader(fh, delimiter=delimiter)]
    except UnicodeDecodeError as exc:
        raise UploadError(
            f"Could not decode {path} as {encoding}. Try --encoding cp1252 "
            f"or --encoding latin-1. ({exc})"
        ) from exc


def _read_excel(path: str, worksheet) -> list[list]:
    try:
        import pandas as pd
    except ImportError as exc:
        raise UploadError(
            "Reading Excel files requires pandas and openpyxl: "
            "pip install pandas openpyxl"
        ) from exc

    try:
        sheet = int(worksheet)
    except (TypeError, ValueError):
        sheet = worksheet

    frame = pd.read_excel(path, sheet_name=sheet, header=None, dtype=object)
    return frame.where(frame.notna(), None).values.tolist()


# --------------------------------------------------------------------------
# Value normalisation -- the API only accepts JSON scalars
# --------------------------------------------------------------------------
def normalize(rows: list[list]) -> tuple[list[list], int]:
    width = max(len(r) for r in rows)
    out = []
    for row in rows:
        clean = [_scalar(v) for v in row]
        clean.extend([""] * (width - len(clean)))   # pad ragged rows
        out.append(clean)
    return out, width


def _scalar(value):
    if value is None:
        return ""
    if isinstance(value, bool):
        return value
    if isinstance(value, (int, float, str)):
        # NaN is not valid JSON.
        if isinstance(value, float) and value != value:
            return ""
        return value
    if isinstance(value, decimal.Decimal):
        return float(value)
    if isinstance(value, (_dt.datetime, _dt.date, _dt.time)):
        return value.isoformat(sep=" ") if isinstance(value, _dt.datetime) \
            else value.isoformat()
    return str(value)


# --------------------------------------------------------------------------
# Google Sheets plumbing
# --------------------------------------------------------------------------
def extract_spreadsheet_id(url: str) -> str:
    match = re.search(r"/spreadsheets/d/([a-zA-Z0-9_-]+)", url)
    if match:
        return match.group(1)
    # Allow a bare ID to be passed instead of a full URL.
    if re.fullmatch(r"[a-zA-Z0-9_-]{20,}", url.strip()):
        return url.strip()
    raise UploadError(f"Could not extract a spreadsheet ID from: {url}")


def build_service(credentials_path: str | None):
    path = credentials_path or os.environ.get("GOOGLE_APPLICATION_CREDENTIALS")
    if not path:
        raise UploadError(
            "No credentials supplied. Pass --credentials <service_account.json> "
            "or set GOOGLE_APPLICATION_CREDENTIALS."
        )
    if not os.path.isfile(path):
        raise UploadError(f"Credentials file not found: {path}")

    try:
        from google.oauth2.service_account import Credentials
        from googleapiclient.discovery import build
    except ImportError as exc:
        raise UploadError(
            "Missing Google client libraries: "
            "pip install google-api-python-client google-auth"
        ) from exc

    try:
        creds = Credentials.from_service_account_file(path, scopes=SCOPES)
    except ValueError as exc:
        raise UploadError(
            f"{path} is not a valid service account key file. ({exc})"
        ) from exc

    return build("sheets", "v4", credentials=creds, cache_discovery=False)


def a1(tab: str, cell_range: str = "") -> str:
    """Quote a tab name for A1 notation, escaping embedded apostrophes."""
    quoted = "'" + tab.replace("'", "''") + "'"
    return f"{quoted}!{cell_range}" if cell_range else quoted


def col_letter(index: int) -> str:
    """1 -> A, 27 -> AA."""
    letters = ""
    while index > 0:
        index, rem = divmod(index - 1, 26)
        letters = chr(65 + rem) + letters
    return letters


def parse_start_cell(cell: str) -> tuple[int, int]:
    match = re.fullmatch(r"([A-Za-z]+)(\d+)", cell.strip())
    if not match:
        raise UploadError(f"--start-cell must look like 'A1', got: {cell}")
    col = 0
    for ch in match.group(1).upper():
        col = col * 26 + (ord(ch) - 64)
    return int(match.group(2)), col


def get_sheet_properties(service, spreadsheet_id: str, tab: str,
                         create: bool, rows: int, cols: int) -> dict:
    from googleapiclient.errors import HttpError

    try:
        meta = service.spreadsheets().get(
            spreadsheetId=spreadsheet_id,
            fields="sheets.properties(sheetId,title,gridProperties)",
        ).execute()
    except HttpError as exc:
        raise UploadError(_explain_http_error(exc, spreadsheet_id)) from exc

    for sheet in meta.get("sheets", []):
        if sheet["properties"]["title"] == tab:
            return sheet["properties"]

    existing = ", ".join(s["properties"]["title"] for s in meta.get("sheets", []))
    if not create:
        raise UploadError(
            f"Tab {tab!r} not found. Existing tabs: {existing}. "
            f"Pass --create-tab to create it."
        )

    body = {"requests": [{"addSheet": {"properties": {
        "title": tab,
        "gridProperties": {"rowCount": max(rows, 1000),
                           "columnCount": max(cols, 26)},
    }}}]}
    result = service.spreadsheets().batchUpdate(
        spreadsheetId=spreadsheet_id, body=body).execute()
    return result["replies"][0]["addSheet"]["properties"]


def ensure_grid_size(service, spreadsheet_id: str, props: dict,
                     needed_rows: int, needed_cols: int) -> None:
    """Grow the tab if the incoming data would overflow it. Never shrinks."""
    grid = props.get("gridProperties", {})
    current_rows = grid.get("rowCount", 0)
    current_cols = grid.get("columnCount", 0)
    if needed_rows <= current_rows and needed_cols <= current_cols:
        return

    service.spreadsheets().batchUpdate(
        spreadsheetId=spreadsheet_id,
        body={"requests": [{"updateSheetProperties": {
            "properties": {
                "sheetId": props["sheetId"],
                "gridProperties": {
                    "rowCount": max(current_rows, needed_rows),
                    "columnCount": max(current_cols, needed_cols),
                },
            },
            "fields": "gridProperties.rowCount,gridProperties.columnCount",
        }}]},
    ).execute()


def write_replace(service, spreadsheet_id, tab, rows, width,
                  start_row, start_col, value_input_option) -> int:
    service.spreadsheets().values().clear(
        spreadsheetId=spreadsheet_id, range=a1(tab), body={}).execute()

    chunk_rows = max(1, MAX_CELLS_PER_REQUEST // max(width, 1))
    end_col = col_letter(start_col + width - 1)
    written = 0

    for offset in range(0, len(rows), chunk_rows):
        chunk = rows[offset:offset + chunk_rows]
        top = start_row + offset
        bottom = top + len(chunk) - 1
        cell_range = f"{col_letter(start_col)}{top}:{end_col}{bottom}"
        service.spreadsheets().values().update(
            spreadsheetId=spreadsheet_id,
            range=a1(tab, cell_range),
            valueInputOption=value_input_option,
            body={"values": chunk},
        ).execute()
        written += len(chunk)
        _progress(written, len(rows))
    return written


def write_append(service, spreadsheet_id, tab, rows, width,
                 value_input_option) -> int:
    chunk_rows = max(1, MAX_CELLS_PER_REQUEST // max(width, 1))
    written = 0
    for offset in range(0, len(rows), chunk_rows):
        chunk = rows[offset:offset + chunk_rows]
        service.spreadsheets().values().append(
            spreadsheetId=spreadsheet_id,
            range=a1(tab),
            valueInputOption=value_input_option,
            insertDataOption="INSERT_ROWS",
            body={"values": chunk},
        ).execute()
        written += len(chunk)
        _progress(written, len(rows))
    return written


def _progress(done: int, total: int) -> None:
    print(f"  wrote {done:,} / {total:,} rows", flush=True)


def _explain_http_error(exc, spreadsheet_id: str) -> str:
    status = getattr(getattr(exc, "resp", None), "status", None)
    if status == 404:
        return (f"Spreadsheet {spreadsheet_id} not found. Check the URL, and "
                f"confirm the sheet is shared with the service account's "
                f"client_email.")
    if status == 403:
        return (f"Permission denied on {spreadsheet_id}. Share the sheet with "
                f"the service account's client_email as an Editor, and confirm "
                f"the Google Sheets API is enabled on the project.")
    return f"Google Sheets API error ({status}): {exc}"


# --------------------------------------------------------------------------
# Main
# --------------------------------------------------------------------------
def run(args) -> int:
    spreadsheet_id = extract_spreadsheet_id(args.sheet_url)
    start_row, start_col = parse_start_cell(args.start_cell)

    print(f"Reading {args.data_file} ...", flush=True)
    rows, width = normalize(read_rows(args))
    print(f"  {len(rows):,} rows x {width} columns", flush=True)

    needed_rows = len(rows) + (start_row - 1)
    needed_cols = width + (start_col - 1)

    if args.dry_run:
        preview = rows[0][:8] if rows else []
        print(f"DRY RUN -- nothing written.\n"
              f"  spreadsheet: {spreadsheet_id}\n"
              f"  tab:         {args.tab}\n"
              f"  mode:        {args.mode}\n"
              f"  first row:   {preview}")
        return 0

    from googleapiclient.errors import HttpError

    service = build_service(args.credentials)
    props = get_sheet_properties(service, spreadsheet_id, args.tab,
                                 args.create_tab, needed_rows, needed_cols)

    try:
        if args.mode == "replace":
            ensure_grid_size(service, spreadsheet_id, props,
                             needed_rows, needed_cols)
            written = write_replace(service, spreadsheet_id, args.tab, rows,
                                    width, start_row, start_col,
                                    args.value_input_option)
        else:
            written = write_append(service, spreadsheet_id, args.tab, rows,
                                   width, args.value_input_option)
    except HttpError as exc:
        raise UploadError(_explain_http_error(exc, spreadsheet_id)) from exc

    print(f"Done. {written:,} rows uploaded to '{args.tab}' "
          f"({args.mode} mode).")
    return 0


def main(argv=None) -> int:
    args = parse_args(argv)
    try:
        return run(args)
    except UploadError as exc:
        print(f"ERROR: {exc}", file=sys.stderr)
        return 1
    except KeyboardInterrupt:
        print("ERROR: interrupted.", file=sys.stderr)
        return 1
    except Exception:
        traceback.print_exc()
        return 2


if __name__ == "__main__":
    sys.exit(main())
