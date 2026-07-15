#!/usr/bin/env bash
# Upload workshop/ to Steam Workshop (private by default in workshop.json).

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
WORKSHOP_DIR="$REPO_ROOT/workshop"
UPLOADER_VERSION="${STS2_MOD_UPLOADER_VERSION:-v0.0.5-fix}"
UPLOADER_URL="https://github.com/Miooowo/sts2-mod-uploader/releases/download/${UPLOADER_VERSION}/ModUploader-linux-x64.zip"

die() { printf '\033[1;31mERR>\033[0m %s\n' "$*" >&2; exit 1; }
info() { printf '\033[1;34m==>\033[0m %s\n' "$*"; }

[[ -f "$WORKSHOP_DIR/content/Struggler.dll" ]] || die "Run ./scripts/prepare-workshop.sh first."

if [[ "${1:-}" == "--windows" ]]; then
    if ! command -v powershell.exe >/dev/null 2>&1; then
        die "--windows requires WSL + Windows Steam running"
    fi
    info "Uploading via Windows ModUploader (Steam client must be running)..."
    workshop_win="$(wslpath -w "$WORKSHOP_DIR")"
    powershell.exe -NoProfile -Command "
      \$uploader = Join-Path '$((wslpath -w "$REPO_ROOT"))' 'ModUploader.exe'
      if (-not (Test-Path \$uploader)) {
        \$uploader = Join-Path \$env:USERPROFILE 'sts2-mod-uploader/ModUploader.exe'
      }
      if (-not (Test-Path \$uploader)) {
        Write-Error 'Get ModUploader-win-x64.zip from https://github.com/Miooowo/sts2-mod-uploader/releases — extract ModUploader.exe to repo root or %USERPROFILE%\\sts2-mod-uploader\\'
      }
      & \$uploader upload -w '$workshop_win'
    "
    exit $?
fi

if [[ "${1:-}" == "--github" ]]; then
    if ! command -v gh >/dev/null 2>&1; then
        die "gh CLI required for --github"
    fi
    info "Triggering GitHub Actions workshop upload..."
    gh workflow run workshop-publish.yml --repo "${STRUGGLER_GITHUB_REPO:-D3adByte/STS2-TheStruggler}" \
        -f change_note="$(python3 -c "import json; print(json.load(open('$WORKSHOP_DIR/workshop.json'))['changeNote'])")"
    info "Watch: gh run watch"
    exit 0
fi

# Linux ModUploader (needs native Linux Steam — usually NOT WSL)
UPLOADER="$REPO_ROOT/ModUploader"
if [[ ! -x "$UPLOADER" ]]; then
  TMP_DIR="$(mktemp -d)"
  trap 'rm -rf "$TMP_DIR"' EXIT
  info "Downloading STS2 ModUploader ${UPLOADER_VERSION}..."
  curl -fsSL "$UPLOADER_URL" -o "$TMP_DIR/uploader.zip"
  unzip -q "$TMP_DIR/uploader.zip" -d "$TMP_DIR"
  UPLOADER="$TMP_DIR/ModUploader"
fi

info "Uploading workshop workspace (./ModUploader)..."
"$UPLOADER" upload -w "$WORKSHOP_DIR" || {
    cat <<'EOF'

Upload failed. Common fixes:
  1. Steam must be running and logged in on this machine
  2. On WSL, use Windows upload instead:
       ./scripts/publish-workshop.sh --windows
  3. Or use GitHub Actions (set STEAM_USERNAME + STEAM_CONFIG_VDF secrets):
       ./scripts/publish-workshop.sh --github
EOF
    exit 1
}

if [[ -f "$WORKSHOP_DIR/mod_id.txt" ]]; then
    info "Workshop item ID: $(cat "$WORKSHOP_DIR/mod_id.txt")"
    info "Set GitHub repo variable WORKSHOP_PUBLISHED_FILE_ID to this value for CI uploads."
fi

info "Done. Subscribe to the mod in Steam Workshop (private until you flip visibility)."
