# Report Smart App Control mode (Off / Evaluation / On).

$ErrorActionPreference = "SilentlyContinue"

$mode = "Unknown"
$raw = $null

$reg = Get-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Control\CI\Policy" `
    -Name "VerifiedAndReputablePolicyState" -ErrorAction SilentlyContinue
if ($reg) {
    $raw = [int]$reg.VerifiedAndReputablePolicyState
    switch ($raw) {
        0 { $mode = "Off" }
        1 { $mode = "Evaluation" }
        2 { $mode = "On (Enforcement)" }
        default { $mode = "Unknown (registry value $raw)" }
    }
}

if ($mode -eq "Unknown") {
    try {
        $sac = (Get-MpComputerStatus).SmartAppControlState
        switch ($sac) {
            0 { $mode = "Off" }
            1 { $mode = "Evaluation" }
            2 { $mode = "On (Enforcement)" }
        }
    } catch {}
}

Write-Host "Smart App Control: $mode"

if ($mode -match "Evaluation|On") {
    Write-Host ""
    Write-Host "Unsigned Struggler.dll builds will be blocked while SAC is active."
    Write-Host "Long-term fix (keep SAC on):"
    Write-Host "  1. Apply at https://signpath.org/ (free OSS signing) — see SIGNPATH.md"
    Write-Host "  2. ./scripts/install-signed.sh after first signed release"
    Write-Host "  3. Or buy a trusted cert + install.sh --sign"
    exit 2
}

exit 0
