#!/usr/bin/env bash
# Quick test: can Windows load Struggler.dll, or is Application Control blocking it?

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=common.sh
source "$SCRIPT_DIR/common.sh"

resolve_paths

if ! command -v powershell.exe >/dev/null 2>&1; then
    die "powershell.exe required (run from WSL on Windows)"
fi

mods_dir_win="$(wslpath -w "$MODS_DIR")"

powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "
\$dll = Join-Path '$mods_dir_win' 'Struggler.dll'
Write-Host 'DLL:' \$dll
\$reg = (Get-ItemProperty 'HKLM:\SYSTEM\CurrentControlSet\Control\CI\Policy' -Name VerifiedAndReputablePolicyState -EA SilentlyContinue).VerifiedAndReputablePolicyState
Write-Host 'SAC registry VerifiedAndReputablePolicyState:' \$reg
try { Write-Host 'Defender SAC state:' (Get-MpComputerStatus).SmartAppControlState } catch {}
Write-Host 'Signature:' (Get-AuthenticodeSignature \$dll).Status
try {
  [void][System.Reflection.Assembly]::LoadFile(\$dll)
  Write-Host 'RESULT: OK — Windows will let STS2 load this DLL'
  exit 0
} catch {
  Write-Host 'RESULT: BLOCKED — STS2 will show FileLoadException'
  Write-Host \$_.Exception.Message
  exit 1
}
"
