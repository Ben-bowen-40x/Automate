param(
    [Parameter(Mandatory)][string]$SecretsPath,
    [Parameter(Mandatory)][string]$OutPath,
    [string]$Schema   # optional: ConnectionStrings key to use; defaults to the first one
)

$ErrorActionPreference = 'Stop'

try {
    if (-not (Test-Path -LiteralPath $SecretsPath)) {
        throw "Secrets file not found: $SecretsPath"
    }

    # The secrets file is JSONC: it contains // and /* */ comments (and possibly trailing commas),
    # which ConvertFrom-Json rejects. Clean it with a string-aware scanner so "http://..." values survive.
    function ConvertTo-PlainJson([string]$text) {
        # Pass 1: remove comments
        $sb = New-Object Text.StringBuilder $text.Length
        $inStr = $false; $i = 0; $n = $text.Length
        while ($i -lt $n) {
            $c = $text[$i]
            if ($inStr) {
                [void]$sb.Append($c)
                if ($c -eq '\' -and $i + 1 -lt $n) { $i++; [void]$sb.Append($text[$i]) }
                elseif ($c -eq '"') { $inStr = $false }
                $i++
            }
            elseif ($c -eq '"') { $inStr = $true; [void]$sb.Append($c); $i++ }
            elseif ($c -eq '/' -and $i + 1 -lt $n -and $text[$i + 1] -eq '/') {
                while ($i -lt $n -and $text[$i] -ne "`n") { $i++ }   # keep the newline itself
            }
            elseif ($c -eq '/' -and $i + 1 -lt $n -and $text[$i + 1] -eq '*') {
                $i += 2
                while ($i + 1 -lt $n -and -not ($text[$i] -eq '*' -and $text[$i + 1] -eq '/')) { $i++ }
                $i += 2
            }
            else { [void]$sb.Append($c); $i++ }
        }

        # Pass 2: remove trailing commas before } or ]
        $s = $sb.ToString()
        $out = New-Object Text.StringBuilder $s.Length
        $inStr = $false; $i = 0; $n = $s.Length
        while ($i -lt $n) {
            $c = $s[$i]
            if ($inStr) {
                [void]$out.Append($c)
                if ($c -eq '\' -and $i + 1 -lt $n) { $i++; [void]$out.Append($s[$i]) }
                elseif ($c -eq '"') { $inStr = $false }
            }
            elseif ($c -eq '"') { $inStr = $true; [void]$out.Append($c) }
            elseif ($c -eq ',') {
                $j = $i + 1
                while ($j -lt $n -and [char]::IsWhiteSpace($s[$j])) { $j++ }
                if (-not ($j -lt $n -and ($s[$j] -eq '}' -or $s[$j] -eq ']'))) { [void]$out.Append($c) }
            }
            else { [void]$out.Append($c) }
            $i++
        }
        return $out.ToString()
    }

    $raw     = Get-Content -LiteralPath $SecretsPath -Raw
    $cleaned = ConvertTo-PlainJson $raw
    try {
        $json = $cleaned | ConvertFrom-Json
    }
    catch {
        $msg = $_.Exception.Message
        if ($msg -match '\((\d+)\)\s*$') {
            $pos   = [int]$Matches[1]
            $start = [Math]::Max(0, $pos - 20)
            $len   = [Math]::Min(40, $cleaned.Length - $start)
            $snip  = $cleaned.Substring($start, $len) -replace "`r", '\r' -replace "`n", '\n'
            throw "JSON parse failed: $msg`r`nCleaned text near position ${pos}: [$snip]"
        }
        throw
    }

    if (-not $json.ConnectionStrings) { throw "No 'ConnectionStrings' section in secrets file." }
    if (-not $json.DbPassword)        { throw "No 'DbPassword' value in secrets file." }

    $props = @($json.ConnectionStrings.PSObject.Properties)
    if ($props.Count -eq 0) { throw "'ConnectionStrings' is empty." }

    if ($Schema) {
        $entry = $props | Where-Object { $_.Name -eq $Schema } | Select-Object -First 1
        if (-not $entry) { throw "ConnectionStrings has no entry named '$Schema'." }
    } else {
        $entry = $props[0]
    }

    # Parse "key=value; key=value; ..." into a case-insensitive map
    $map = @{}
    foreach ($part in ([string]$entry.Value -split ';')) {
        $i = $part.IndexOf('=')
        if ($i -gt 0) {
            $map[$part.Substring(0, $i).Trim().ToLowerInvariant()] = $part.Substring($i + 1).Trim()
        }
    }

    function First-Value($keys) {
        foreach ($k in $keys) { if ($map.ContainsKey($k) -and $map[$k]) { return $map[$k] } }
        return $null
    }

    $dbHost = First-Value @('server', 'host', 'data source')
    $dbUser = First-Value @('uid', 'user id', 'user', 'username')
    $dbPort = First-Value @('port')
    $dbPass = [string]$json.DbPassword

    if (-not $dbHost) { throw "No server/host found in connection string '$($entry.Name)'." }
    if (-not $dbUser) { throw "No uid/user found in connection string '$($entry.Name)'." }

    # Option-file values: wrap in double quotes, escape backslash and double quote
    function Q([string]$v) { '"' + $v.Replace('\', '\\').Replace('"', '\"') + '"' }

    $lines = @('[client]', "host=$(Q $dbHost)", "user=$(Q $dbUser)", "password=$(Q $dbPass)")
    if ($dbPort) { $lines += "port=$dbPort" }

    # UTF-8 without BOM (mysql chokes on a BOM)
    [IO.File]::WriteAllText($OutPath, (($lines -join "`r`n") + "`r`n"), (New-Object Text.UTF8Encoding($false)))

    Write-Output "Using '$($entry.Name)' as $dbUser @ $dbHost"
    exit 0
}
catch {
    [Console]::Error.WriteLine($_.Exception.Message)
    exit 1
}