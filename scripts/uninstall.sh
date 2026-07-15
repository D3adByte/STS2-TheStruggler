#!/usr/bin/env bash
# Remove Struggler from your STS2 mods folder.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=common.sh
source "$SCRIPT_DIR/common.sh"

FORCE=false

usage() {
    cat <<'EOF'
Usage: scripts/uninstall.sh [options]

Removes mods/Struggler from your STS2 install.

Options:
  -f, --force    Skip confirmation prompt
  -h, --help     Show this help
EOF
}

while [[ $# -gt 0 ]]; do
    case "$1" in
        -f|--force) FORCE=true; shift ;;
        -h|--help) usage; exit 0 ;;
        *) die "Unknown option: $1 (try --help)" ;;
    esac
done

resolve_paths
require_sts2_closed

if [[ ! -d "$MODS_DIR" ]]; then
    info "Nothing to remove — mod folder does not exist:"
    info "  $MODS_DIR"
    exit 0
fi

info "Mod folder: $MODS_DIR"
list_mod_files

if [[ "$FORCE" != true ]]; then
    printf 'Remove this folder? [y/N] '
    read -r reply
    case "$reply" in
        y|Y|yes|YES) ;;
        *) info "Cancelled."; exit 0 ;;
    esac
fi

rm -rf "$MODS_DIR"
info "Removed $MODS_DIR"

cat <<'EOF'

If Struggler was enabled in Mod Settings, the entry may linger until you
launch "Play with Mods" again — harmless, just re-enable after reinstall.
EOF
