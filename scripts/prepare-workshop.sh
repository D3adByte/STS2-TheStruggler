#!/usr/bin/env bash
# Build the mod locally and stage files for Steam Workshop upload.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=common.sh
source "$SCRIPT_DIR/common.sh"

CHANGE_NOTE="${1:-Dev build}"

cd "$REPO_ROOT"
resolve_paths
require_build_props
require_sts2_closed
require_sts2_install

WORKSHOP_DIR="$REPO_ROOT/workshop"
CONTENT_DIR="$WORKSHOP_DIR/content"

info "Publishing mod to staging area..."
if [[ -z "${GODOT_PATH:-}" || ! -e "${GODOT_PATH:-}" ]]; then
    die "GodotPath missing in Directory.Build.props (needed for .pck)."
fi
dotnet publish Struggler.csproj -c Release --nologo -v minimal

info "Staging Workshop content..."
rm -rf "$CONTENT_DIR"
mkdir -p "$CONTENT_DIR"
cp "$MODS_DIR/Struggler.dll" "$CONTENT_DIR/"
cp "$MODS_DIR/Struggler.json" "$CONTENT_DIR/"
cp "$MODS_DIR/Struggler.pck" "$CONTENT_DIR/"
cp "$REPO_ROOT/Struggler/mod_image.png" "$WORKSHOP_DIR/image.png"

# Update change note for next upload
python3 - <<PY
import json
from pathlib import Path
path = Path("$WORKSHOP_DIR/workshop.json")
data = json.loads(path.read_text())
data["changeNote"] = """$CHANGE_NOTE"""
path.write_text(json.dumps(data, indent=2) + "\n")
PY

info "Workshop package ready:"
ls -la "$CONTENT_DIR"

cat <<EOF

Next: upload to Steam Workshop
  ./scripts/publish-workshop.sh

After upload: subscribe in Steam, remove local mods/Struggler/, launch Play with Mods.
EOF
