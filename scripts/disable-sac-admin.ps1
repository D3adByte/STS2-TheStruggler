# Turn off Smart App Control so unsigned Struggler.dll can load.
# MUST run as Administrator, then REBOOT Windows.
#
# Right-click PowerShell -> Run as administrator, then:
#   Set-ExecutionPolicy Bypass -Scope Process -Force
#   & "C:\path\to\repo\scripts\disable-sac-admin.ps1"
#
# Or from WSL (opens elevated PowerShell):
#   powershell.exe -Command "Start-Process powershell -Verb RunAs -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File \"$(wslpath -w /home/dead/STS2/Struggler/scripts/disable-sac-admin.ps1)\"'"

#Requires -RunAsAdministrator

$ErrorActionPreference = "Stop"

$policyPath = "HKLM:\SYSTEM\CurrentControlSet\Control\CI\Policy"
$current = (Get-ItemProperty -Path $policyPath -Name "VerifiedAndReputablePolicyState" -ErrorAction SilentlyContinue).VerifiedAndReputablePolicyState

Write-Host "Current VerifiedAndReputablePolicyState: $current"
Write-Host "  0 = Off, 1 = Evaluation, 2 = On (values per Windows builds may vary)"
Write-Host ""

if ($current -eq 0) {
    Write-Host "SAC already Off in registry. If DLL still blocked, reboot and re-test."
} else {
    New-Item -Path $policyPath -Force | Out-Null
    Set-ItemProperty -Path $policyPath -Name "VerifiedAndReputablePolicyState" -Value 0 -Type DWord
    Write-Host "Set VerifiedAndReputablePolicyState -> 0 (Off)"
}

$citoolCandidates = @(
    "${env:ProgramFiles}\Windows Defender\CiTool.exe",
    "${env:ProgramFiles(x86)}\Windows Defender\CiTool.exe"
)
$citool = $citoolCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if ($citool) {
    Write-Host "Refreshing CI policy via CiTool..."
    & $citool -r
} else {
    Write-Host "CiTool.exe not found — reboot is required for the registry change to apply."
}

Write-Host ""
Write-Host "REBOOT Windows now, then verify with:"
Write-Host '  [System.Reflection.Assembly]::LoadFile("C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\Struggler\Struggler.dll")'
Write-Host ""
Write-Host "If that succeeds without error, launch STS2 -> Play with Mods."
