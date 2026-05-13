<#
.SYNOPSIS
    Re-emits migration-output.sql with two fixes applied:
      1. Large multi-value INSERTs are split into smaller batches (default 500 rows)
         so they can be streamed to a remote Postgres over WAN without hitting
         transmission/timeout limits.
      2. MySQL "zero as no-value" FK sentinels are converted to NULL where the
         original schema used 0 to mean "no foreign key" (e.g. teams.sponsorId).
#>
param(
    [string]$InputFile  = "$PSScriptRoot\migration-output.sql",
    [string]$OutputFile = "$PSScriptRoot\migration-output-new.sql",
    [int]   $BatchSize  = 500
)

$sw = [System.IO.StreamWriter]::new($OutputFile, $false, [System.Text.Encoding]::UTF8)

function Flush-Batch {
    param($header, $rows)
    if ($rows.Count -eq 0) { return }

    $sw.WriteLine($header)
    for ($i = 0; $i -lt $rows.Count; $i++) {
        $row = $rows[$i].TrimEnd()
        if ($i -lt $rows.Count - 1) {
            # non-last row in this batch: must have trailing comma
            if (-not $row.EndsWith(',')) { $row += ',' }
        } else {
            # last row in this batch: must NOT have trailing comma
            $row = $row.TrimEnd(',')
        }
        $sw.WriteLine($row)
    }
    $sw.WriteLine('ON CONFLICT DO NOTHING;')
    $rows.Clear()
}

try {
    # Disable statement timeout for the session before anything else
    $sw.WriteLine('SET statement_timeout = 0;')

    $inInsert     = $false
    $insertHeader = $null
    $currentTable = $null
    $pending      = [System.Collections.Generic.List[string]]::new()
    $lineNum      = 0

    foreach ($line in [System.IO.File]::ReadLines($InputFile)) {
        $lineNum++

        # Skip session_replication_role — requires superuser on DO managed Postgres
        if ($line -match 'session_replication_role') { continue }

        if (-not $inInsert -and $line -match '^INSERT INTO (\S+)') {
            $inInsert     = $true
            $insertHeader = $line
            $currentTable = $Matches[1].Trim('"')
            $pending.Clear()
            continue
        }

        if ($inInsert -and $line.Trim() -eq 'ON CONFLICT DO NOTHING;') {
            Flush-Batch $insertHeader $pending
            $inInsert     = $false
            $currentTable = $null
            continue
        }

        if ($inInsert) {
            # Fix MySQL zero-as-null FK sentinels per table
            if ($currentTable -eq 'teams') {
                # sponsorId=0 means no sponsor — pattern: closing quote, ', 0, ', opening quote
                $line = $line -replace "', 0, '", "', NULL, '"
            }
            $pending.Add($line)
            if ($pending.Count -ge $BatchSize) {
                Flush-Batch $insertHeader $pending
            }
            continue
        }

        # Pass-through: comments, SET commands, SELECT setval(), blank lines
        $sw.WriteLine($line)
    }

    Write-Host "Done. $lineNum lines read -> $OutputFile"
}
finally {
    $sw.Close()
}
