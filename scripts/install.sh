#!/usr/bin/env bash
# Build + publish Struggler and deploy to your STS2 mods folder.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=common.sh
source "$SCRIPT_DIR/common.sh"

CONFIGURATION="${CONFIGURATION:-Release}"
SKIP_PUBLISH=false
SIGN_DLL=false
USE_LINUX=false

usage() {
    cat <<'EOF'
Usage: scripts/install.sh [options]

Builds The Struggler mod and copies output to your STS2 mods folder.

Options:
  --build-only   dotnet build only (no .pck export; faster for code-only changes)
  --debug        Use Debug configuration instead of Release
  --linux        Install to Linux Steam STS2 (no Smart App Control)
  --sign         Authenticode-sign Struggler.dll after build (needs cert env vars)
  -h, --help     Show this help

Environment:
  CONFIGURATION         Release (default) or Debug
  STRUGGLER_SIGN_CERT   Certificate thumbprint in Windows cert store
  STRUGGLER_SIGN_PFX    Path to .pfx file (optional: STRUGGLER_SIGN_PASSWORD)

After install:
  Launch STS2 via "Play with Mods", enable Struggler + BaseLib, restart once.
EOF
}

while [[ $# -gt 0 ]]; do
    case "$1" in
        --build-only) SKIP_PUBLISH=true; shift ;;
        --debug) CONFIGURATION=Debug; shift ;;
        --linux) USE_LINUX=true; shift ;;
        --sign) SIGN_DLL=true; shift ;;
        -h|--help) usage; exit 0 ;;
        *) die "Unknown option: $1 (try --help)" ;;
    esac
done

cd "$REPO_ROOT"
if [[ "$USE_LINUX" == true ]]; then
    resolve_linux_paths
else
    resolve_paths
fi
require_build_props
require_sts2_closed
require_sts2_install

info "Repo:      $REPO_ROOT"
info "STS2:      $STS2_PATH"
info "Mods dir:  $MODS_DIR"
info "Target:    $INSTALL_TARGET"
info "Config:    $CONFIGURATION"

if [[ "$SIGN_DLL" == true && -z "${STRUGGLER_SIGN_CERT:-}" && -z "${STRUGGLER_SIGN_PFX:-}" ]]; then
    die "--sign requires STRUGGLER_SIGN_CERT or STRUGGLER_SIGN_PFX. See scripts/sign-mod.ps1"
fi

if [[ "$SKIP_PUBLISH" == true ]]; then
    info "Building (no .pck export)..."
    dotnet build Struggler.csproj -c "$CONFIGURATION" --nologo -v minimal
else
    if [[ -z "$GODOT_PATH" || ! -e "$GODOT_PATH" ]]; then
        die "GodotPath missing or invalid in Directory.Build.props (needed for publish + .pck)."
    fi
    info "Publishing (dll + json + pdb + .pck)..."
    dotnet publish Struggler.csproj -c "$CONFIGURATION" --nologo -v minimal
fi

mkdir -p "$MODS_DIR"

if [[ ! -f "$MODS_DIR/Struggler.dll" ]]; then
    die "Install finished but Struggler.dll is missing in $MODS_DIR"
fi

if [[ "$SIGN_DLL" == true ]]; then
    sign_windows_mod_dll
fi

info "Installed to: $MODS_DIR"
list_mod_files
unblock_windows_mod_files
warn_if_sac_enabled

cat <<'EOF'

Next steps:
  1. Open STS2 with "Play with Mods"
  2. Enable Struggler and BaseLib in Mod Settings
  3. Restart the game once, then pick The Struggler
EOF
