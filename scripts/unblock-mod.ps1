# Remove Mark-of-the-Web from mod files copied via WSL/network.
# Note: Smart App Control blocks unsigned DLLs regardless — see CONTRIBUTING.md.

param(
    [Parameter(Mandatory = $true)]
    [string]$ModsDir
)

if (-not (Test-Path -LiteralPath $ModsDir)) {
    Write-Error "Mod folder not found: $ModsDir"
    exit 1
}

$files = Get-ChildItem -LiteralPath $ModsDir -File -Recurse
$unblocked = 0

foreach ($file in $files) {
    $streams = Get-Item -LiteralPath $file.FullName -Stream * -ErrorAction SilentlyContinue
    if ($streams | Where-Object { $_.Stream -eq 'Zone.Identifier' }) {
        Unblock-File -LiteralPath $file.FullName
        $unblocked++
        Write-Host "Unblocked: $($file.Name)"
    }
}

if ($unblocked -eq 0) {
    Write-Host "No Mark-of-the-Web flags found (Smart App Control may still block unsigned DLLs)."
} else {
    Write-Host "Unblocked $unblocked file(s)."
}
