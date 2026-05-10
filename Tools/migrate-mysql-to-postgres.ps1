<#
.SYNOPSIS
    Converts a phpMyAdmin MySQL dump to a PostgreSQL-compatible SQL file for Trenton Darts.

.PARAMETER SourceFile
    Path to the MySQL .sql export file.

.PARAMETER OutputFile
    Path to write the generated PostgreSQL SQL file.

.EXAMPLE
    .\migrate-mysql-to-postgres.ps1
    .\migrate-mysql-to-postgres.ps1 -SourceFile "C:\path\to\dump.sql" -OutputFile ".\migration-output.sql"
#>
param(
    [string]$SourceFile = "C:\Users\johnn\Downloads\trentondarts_export.sql",
    [string]$OutputFile = "$PSScriptRoot\migration-output.sql"
)

Set-StrictMode -Off
$ErrorActionPreference = 'Stop'

# ─── Per-table configuration ─────────────────────────────────────────────────
#   Renames    : MySQL column name → PostgreSQL column name (double-quoted in output)
#   BoolCols   : columns to convert 0→false / 1→true
#   NullCols   : columns to force to NULL (e.g. userId)
#   InjectCols : {pgColumnName = literalValue} to add columns not in MySQL
#   ExcludeCols: MySQL columns to skip entirely

$TableConfigs = @{
    sponsors = @{
        Renames     = @{ name='Name'; type='Type'; city='City'; state='State'; zip='Zip'
                         phone='Phone'; url='Url'; email='Email'; description='Description'; comments='Comments' }
        BoolCols    = @()
        NullCols    = @()
        InjectCols  = @{}
        ExcludeCols = @()
    }
    match_types = @{
        Renames = @{ name='Name' }; BoolCols = @(); NullCols = @(); InjectCols = @{}; ExcludeCols = @()
    }
    players = @{
        Renames     = @{ email='Email'; city='City'; state='State'; zip='Zip'; notes='Notes' }
        BoolCols    = @('acceptText','acceptEmail')
        NullCols    = @('userId')
        InjectCols  = @{}
        ExcludeCols = @()
    }
    teams = @{
        Renames = @{ name='Name'; notes='Notes' }
        BoolCols = @(); NullCols = @(); InjectCols = @{}; ExcludeCols = @()
    }
    board_members = @{
        Renames     = @{ name='Name'; position='Position' }
        BoolCols    = @()
        NullCols    = @('userId')
        InjectCols  = @{}
        ExcludeCols = @()
    }
    # browsable_files handled specially (inject Title + slug columns)
    browsable_files = @{
        Renames     = @{ category='Category' }
        BoolCols    = @()
        NullCols    = @()
        InjectCols  = @{}
        ExcludeCols = @()
        SpecialTitle = $true
    }
    page_parts = @{
        Renames = @{ name='Name'; description='Description'; html='Html' }
        BoolCols = @(); NullCols = @(); InjectCols = @{}; ExcludeCols = @()
    }
    dart_events = @{
        Renames     = @{ name='Name'; city='City'; state='State'; zip='Zip'; url='Url'; description='Description' }
        BoolCols    = @('isTitleEvent')
        NullCols    = @()
        InjectCols  = @{}
        ExcludeCols = @()
    }
    dart_event_results = @{
        Renames = @{ finished='Finished' }
        BoolCols = @(); NullCols = @(); InjectCols = @{}; ExcludeCols = @()
    }
    match_type_game_rules = @{
        Renames     = @{}
        BoolCols    = @('doubleIn','doubleOut','forfeitIfNoPlayers')
        NullCols    = @()
        InjectCols  = @{}
        ExcludeCols = @()
    }
    winter_seasons = @{
        Renames     = @{ name='Name' }
        BoolCols    = @('isCurrent','isUsingMatchPoints','accumulatePointsForAllParts')
        NullCols    = @()
        InjectCols  = @{}
        ExcludeCols = @()
    }
    winter_season_weeks = @{
        Renames = @{ date='Date' }
        BoolCols = @(); NullCols = @(); InjectCols = @{}; ExcludeCols = @()
    }
    winter_season_teams = @{
        Renames = @{}; BoolCols = @(); NullCols = @(); InjectCols = @{ leagueId = '1' }; ExcludeCols = @()
    }
    winter_season_team_players = @{
        Renames = @{ role='Role' }
        BoolCols = @(); NullCols = @(); InjectCols = @{}; ExcludeCols = @()
    }
    winter_season_matches = @{
        Renames     = @{ division='Division' }
        BoolCols    = @()
        NullCols    = @()
        InjectCols  = @{ leagueId = '1' }
        ExcludeCols = @()
    }
    winter_match_results = @{
        Renames = @{}
        BoolCols = @('hasScorecard')
        NullCols = @(); InjectCols = @{}; ExcludeCols = @()
    }
    winter_game_results = @{
        Renames = @{ legs='Legs' }; BoolCols = @(); NullCols = @(); InjectCols = @{}; ExcludeCols = @()
    }
    winter_game_awards = @{
        Renames = @{ value='Value' }
        BoolCols = @(); NullCols = @(); InjectCols = @{}; ExcludeCols = @()
    }
    winter_season_player_payments = @{
        Renames = @{}; BoolCols = @(); NullCols = @(); InjectCols = @{}; ExcludeCols = @()
    }
    winter_season_team_payments = @{
        Renames = @{}; BoolCols = @(); NullCols = @(); InjectCols = @{}; ExcludeCols = @()
    }
    winter_stats_matches = @{
        Renames = @{ date='Date' }
        BoolCols = @('homeMatch','hasScorecard')
        NullCols = @(); InjectCols = @{}; ExcludeCols = @('division')
    }
    winter_stats_team_games = @{
        Renames = @{ date='Date' }
        BoolCols = @('isWon','isForfeitGame')
        NullCols = @(); InjectCols = @{}; ExcludeCols = @('division')
    }
    winter_stats_player_games = @{
        Renames = @{ date='Date' }
        BoolCols = @('isWon','isForfeit','isHome')
        NullCols = @(); InjectCols = @{}; ExcludeCols = @('division')
    }
    winter_stats_awards = @{
        Renames = @{ date='Date'; value='Value' }
        BoolCols = @(); NullCols = @(); InjectCols = @{}; ExcludeCols = @('division')
    }
}

$TableOrder = @(
    'sponsors','match_types','players','teams','board_members',
    'browsable_files','page_parts','dart_events','dart_event_results',
    'match_type_game_rules','winter_seasons','winter_season_weeks',
    'winter_season_teams','winter_season_team_players','winter_season_matches',
    'winter_match_results','winter_game_results','winter_game_awards',
    'winter_season_player_payments','winter_season_team_payments',
    'winter_stats_matches','winter_stats_team_games','winter_stats_player_games',
    'winter_stats_awards'
)

# ─── Helper: derive a readable title from a file name ────────────────────────
function Get-TitleFromFileName([string]$fileName) {
    $base = [System.IO.Path]::GetFileNameWithoutExtension($fileName)
    $base = $base -replace '[-_]+', ' '
    $base = $base.Trim()
    $base = $base -replace "'", "''"   # escape single quotes for SQL
    return "'$base'"
}

# ─── Helper: parse VALUE tuples from the body of a VALUES clause ─────────────
# Returns a List of string arrays. Each string array is one row of raw SQL values.
function Split-ValueRows([string]$valuesBody) {
    $rows  = [System.Collections.Generic.List[string[]]]::new()
    $row   = [System.Collections.Generic.List[string]]::new()
    $val   = [System.Text.StringBuilder]::new()
    $inStr = $false
    $depth = 0
    $i     = 0
    $n     = $valuesBody.Length

    while ($i -lt $n) {
        $c = $valuesBody[$i]

        if ($inStr) {
            if ($c -eq '\' -and ($i + 1) -lt $n) {
                $nx = $valuesBody[$i + 1]
                if ($nx -eq "'") {
                    # MySQL \' escape → PostgreSQL ''
                    $val.Append("''") | Out-Null
                    $i += 2; continue
                } elseif ($nx -eq '\') {
                    $val.Append('\\') | Out-Null
                    $i += 2; continue
                } else {
                    # other escape sequences (\n, \r, \t, \0, etc.) — keep as-is
                    $val.Append($c) | Out-Null
                }
            } elseif ($c -eq "'") {
                # End of string
                $inStr = $false
                $val.Append($c) | Out-Null
            } else {
                $val.Append($c) | Out-Null
            }
        } else {
            if ($c -eq "'") {
                $inStr = $true
                $val.Append($c) | Out-Null
            } elseif ($c -eq '(') {
                $depth++
                if ($depth -gt 1) { $val.Append($c) | Out-Null }
            } elseif ($c -eq ')') {
                if ($depth -eq 1) {
                    # End of this row
                    $row.Add($val.ToString().Trim()) | Out-Null
                    $rows.Add($row.ToArray()) | Out-Null
                    $row = [System.Collections.Generic.List[string]]::new()
                    $val = [System.Text.StringBuilder]::new()
                    $depth = 0
                } else {
                    $depth--
                    $val.Append($c) | Out-Null
                }
            } elseif ($c -eq ',' -and $depth -eq 1) {
                # Column separator within a row
                $row.Add($val.ToString().Trim()) | Out-Null
                $val = [System.Text.StringBuilder]::new()
            } else {
                if ($depth -ge 1) { $val.Append($c) | Out-Null }
            }
        }
        $i++
    }

    return $rows
}

# ─── Helper: transform one raw SQL value for a given column ──────────────────
function Transform-Value([string]$rawVal, [string]$colName, [hashtable]$cfg) {
    # Force NULL
    if ($cfg.NullCols -contains $colName) { return 'NULL' }

    # Boolean conversion
    if ($cfg.BoolCols -contains $colName) {
        if ($rawVal -eq '0') { return 'false' }
        if ($rawVal -eq '1') { return 'true'  }
        return $rawVal  # handles NULL
    }

    # Zero timestamps/dates (invalid in PostgreSQL) -> minimum representable value
    if ($rawVal -eq "'0000-00-00 00:00:00'") { return "'1970-01-01 00:00:00'" }
    if ($rawVal -eq "'0000-00-00'")           { return "'1970-01-01'" }

    # Non-nullable timestamp columns with genuinely NULL MySQL values
    if ($rawVal -eq 'NULL' -and ($colName -eq 'created_at' -or $colName -eq 'updated_at')) {
        return "'1970-01-01 00:00:00'"
    }

    # Per-table column defaults when value is NULL
    if ($rawVal -eq 'NULL' -and $cfg.ContainsKey('DefaultCols') -and $cfg.DefaultCols.ContainsKey($colName)) {
        return $cfg.DefaultCols[$colName]
    }

    return $rawVal
}

# ─── Helper: find all INSERT bodies for a table using a string-aware scanner ──
# phpMyAdmin may emit multiple INSERT blocks per table. Returns array of
# [colHeader, valuesBody] pairs so callers process every batch of rows.
function Get-InsertBlocks([string]$content, [string]$tableName) {
    $results = [System.Collections.Generic.List[object]]::new()
    $marker  = "INSERT INTO ``$tableName``"
    $pos     = 0
    $n       = $content.Length

    while ($true) {
        $start = $content.IndexOf($marker, $pos)
        if ($start -lt 0) { break }

        # Locate ' VALUES' keyword after the column list
        $valKw = $content.IndexOf(' VALUES', $start)
        if ($valKw -lt 0) { break }

        # Extract column header: text between first ( and its matching )
        $lp = $content.IndexOf('(', $start)
        if ($lp -lt 0 -or $lp -gt $valKw) { break }
        $rp = $content.IndexOf(')', $lp)
        $colHeader = $content.Substring($lp + 1, $rp - $lp - 1)

        # Scan from after VALUES to find the terminating ; outside any string
        $i     = $valKw + 7   # skip ' VALUES'
        $inStr = $false
        $endIdx = -1

        while ($i -lt $n) {
            $c = $content[$i]
            if ($inStr) {
                if ($c -eq '\' -and ($i + 1) -lt $n) { $i++ }   # skip escaped char
                elseif ($c -eq "'") { $inStr = $false }
            } else {
                if ($c -eq "'") { $inStr = $true }
                elseif ($c -eq ';') { $endIdx = $i; break }
            }
            $i++
        }

        if ($endIdx -lt 0) { break }

        $valuesBody = $content.Substring($valKw + 7, $endIdx - ($valKw + 7))
        $results.Add([pscustomobject]@{ ColHeader = $colHeader; ValuesBody = $valuesBody }) | Out-Null
        $pos = $endIdx + 1
    }

    return $results
}

# ─── Helper: convert a list of row arrays to formatted SQL value lines ────────
function Convert-Rows([string[]]$sourceCols, $rows, [hashtable]$cfg, [int]$fileNameIdx) {
    $lines = [System.Collections.Generic.List[string]]::new()

    foreach ($row in $rows) {
        $pgVals = [System.Collections.Generic.List[string]]::new()

        if ($fileNameIdx -ge 0) {
            $pgVals.Add($row[0]) | Out-Null   # Id
            $fn = if ($fileNameIdx -lt $row.Length) { $row[$fileNameIdx].Trim("'") } else { '' }
            $pgVals.Add((Get-TitleFromFileName $fn)) | Out-Null   # Title
            for ($ci = 1; $ci -lt $sourceCols.Length; $ci++) {
                $col = $sourceCols[$ci]
                if ($cfg.ExcludeCols -contains $col) { continue }
                $pgVals.Add((Transform-Value $row[$ci] $col $cfg)) | Out-Null
            }
            $pgVals.Add('NULL') | Out-Null   # slug
        } else {
            for ($ci = 0; $ci -lt $sourceCols.Length; $ci++) {
                $col = $sourceCols[$ci]
                if ($cfg.ExcludeCols -contains $col) { continue }
                $pgVals.Add((Transform-Value $row[$ci] $col $cfg)) | Out-Null
            }
            foreach ($injectVal in $cfg.InjectCols.Values) {
                $pgVals.Add($injectVal) | Out-Null
            }
        }

        $lines.Add("  ($($pgVals -join ', '))") | Out-Null
    }

    return $lines
}

# ─── Main ─────────────────────────────────────────────────────────────────────
Write-Host "Reading $SourceFile ..."
$content = [System.IO.File]::ReadAllText($SourceFile)
Write-Host "  $([math]::Round($content.Length / 1MB, 1)) MB loaded."

$writer = [System.IO.StreamWriter]::new($OutputFile, $false, [System.Text.UTF8Encoding]::new($false))

$writer.WriteLine("-- ============================================================")
$writer.WriteLine("-- Migration: MySQL (trentondarts) -> PostgreSQL")
$writer.WriteLine("-- Generated : $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
$writer.WriteLine("-- Source    : $SourceFile")
$writer.WriteLine("-- ============================================================")
$writer.WriteLine("")
$writer.WriteLine("-- Disable FK checks while loading data")
$writer.WriteLine("SET session_replication_role = replica;")
$writer.WriteLine("")

$totalRows = 0

foreach ($tableName in $TableOrder) {
    $cfg = $TableConfigs[$tableName]

    # Find ALL INSERT blocks for this table (handles phpMyAdmin batch splits)
    $blocks = Get-InsertBlocks $content $tableName

    if ($blocks.Count -eq 0) {
        Write-Warning "  No INSERT found for: $tableName -- skipping"
        continue
    }

    # Column info is the same across all blocks for a table — read from first block
    $sourceCols = $blocks[0].ColHeader -split ',\s*' | ForEach-Object { $_.Trim().Trim('`') }

    $fileNameIdx = -1
    if ($cfg.ContainsKey('SpecialTitle') -and $cfg.SpecialTitle) {
        $fileNameIdx = [Array]::IndexOf([string[]]$sourceCols, 'fileName')
    }

    # Build PostgreSQL column list (done once per table)
    $pgCols = [System.Collections.Generic.List[string]]::new()
    if ($fileNameIdx -ge 0) {
        $pgCols.Add('"Id"') | Out-Null
        $pgCols.Add('"Title"') | Out-Null
        foreach ($col in $sourceCols) {
            if ($col -eq 'id') { continue }
            if ($cfg.ExcludeCols -contains $col) { continue }
            $pgCol = if ($cfg.Renames.ContainsKey($col)) { $cfg.Renames[$col] } else { $col }
            $pgCols.Add("`"$pgCol`"") | Out-Null
        }
        $pgCols.Add('"slug"') | Out-Null
    } else {
        foreach ($col in $sourceCols) {
            if ($cfg.ExcludeCols -contains $col) { continue }
            $pgCol = if ($cfg.Renames.ContainsKey($col)) { $cfg.Renames[$col] } else { $col }
            if ($pgCol -eq 'id') { $pgCol = 'Id' }
            $pgCols.Add("`"$pgCol`"") | Out-Null
        }
        foreach ($injectCol in $cfg.InjectCols.Keys) {
            $pgCols.Add("`"$injectCol`"") | Out-Null
        }
    }

    $pgColHeader = $pgCols -join ', '

    # Collect all row lines across every INSERT block for this table
    $allRowLines = [System.Collections.Generic.List[string]]::new()
    foreach ($block in $blocks) {
        $rows = Split-ValueRows $block.ValuesBody
        $lines = Convert-Rows $sourceCols $rows $cfg $fileNameIdx
        foreach ($l in $lines) { $allRowLines.Add($l) | Out-Null }
    }

    $tableRowCount = $allRowLines.Count
    $totalRows += $tableRowCount
    Write-Host "  $tableName : $tableRowCount rows"

    $writer.WriteLine("-- ------------------------------------------------------------")
    $writer.WriteLine("-- $tableName  ($tableRowCount rows)")
    $writer.WriteLine("-- ------------------------------------------------------------")
    $writer.WriteLine("INSERT INTO $tableName ($pgColHeader) VALUES")

    for ($ri = 0; $ri -lt $allRowLines.Count; $ri++) {
        if ($ri -lt $allRowLines.Count - 1) {
            $writer.WriteLine($allRowLines[$ri] + ',')
        } else {
            $writer.WriteLine($allRowLines[$ri])
        }
    }
    $writer.WriteLine("ON CONFLICT DO NOTHING;")
    $writer.WriteLine("")
}

# ─── Re-enable FK checks ──────────────────────────────────────────────────────
$writer.WriteLine("-- Re-enable FK checks")
$writer.WriteLine("SET session_replication_role = DEFAULT;")
$writer.WriteLine("")

# ─── Sequence resets ─────────────────────────────────────────────────────────
$writer.WriteLine("-- Reset identity sequences so new INSERTs don't conflict with migrated IDs")
foreach ($t in $TableOrder) {
    $writer.WriteLine("SELECT setval(pg_get_serial_sequence('$t', 'Id'), COALESCE((SELECT MAX(`"Id`") FROM $t), 1));")
}

$writer.Flush()
$writer.Close()

$outSize = [math]::Round((Get-Item $OutputFile).Length / 1MB, 1)
Write-Host ""
Write-Host "Done! $totalRows total rows written to: $OutputFile ($outSize MB)"
Write-Host ""
Write-Host "Next steps:"
Write-Host "  1. Review the output file (optional): code '$OutputFile'"
Write-Host "  2. Run against PostgreSQL:"
Write-Host "       psql -U postgres -d trentondarts -f '$OutputFile'"
Write-Host "     or open it in pgAdmin Query Tool -> Run"
