# SignPath Foundation — trusted DLL signing (SAC-safe)

Smart App Control blocks **unsigned** `Struggler.dll`. The long-term fix is **Authenticode signing** from a Microsoft-trusted CA. Disabling SAC is not required.

This repo is set up for **[SignPath Foundation](https://signpath.org/)** — **free** code signing for open-source projects.

## Step 1: Apply (one time, ~1 week)

1. Go to **https://signpath.org/** → **Apply**
2. Submit:
   - **Repository:** `https://github.com/D3adByte/STS2-TheStruggler`
   - **License:** MIT
   - **Description:** Unofficial Guts character mod for Slay the Spire 2 (requires BaseLib)
   - **Why signing:** Windows Smart App Control blocks unsigned mod DLLs (`HRESULT 0x800711C7`); users need a trusted signed build
3. Wait for approval email (~1 week)

## Step 2: Configure SignPath dashboard

After approval, create a project matching `.signpath/policies/struggler.policy`:

| Setting | Value |
|---------|-------|
| Project slug | `sts2-the-struggler` |
| Signing policy slug | `release-signing` |
| Artifact configuration slug | `struggler-mod-dll` |
| Trusted build system | GitHub |
| Repository | `D3adByte/STS2-TheStruggler` |
| Workflow | `.github/workflows/sign-release.yml` |
| Branch/tag pattern | `refs/tags/v*` (and/or `refs/heads/main` for dev signed builds) |
| Approval mode | Automatic (OSS tier) |

Install the **SignPath GitHub App** on the repo when prompted.

## Step 3: Add GitHub secrets

Repository → **Settings** → **Secrets and variables** → **Actions**:

| Secret | Source |
|--------|--------|
| `SIGNPATH_API_TOKEN` | SignPath dashboard → API tokens |
| `SIGNPATH_ORG_ID` | SignPath organization UUID |

Optional (only if you enable full CI builds on GitHub):

| Variable | Value |
|----------|-------|
| `ENABLE_STS2_CI_BUILD` | `true` |
| Plus private `ci/game-deps/` with `sts2.dll` + `0Harmony.dll` on a self-hosted runner or private fork workflow |

## Step 4: Get a signed DLL

### Option A — GitHub Release (recommended)

```bash
git tag v0.1.1
git push origin v0.1.1
```

Workflow `.github/workflows/sign-release.yml` builds, signs via SignPath, and attaches **`Struggler-signed.zip`** to the GitHub Release.

### Option B — Manual workflow

Actions → **Sign release** → **Run workflow** (after secrets are set).

## Step 5: Install signed build locally

```bash
./scripts/install-signed.sh          # latest signed release
./scripts/install-signed.sh v0.1.1   # specific tag
```

Then launch STS2 → **Play with Mods**. SAC stays **On**.

Verify Windows accepts the DLL:

```bash
./scripts/test-dll-load.sh
# expect: RESULT: OK
```

## Your dev loop (after SignPath is live)

| Task | Command |
|------|---------|
| Code changes | edit in WSL as usual |
| Signed playable build | push tag or run sign workflow |
| Install to Windows STS2 | `./scripts/install-signed.sh` |
| SAC check | `./scripts/test-dll-load.sh` |

Local `dotnet publish` still produces **unsigned** DLLs — fine for compile checks, but **won't load on SAC Windows**. Use signed releases for playtesting.

## If you need signing before SignPath approves

- **Certum Code Signing in the Cloud** (~$80/yr) — cheapest paid trusted cert
- Set `STRUGGLER_SIGN_PFX` + run `./scripts/install.sh --sign`

## Links

- SignPath apply: https://signpath.org/
- Policy file: `.signpath/policies/struggler.policy`
- Workflow: `.github/workflows/sign-release.yml`
- SignPath GitHub Action: https://github.com/signpath/github-action-submit-signing-request
