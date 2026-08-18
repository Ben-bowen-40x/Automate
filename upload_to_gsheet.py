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
    3  PARTIAL WRITE -- the destination tab is in an inconsistent state

Notes on failure semantics
--------------------------
2026-08-10: --replace-strategy controls the atomicity/fidelity tradeoff.
  in-place (default): clear the tab, then write chunks. Cheap, and preserves the
      tab's formatting, conditional formatting, protections and sheetId, but a
      mid-run failure leaves partial data. On such a failure the script stamps a
      loud marker in the top-left data cell and exits 3.
  swap: write to a hidden temp tab, then delete + rename in a single atomic
      batchUpdate. The destination is never partially written, but the tab is
      effectively recreated -- formatting, protections, conditional formats and
      the sheetId are lost, so anything bound to sheetId (charts, pivot tables)
      breaks. Name-based references ('Tab'!A1) survive.
"""

from __future__ import annotations

import argparse
import collections
import csv
import datetime as _dt
import decimal
import os
import random
import re
import socket
import ssl
import sys
import time
import traceback

SCOPES = ["https://www.googleapis.com/auth/spreadsheets"]

# Sheets API caps request payloads; keep each write well under the limit.
MAX_CELLS_PER_REQUEST = 50_000

# 2026-08-10: Sheets returns 429 on write-quota exhaustion (~60 write requests
# per minute per user per project). 5xx are transient backend errors. Both are
# retryable; everything else is a real error and must surface immediately.
RETRY_STATUSES = frozenset({429, 500, 502, 503, 504})
RETRY_EXCEPTIONS = (socket.timeout, TimeoutError, ConnectionError, ssl.SSLError)

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


class PartialWriteError(UploadError):
    """The destination tab was left in an inconsistent state."""


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
                   help="replace = overwrite the tab; append = add below "
                        "existing rows.")
    p.add_argument("--replace-strategy", choices=("in-place", "swap"),
                   default="in-place",
                   help="in-place keeps tab formatting but can leave partial "
                        "data on failure; swap is atomic but recreates the tab.")
    p.add_argument("--start-cell", default="A1",
                   help="Top-left cell for replace mode. Ignored when "
                        "--mode append.")
    p.add_argument("--value-input-option", choices=("RAW", "USER_ENTERED"),
                   default="USER_ENTERED",
                   help="RAW writes strings verbatim (use for zip codes, phone "
                        "numbers, click IDs); USER_ENTERED lets Sheets parse "
                        "numbers, dates and formulas.")
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

    p.add_argument("--max-retries", type=int, default=5,
                   help="Retries per request on 429/5xx before giving up.")
    p.add_argument("--requests-per-minute", type=int, default=55,
                   help="Client-side write throttle. 0 disables it.")
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
# Rate limiting and retry
# --------------------------------------------------------------------------
class RateLimiter:
    """Sliding-window throttle. Allows a full burst, then paces to the cap.

    2026-08-10: added because the Sheets write quota is per-minute, not
    per-second. A large chunked upload will otherwise burn the quota and start
    429ing part way through -- recoverable via retry, but slower and noisier
    than simply not exceeding it.
    """

    def __init__(self, per_minute: int, sleep=time.sleep, clock=time.monotonic):
        self.per_minute = per_minute
        self._sleep = sleep
        self._clock = clock
        self._window = collections.deque()

    def _evict(self, now: float) -> None:
        while self._window and now - self._window[0] >= 60.0:
            self._window.popleft()

    def acquire(self) -> None:
        if self.per_minute <= 0:
            return
        now = self._clock()
        self._evict(now)
        if len(self._window) >= self.per_minute:
            wait = 60.0 - (now - self._window[0])
            if wait > 0:
                self._sleep(wait)
            now = self._clock()
            self._evict(now)
        self._window.append(self._clock())


def execute_with_retry(request, what: str, max_retries: int = 5,
                       limiter: "RateLimiter | None" = None,
                       sleep=time.sleep):
    """Execute a googleapiclient request, retrying 429/5xx with backoff."""
    from googleapiclient.errors import HttpError

    attempt = 0
    while True:
        if limiter is not None:
            limiter.acquire()
        try:
            return request.execute(num_retries=0)
        except HttpError as exc:
            status = getattr(getattr(exc, "resp", None), "status", None)
            if status not in RETRY_STATUSES or attempt >= max_retries:
                raise
            reason = f"HTTP {status}"
        except RETRY_EXCEPTIONS as exc:
            if attempt >= max_retries:
                raise
            reason = f"{type(exc).__name__}: {exc}"

        attempt += 1
        delay = min(60.0, 2.0 * (2 ** (attempt - 1))) + random.uniform(0, 1.0)
        print(f"  WARN {what}: {reason} -- retry {attempt}/{max_retries} in {delay:.1f}s", 
            file=sys.stderr, flush=True)
        sleep(delay)


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


def build_service(credentials_path: "str | None"):
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


def parse_start_cell(cell: str) -> tuple:
    match = re.fullmatch(r"([A-Za-z]+)(\d+)", cell.strip())
    if not match:
        raise UploadError(f"--start-cell must look like 'A1', got: {cell}")
    col = 0
    for ch in match.group(1).upper():
        col = col * 26 + (ord(ch) - 64)
    return int(match.group(2)), col


def find_sheet(service, spreadsheet_id: str, tab: str, ctx):
    from googleapiclient.errors import HttpError

    try:
        meta = execute_with_retry(
            service.spreadsheets().get(
                spreadsheetId=spreadsheet_id,
                fields="sheets.properties(sheetId,title,index,gridProperties)",
            ), "fetch spreadsheet metadata", ctx.max_retries, ctx.limiter)
    except HttpError as exc:
        raise UploadError(_explain_http_error(exc, spreadsheet_id)) from exc

    ctx.all_titles = [s["properties"]["title"] for s in meta.get("sheets", [])]
    for sheet in meta.get("sheets", []):
        if sheet["properties"]["title"] == tab:
            return sheet["properties"]
    return None


def add_sheet(service, spreadsheet_id: str, title: str, rows: int, cols: int,
              ctx, hidden: bool = False) -> dict:
    props = {
        "title": title,
        "gridProperties": {"rowCount": max(rows, 1000),
                           "columnCount": max(cols, 26)},
    }
    if hidden:
        props["hidden"] = True
    result = execute_with_retry(
        service.spreadsheets().batchUpdate(
            spreadsheetId=spreadsheet_id,
            body={"requests": [{"addSheet": {"properties": props}}]}),
        f"create tab {title!r}", ctx.max_retries, ctx.limiter)
    return result["replies"][0]["addSheet"]["properties"]


def delete_sheet(service, spreadsheet_id: str, sheet_id: int, ctx) -> None:
    execute_with_retry(
        service.spreadsheets().batchUpdate(
            spreadsheetId=spreadsheet_id,
            body={"requests": [{"deleteSheet": {"sheetId": sheet_id}}]}),
        "delete temp tab", ctx.max_retries, ctx.limiter)


def ensure_grid_size(service, spreadsheet_id: str, props: dict,
                     needed_rows: int, needed_cols: int, ctx) -> None:
    """Grow the tab if the incoming data would overflow it. Never shrinks."""
    grid = props.get("gridProperties", {})
    current_rows = grid.get("rowCount", 0)
    current_cols = grid.get("columnCount", 0)
    if needed_rows <= current_rows and needed_cols <= current_cols:
        return

    execute_with_retry(
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
                "fields":
                    "gridProperties.rowCount,gridProperties.columnCount",
            }}]}),
        "resize grid", ctx.max_retries, ctx.limiter)


def write_chunks(service, spreadsheet_id: str, tab: str, rows, width: int,
                 start_row: int, start_col: int, ctx) -> int:
    chunk_rows = max(1, MAX_CELLS_PER_REQUEST // max(width, 1))
    end_col = col_letter(start_col + width - 1)
    written = 0

    for offset in range(0, len(rows), chunk_rows):
        chunk = rows[offset:offset + chunk_rows]
        top = start_row + offset
        bottom = top + len(chunk) - 1
        cell_range = f"{col_letter(start_col)}{top}:{end_col}{bottom}"
        execute_with_retry(
            service.spreadsheets().values().update(
                spreadsheetId=spreadsheet_id,
                range=a1(tab, cell_range),
                valueInputOption=ctx.value_input_option,
                body={"values": chunk}),
            f"write rows {top}-{bottom}", ctx.max_retries, ctx.limiter)
        written += len(chunk)
        _progress(written, len(rows))
    return written


def upload_replace_in_place(service, spreadsheet_id, tab, props, rows, width,
                            start_row, start_col, ctx) -> int:
    ensure_grid_size(service, spreadsheet_id, props,
                     len(rows) + start_row - 1, width + start_col - 1, ctx)
    execute_with_retry(
        service.spreadsheets().values().clear(
            spreadsheetId=spreadsheet_id, range=a1(tab), body={}),
        "clear tab", ctx.max_retries, ctx.limiter)

    try:
        return write_chunks(service, spreadsheet_id, tab, rows, width,
                            start_row, start_col, ctx)
    except Exception as exc:
        _stamp_failure(service, spreadsheet_id, tab, start_row, start_col, ctx)
        raise PartialWriteError(
            f"Write failed part way through; tab {tab!r} now holds partial "
            f"data and has been marked in-sheet. Re-run to restore it. "
            f"Underlying error: {_describe(exc, spreadsheet_id)}"
        ) from exc


def upload_replace_swap(service, spreadsheet_id, tab, props, rows, width,
                        start_row, start_col, ctx) -> int:
    """Write to a hidden temp tab, then delete + rename atomically."""
    temp_title = f"_upl_{os.getpid()}_{int(time.time())}"[:100]
    temp = add_sheet(service, spreadsheet_id, temp_title,
                     len(rows) + start_row - 1, width + start_col - 1,
                     ctx, hidden=True)
    committed = False
    try:
        written = write_chunks(service, spreadsheet_id, temp_title, rows, width,
                               start_row, start_col, ctx)
        # Ordered within one batchUpdate: the delete frees the title before the
        # rename claims it, and the whole request is applied atomically.
        execute_with_retry(
            service.spreadsheets().batchUpdate(
                spreadsheetId=spreadsheet_id,
                body={"requests": [
                    {"deleteSheet": {"sheetId": props["sheetId"]}},
                    {"updateSheetProperties": {
                        "properties": {
                            "sheetId": temp["sheetId"],
                            "title": tab,
                            "index": props.get("index", 0),
                            "hidden": False,
                        },
                        "fields": "title,index,hidden",
                    }},
                ]}),
            "swap temp tab into place", ctx.max_retries, ctx.limiter)
        committed = True
        return written
    finally:
        if not committed:
            try:
                delete_sheet(service, spreadsheet_id, temp["sheetId"], ctx)
                print(f"  cleaned up temp tab {temp_title!r}; destination tab "
                      f"{tab!r} was left untouched.",
                      file=sys.stderr, flush=True)
            except Exception:
                print(f"  WARNING: could not remove temp tab {temp_title!r}; "
                      f"delete it manually.", file=sys.stderr, flush=True)


def upload_append(service, spreadsheet_id, tab, rows, width, ctx) -> int:
    chunk_rows = max(1, MAX_CELLS_PER_REQUEST // max(width, 1))
    written = 0
    for offset in range(0, len(rows), chunk_rows):
        chunk = rows[offset:offset + chunk_rows]
        execute_with_retry(
            service.spreadsheets().values().append(
                spreadsheetId=spreadsheet_id,
                range=a1(tab),
                valueInputOption=ctx.value_input_option,
                insertDataOption="INSERT_ROWS",
                body={"values": chunk}),
            f"append chunk at row {offset}", ctx.max_retries, ctx.limiter)
        written += len(chunk)
        _progress(written, len(rows))
    return written


def _stamp_failure(service, spreadsheet_id, tab, start_row, start_col, ctx):
    """Overwrite the top-left data cell so a partial upload is visible in-sheet.

    2026-08-10: deliberately destructive. A half-written tab that looks clean is
    worse than one that obviously isn't.
    """
    stamp = (f"*** UPLOAD FAILED {_dt.datetime.now():%Y-%m-%d %H:%M:%S} -- "
             f"THIS TAB HOLDS PARTIAL DATA. DO NOT USE. ***")
    cell = f"{col_letter(start_col)}{start_row}"
    try:
        execute_with_retry(
            service.spreadsheets().values().update(
                spreadsheetId=spreadsheet_id,
                range=a1(tab, cell),
                valueInputOption="RAW",
                body={"values": [[stamp]]}),
            "write failure marker", max_retries=1, limiter=ctx.limiter)
    except Exception:
        print("  WARNING: could not write the in-sheet failure marker either; "
              "the tab holds partial data with nothing flagging it.",
              file=sys.stderr, flush=True)


def _progress(done: int, total: int) -> None:
    print(f"  wrote {done:,} / {total:,} rows", flush=True)


def _describe(exc, spreadsheet_id: str) -> str:
    try:
        from googleapiclient.errors import HttpError
    except ImportError:
        return f"{type(exc).__name__}: {exc}"
    if isinstance(exc, HttpError):
        return _explain_http_error(exc, spreadsheet_id)
    return f"{type(exc).__name__}: {exc}"


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
    if status == 429:
        return ("Rate limited by the Sheets API and still failing after "
                "retries. Lower --requests-per-minute and re-run.")
    return f"Google Sheets API error ({status}): {exc}"


# --------------------------------------------------------------------------
# Main
# --------------------------------------------------------------------------
class Context:
    """Everything the write helpers need that isn't the data itself."""

    def __init__(self, args):
        self.max_retries = args.max_retries
        self.value_input_option = args.value_input_option
        self.limiter = RateLimiter(args.requests_per_minute)
        self.all_titles = []


def run(args) -> int:
    spreadsheet_id = extract_spreadsheet_id(args.sheet_url)
    start_row, start_col = parse_start_cell(args.start_cell)
    if args.mode == "append" and args.start_cell != "A1":
        print("WARNING: --start-cell is ignored in append mode.",
              file=sys.stderr, flush=True)

    print(f"Reading {args.data_file} ...", flush=True)
    rows, width = normalize(read_rows(args))
    print(f"  {len(rows):,} rows x {width} columns", flush=True)

    needed_rows = len(rows) + (start_row - 1)
    needed_cols = width + (start_col - 1)
    if needed_rows * needed_cols > 10_000_000:
        raise UploadError(
            f"{needed_rows:,} x {needed_cols} exceeds the 10,000,000-cell "
            f"limit for a Google spreadsheet.")

    if args.dry_run:
        strategy = f" ({args.replace_strategy})" if args.mode == "replace" else ""
        print(f"DRY RUN -- nothing written.\n"
              f"  spreadsheet: {spreadsheet_id}\n"
              f"  tab:         {args.tab}\n"
              f"  mode:        {args.mode}{strategy}\n"
              f"  first row:   {rows[0][:8]}")
        return 0

    ctx = Context(args)
    # 2026-08-10: build_service() first -- it converts a missing google client
    # library into a clean UploadError. Importing HttpError above it would let
    # the raw ImportError escape as an exit-2 traceback instead.
    service = build_service(args.credentials)
    from googleapiclient.errors import HttpError

    props = find_sheet(service, spreadsheet_id, args.tab, ctx)

    if props is None:
        if not args.create_tab:
            raise UploadError(
                f"Tab {args.tab!r} not found. Existing tabs: "
                f"{', '.join(ctx.all_titles)}. Pass --create-tab to create it.")
        props = add_sheet(service, spreadsheet_id, args.tab,
                          needed_rows, needed_cols, ctx)
        fresh = True
    else:
        fresh = False

    try:
        if args.mode == "append":
            written = upload_append(service, spreadsheet_id, args.tab,
                                    rows, width, ctx)
            how = "append"
        elif args.replace_strategy == "swap" and not fresh:
            # A tab we just created has nothing to lose, so skip the swap.
            written = upload_replace_swap(service, spreadsheet_id, args.tab,
                                          props, rows, width,
                                          start_row, start_col, ctx)
            how = "replace/swap"
        else:
            written = upload_replace_in_place(service, spreadsheet_id, args.tab,
                                              props, rows, width,
                                              start_row, start_col, ctx)
            how = "replace/in-place"
    except PartialWriteError:
        raise
    except HttpError as exc:
        raise UploadError(_explain_http_error(exc, spreadsheet_id)) from exc

    print(f"Done. {written:,} rows uploaded to '{args.tab}' ({how}).")
    return 0


def main(argv=None) -> int:
    args = parse_args(argv)
    try:
        return run(args)
    except PartialWriteError as exc:
        print(f"ERROR: {exc}", file=sys.stderr)
        return 3
    except UploadError as exc:
        print(f"ERROR: {exc}", file=sys.stderr)
        return 1
    except KeyboardInterrupt:
        print("ERROR: interrupted; the destination tab may hold partial data.",
              file=sys.stderr)
        return 3
    except Exception:
        traceback.print_exc()
        return 2


if __name__ == "__main__":
    sys.exit(main())