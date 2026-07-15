# Steam Workshop dev loop (SAC-safe)

Ship **The Struggler** via Steam Workshop so players (and you) load the mod from Workshop — same as BaseLib and IntoTheSpireverse. **No signing, no SAC off.**

## Your dev loop

```
edit code (WSL)
    ↓
./scripts/prepare-workshop.sh "what changed"
    ↓
./scripts/publish-workshop.sh --windows   (or --github)
    ↓
Steam updates Workshop item (private)
    ↓
Subscribe in Steam → remove local mods/Struggler/
    ↓
Play with Mods → test
```

## One-time setup

### 1. First Workshop upload (Windows)

1. Download [STS2 Mod Uploader](https://github.com/Miooowo/sts2-mod-uploader/releases) → extract to `%USERPROFILE%\sts2-mod-uploader\`
2. **Steam must be running** and logged in
3. Edit `workshop/workshop.json` if needed (starts as **private**)
4. Run:
   ```bash
   ./scripts/prepare-workshop.sh "Initial upload"
   ./scripts/publish-workshop.sh --windows
   ```
5. Note the **Workshop item ID** in `workshop/mod_id.txt` (gitignored)
6. In Steam: subscribe to your own mod
7. **Delete** `mods/Struggler/` locally — use Workshop copy only (SAC-safe)

### 2. GitHub Actions (optional automation)

Secrets (Settings → Actions):

| Secret | How |
|--------|-----|
| `STEAM_USERNAME` | Your Steam login |
| `STEAM_CONFIG_VDF` | From `steamcmd +login` MFA flow — see [steam-workshop-deploy docs](https://github.com/m00nl1ght-dev/steam-workshop-deploy) |

Variable:

| Variable | Value |
|----------|-------|
| `WORKSHOP_PUBLISHED_FILE_ID` | From `workshop/mod_id.txt` after first upload |

CI also needs `ci/game-deps/sts2.dll` + `0Harmony.dll` on the runner (self-hosted or private fork). Until then, use **local prepare + publish**.

Trigger CI upload:

```bash
./scripts/prepare-workshop.sh "balance tweak"
./scripts/publish-workshop.sh --github
```

## Daily commands

| Command | What |
|---------|------|
| `./scripts/prepare-workshop.sh "changelog"` | Build + stage `workshop/content/` |
| `./scripts/publish-workshop.sh --windows` | Upload via Windows Steam |
| `./scripts/publish-workshop.sh --github` | Trigger `.github/workflows/workshop-publish.yml` |

## Why this fixes SAC

Workshop DLLs load from `steamapps/workshop/content/2868840/...` — Microsoft trusts that path via **cloud reputation**, even when unsigned. Local `mods/Struggler/` copies are treated as unknown files and get `0x800711C7`.

## Workshop metadata

- **App ID:** `2868840` (Slay the Spire 2)
- **BaseLib dependency:** `3737335127` (in `workshop.json`)
- **Preview image:** `workshop/image.png`
- **Content:** `Struggler.dll`, `Struggler.json`, `Struggler.pck`

Flip `visibility` to `public` in `workshop.json` when ready for release.

## What not to use for playtesting on SAC Windows

- `./scripts/install.sh` → drops unsigned DLL into `mods/` → blocked
- Keeping both Workshop + local copy — local may take precedence or confuse loads; prefer Workshop only
