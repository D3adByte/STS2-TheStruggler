#!/usr/bin/env bash
# Shared helpers for Struggler install/uninstall scripts.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
MOD_NAME="Struggler"
BUILD_PROPS="$REPO_ROOT/Directory.Build.props"

info() { printf '\033[1;34m==>\033[0m %s\n' "$*"; }
warn() { printf '\033[1;33m!!>\033[0m %s\n' "$*" >&2; }
die()  { printf '\033[1;31mERR>\033[0m %s\n' "$*" >&2; exit 1; }

read_props_value() {
    local key="$1"
    local file="$2"
    [[ -f "$file" ]] || return 1

    local value
    value="$(sed -n "s|.*<${key}>\\(.*\\)</${key}>.*|\\1|p" "$file" | head -n1 | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')"
    [[ -n "$value" ]] || return 1
    printf '%s' "$value"
}

default_sts2_path() {
    local candidates=(
        "/mnt/c/Program Files (x86)/Steam/steamapps/common/Slay the Spire 2"
        "${HOME}/.local/share/Steam/steamapps/common/Slay the Spire 2"
        "${HOME}/Library/Application Support/Steam/steamapps/common/Slay the Spire 2"
    )
    local path
    for path in "${candidates[@]}"; do
        if [[ -d "$path" ]]; then
            printf '%s' "$path"
            return 0
        fi
    done
    return 1
}

resolve_paths() {
    local sts2_path=""
    local mods_path=""
    local godot_path=""

    if [[ -f "$BUILD_PROPS" ]]; then
        sts2_path="$(read_props_value Sts2Path "$BUILD_PROPS" || true)"
        mods_path="$(read_props_value ModsPath "$BUILD_PROPS" || true)"
        godot_path="$(read_props_value GodotPath "$BUILD_PROPS" || true)"
    fi

    if [[ -z "$sts2_path" ]]; then
        sts2_path="$(default_sts2_path || true)"
    fi

    if [[ -n "$mods_path" ]]; then
        mods_path="${mods_path//\$\(Sts2Path\)/$sts2_path}"
    elif [[ -n "$sts2_path" ]]; then
        mods_path="${sts2_path}/mods/"
    fi

    [[ -n "$sts2_path" ]] || die "Could not find STS2. Copy Directory.Build.props.example to Directory.Build.props and set Sts2Path."
    [[ -n "$mods_path" ]] || die "Could not resolve mods folder. Set ModsPath in Directory.Build.props."

    MODS_DIR="${mods_path%/}/${MOD_NAME}"
    STS2_PATH="$sts2_path"
    STS2_DATA_DIR="${sts2_path}/data_sts2_windows_x86_64"
    if [[ ! -d "$STS2_DATA_DIR" ]]; then
        STS2_DATA_DIR="${sts2_path}/data_sts2_linuxbsd_x86_64"
    fi
    if [[ ! -d "$STS2_DATA_DIR" ]]; then
        STS2_DATA_DIR="${sts2_path}/SlayTheSpire2.app/Contents/Resources/data_sts2_macos_x86_64"
    fi
    GODOT_PATH="${godot_path:-}"
    INSTALL_TARGET="windows"
}

resolve_linux_paths() {
    local linux_sts2="${HOME}/.local/share/Steam/steamapps/common/Slay the Spire 2"
    [[ -d "$linux_sts2" ]] || die "Linux STS2 not found at: $linux_sts2 — install STS2 in Linux Steam (no Smart App Control)."

    STS2_PATH="$linux_sts2"
    MODS_DIR="${linux_sts2}/mods/${MOD_NAME}"
    STS2_DATA_DIR="${linux_sts2}/data_sts2_linuxbsd_x86_64"
    [[ -d "$STS2_DATA_DIR" ]] || die "Linux STS2 data not found at: $STS2_DATA_DIR"

    if [[ -f "$BUILD_PROPS" ]]; then
        GODOT_PATH="$(read_props_value GodotPath "$BUILD_PROPS" || true)"
    else
        GODOT_PATH=""
    fi
    INSTALL_TARGET="linux"
}

sts2_is_running() {
    if command -v tasklist.exe >/dev/null 2>&1; then
        tasklist.exe 2>/dev/null | grep -qi 'SlayTheSpire2' && return 0
    fi
    if command -v pgrep >/dev/null 2>&1; then
        pgrep -fi 'SlayTheSpire2' >/dev/null 2>&1 && return 0
    fi
    return 1
}

require_sts2_closed() {
    if sts2_is_running; then
        die "Slay the Spire 2 looks like it is running. Close the game first so DLLs are not locked."
    fi
}

require_build_props() {
    if [[ ! -f "$BUILD_PROPS" ]]; then
        die "Missing Directory.Build.props. Run: cp Directory.Build.props.example Directory.Build.props"
    fi
}

require_sts2_install() {
    [[ -d "$STS2_PATH" ]] || die "STS2 install not found at: $STS2_PATH"
    [[ -d "$STS2_DATA_DIR" ]] || die "STS2 data folder not found at: $STS2_DATA_DIR"
}

list_mod_files() {
    if [[ -d "$MODS_DIR" ]]; then
        find "$MODS_DIR" -maxdepth 1 -type f -printf '  %f\n' 2>/dev/null \
            || find "$MODS_DIR" -maxdepth 1 -type f | sed 's|^|  |'
    fi
}

unblock_windows_mod_files() {
    if ! command -v powershell.exe >/dev/null 2>&1; then
        return 0
    fi
    if ! command -v wslpath >/dev/null 2>&1; then
        return 0
    fi

    local mods_dir_win ps1_win
    mods_dir_win="$(wslpath -w "$MODS_DIR")"
    ps1_win="$(wslpath -w "$SCRIPT_DIR/unblock-mod.ps1")"

    info "Clearing Windows Mark-of-the-Web on mod files (if any)..."
    powershell.exe -NoProfile -ExecutionPolicy Bypass \
        -File "$ps1_win" -ModsDir "$mods_dir_win" || warn "Unblock step failed (non-fatal)."
}

sign_windows_mod_dll() {
    if [[ "${INSTALL_TARGET:-windows}" != "windows" ]]; then
        return 0
    fi
    if ! command -v powershell.exe >/dev/null 2>&1 || ! command -v wslpath >/dev/null 2>&1; then
        return 0
    fi
    if [[ -z "${STRUGGLER_SIGN_CERT:-}" && -z "${STRUGGLER_SIGN_PFX:-}" ]]; then
        return 0
    fi

    local mods_dir_win ps1_win
    mods_dir_win="$(wslpath -w "$MODS_DIR")"
    ps1_win="$(wslpath -w "$SCRIPT_DIR/sign-mod.ps1")"

    info "Authenticode-signing Struggler.dll..."
    powershell.exe -NoProfile -ExecutionPolicy Bypass \
        -File "$ps1_win" -ModsDir "$mods_dir_win" || die "DLL signing failed."
}

warn_if_sac_enabled() {
    if [[ "${INSTALL_TARGET:-windows}" != "windows" ]]; then
        return 0
    fi
    if ! command -v powershell.exe >/dev/null 2>&1; then
        return 0
    fi
    if [[ -n "${STRUGGLER_SIGN_CERT:-}" || -n "${STRUGGLER_SIGN_PFX:-}" ]]; then
        return 0
    fi

    local ps1_win
    ps1_win="$(wslpath -w "$SCRIPT_DIR/check-sac.ps1")"
    if ! powershell.exe -NoProfile -ExecutionPolicy Bypass -File "$ps1_win" 2>/dev/null; then
        warn "Unsigned DLL — SAC will block local builds (HRESULT 0x800711C7)."
        warn "Long-term fix: apply at https://signpath.org/ then ./scripts/install-signed.sh"
        warn "See SIGNPATH.md"
    fi
}
