# Contributing to The Struggler

Thank you for helping improve this mod! We are building toward a **Steam Workshop** release and welcome contributions of all kinds.

## What we need most

1. **Art** — Card portraits, power icons, relic art, and character UI were AI-generated placeholders. Hand-drawn, pixel, or commissioned art is warmly accepted.
2. **Balance & bugs** — Playtest reports, tuning PRs, and crash fixes.
3. **Code** — New cards/relics, cleaner mechanics, compatibility with BaseLib updates.
4. **Docs & localization** — README improvements, other languages under `Struggler/localization/`.

## Before you start

- Read the [AI & fan-work disclaimer](README.md#ai--fan-work-disclaimer) in the README.
- You need a **legal copy** of Slay the Spire 2. Never commit `sts2.dll`, `0Harmony.dll`, or other game files.
- Install [BaseLib](https://steamcommunity.com/workshop/filedetails/?id=3737335127) for local testing.

## Development setup

### 1. Clone and configure paths

```bash
git clone git@github.com:D3adByte/STS2-TheStruggler.git
cd STS2-TheStruggler
cp Directory.Build.props.example Directory.Build.props
```

Edit `Directory.Build.props` with your paths:

| Property | Example (WSL) |
|----------|----------------|
| `Sts2Path` | `/mnt/c/Program Files (x86)/Steam/steamapps/common/Slay the Spire 2` |
| `Sts2DataDir` | `$(Sts2Path)/data_sts2_windows_x86_64` |
| `ModsPath` | `$(Sts2Path)/mods/` |
| `GodotPath` | Path to MegaDot / Godot 4.5.1 console binary |

`Directory.Build.props` is gitignored — your local Steam path stays on your machine.

### 2. Build

```bash
dotnet build Struggler.csproj -c Release
```

Output is copied to your STS2 `mods/Struggler/` folder automatically.

### 3. Publish (assets + .pck)

When changing images, shaders, or localization:

```bash
dotnet publish Struggler.csproj -c Release
```

Requires a valid `GodotPath` in `Directory.Build.props`.

### 4. Test in-game

1. Launch STS2 via **Play with Mods**
2. Enable **Struggler** and **BaseLib** in Mod Settings
3. Restart once
4. Select **The Struggler** on character select

## Submitting changes

### Pull requests

1. Fork the repo and create a branch from `main`
2. Keep PRs focused (one feature or fix per PR when possible)
3. CI must pass — see [.github/workflows/ci.yml](.github/workflows/ci.yml)
4. Describe **what** changed and **why**
5. For art replacements, note the license/source (must be compatible with MIT or your own work)

### Commit messages

Use clear, imperative summaries:

```
Add Shoulder Check VFX hook
Replace AI art for Dragonslayer card
Fix ammo not resetting between combats
```

### Art contribution guidelines

- Match existing dimensions where `.import` files already exist (check `Struggler/images/`)
- PNG with transparency for cards/powers/relics
- Prefer original or properly licensed work — do not submit copyrighted art you do not have rights to
- In PR description, state whether art is **your original work** or **AI-assisted** so we can track credits

## CI (what runs on every PR)

| Job | Purpose |
|-----|---------|
| **validate** | JSON manifest & localization syntax, project structure |
| **security** | Secret scanning (gitleaks), dependency review |
| **build** | Compiles when STS2 reference DLLs are available (see below) |

Full compilation requires `sts2.dll` and `0Harmony.dll` from your game install. GitHub-hosted runners do not include the game, so the **build job skips gracefully** unless you set up [CI game deps](#optional-ci-build-with-game-dlls).

### Optional: CI build with game DLLs

For maintainers who want green compile checks on GitHub:

1. Create `ci/game-deps/` locally (gitignored)
2. Copy `sts2.dll` and `0Harmony.dll` from your `data_sts2_*` folder
3. Set repository variable `ENABLE_STS2_CI_BUILD=true` in GitHub **Settings → Secrets and variables → Actions → Variables**

Never push game DLLs to the public repo.

## Reporting issues

Use [GitHub Issues](https://github.com/D3adByte/STS2-TheStruggler/issues):

- **Bug reports**: STS2 version, BaseLib version, steps to reproduce, log snippet if possible
- **Feature requests**: Card/relic ideas, balance suggestions
- **Art requests**: Which asset needs a community replacement

## Code of conduct

Be respectful. This is a fan project maintained in spare time. Constructive feedback beats drive-by negativity.

## Questions?

Open a [Discussion](https://github.com/D3adByte/STS2-TheStruggler/discussions) or an Issue — we're happy to help you get set up.
