#!/usr/bin/env bash
# Install a SignPath-signed Struggler.dll from GitHub Releases (SAC-safe).

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=common.sh
source "$SCRIPT_DIR/common.sh"

REPO="${STRUGGLER_GITHUB_REPO:-D3adByte/STS2-TheStruggler}"
TAG="${1:-latest}"
TMP_DIR=""

cleanup() {
    [[ -n "$TMP_DIR" && -d "$TMP_DIR" ]] && rm -rf "$TMP_DIR"
}
trap cleanup EXIT

usage() {
    cat <<EOF
Usage: scripts/install-signed.sh [tag]

Downloads a SignPath-signed Struggler.dll from GitHub Releases and deploys
to your STS2 mods folder. Safe to use with Smart App Control enabled.

Examples:
  ./scripts/install-signed.sh           # latest release
  ./scripts/install-signed.sh v0.1.1    # specific tag

Requires: gh CLI authenticated (gh auth login)
Setup:    See SIGNPATH.md (apply at https://signpath.org/)
EOF
}

if [[ "${1:-}" == "-h" || "${1:-}" == "--help" ]]; then
    usage
    exit 0
fi

if ! command -v gh >/dev/null 2>&1; then
    die "gh CLI not found. Install: https://cli.github.com/ — then gh auth login"
fi

resolve_paths
require_sts2_closed
require_sts2_install

TMP_DIR="$(mktemp -d)"
cd "$TMP_DIR"

info "Fetching signed release from $REPO (tag: $TAG)..."
if [[ "$TAG" == "latest" ]]; then
    if ! gh release download --repo "$REPO" --pattern "struggler-signed.zip" -D . 2>/dev/null; then
        die "No struggler-signed.zip on latest release. Has SignPath signing run yet? See SIGNPATH.md"
    fi
else
    if ! gh release download "$TAG" --repo "$REPO" --pattern "struggler-signed.zip" -D . 2>/dev/null; then
        die "Release $TAG has no struggler-signed.zip. Pick another tag or run sign-release workflow."
    fi
fi

unzip -o struggler-signed.zip -d signed
[[ -f signed/Struggler.dll ]] || die "Signed zip missing Struggler.dll"

mkdir -p "$MODS_DIR"

info "Deploying signed DLL to $MODS_DIR"
cp signed/Struggler.dll "$MODS_DIR/"

# Keep json/pck/pdb from local publish if present; refresh json from release when bundled
if [[ -f signed/Struggler.json ]]; then
    cp signed/Struggler.json "$MODS_DIR/"
fi

if [[ ! -f "$MODS_DIR/Struggler.pck" ]]; then
    warn "No Struggler.pck in mods folder — run ./scripts/install.sh once (publish) for assets, then re-run install-signed for the DLL."
    warn "Or copy .pck from a local publish; SAC only blocks the DLL, not the .pck."
fi

list_mod_files

if command -v powershell.exe >/dev/null 2>&1; then
    info "Verifying Windows can load signed DLL..."
    bash "$SCRIPT_DIR/test-dll-load.sh" || die "Signed DLL still blocked — signing may not be complete yet."
fi

cat <<'EOF'

Signed Struggler.dll installed. SAC can stay On.
Launch STS2 via Play with Mods → enable Struggler + BaseLib → restart once.
EOF
