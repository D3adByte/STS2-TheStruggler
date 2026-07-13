# The Struggler — Guts-inspired STS2 Character Mod

[![CI](https://github.com/D3adByte/STS2-TheStruggler/actions/workflows/ci.yml/badge.svg)](https://github.com/D3adByte/STS2-TheStruggler/actions/workflows/ci.yml)

Playable **Guts-inspired** bruiser character mod for [Slay the Spire 2](https://store.steampowered.com/app/2868840/Slay_the_Spire_2/).

> **Work in progress** — we plan to publish on the **Steam Workshop**. Community help is very welcome!

## Community & contributions

This project was bootstrapped with **AI assistance** — including the **code, card design, and artwork**. It is a passion fan mod, not a polished commercial release. If you enjoy Berserk or STS2 modding and want to help, **please jump in**:

- Replace or improve **AI-generated card art**, character portraits, and relic icons
- Balance tuning, new cards/relics, bug fixes, localization
- Documentation, testing, and Workshop packaging

See **[CONTRIBUTING.md](CONTRIBUTING.md)** for setup, PR guidelines, and art contribution notes.

**You do not need permission** to open issues or PRs. All meaningful contributions get credit in release notes.

## AI & fan-work disclaimer

- **Code and visuals** in this repo were largely produced with generative AI tools and iterative human direction.
- This is an **unofficial fan mod**. Not affiliated with MegaCrit, Kentaro Miura, or any rights holders.
- *Berserk* names and themes are used as homage. Replace art or naming if you prefer a more original presentation.
- **You must own Slay the Spire 2** to build or play this mod. Do not redistribute game binaries (`sts2.dll`, etc.).

## Requirements

- Slay the Spire 2 (v0.107.0+)
- [BaseLib](https://steamcommunity.com/workshop/filedetails/?id=3737335127) (Steam Workshop)
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- MegaDot 4.5.1 (for full publish with `.pck`)

## Quick start (developers)

1. Clone the repo and copy local paths:
   ```bash
   cp Directory.Build.props.example Directory.Build.props
   # Edit Directory.Build.props with your STS2 + MegaDot paths
   ```
2. Build:
   ```bash
   dotnet build Struggler.csproj -c Release
   ```
3. In-game: **Play with Mods** → enable **Struggler** + **BaseLib** → restart → select **The Struggler**.

See [CONTRIBUTING.md](CONTRIBUTING.md) for WSL/Windows paths, publish steps, and CI notes.

## Core mechanics

| Mechanic | Description |
|----------|-------------|
| **Struggle** | Gain stacks when taking damage; every 3 → +1 Strength |
| **Ammo** | 3 per combat; fuels Cannon Shot / Crossbow / Recoil |
| **Berserk** | Rare power: +damage, +Strength, self-damage each turn |
| **Brand of Sacrifice** | Starter relic: struggle on hit, draw when low HP |

## Content

- **32 cards** (strikes, cannon kit, berserk powers, exhaust finishers)
- **6 relics** (Brand, Rickert's Gift, Godot's Steel, etc.)
- **3 potions**

## Project layout

```
StrugglerCode/
  Character/Guts.cs      # Character model
  Cards/                 # Full card pool
  Powers/                # Struggle, Berserk, Hatred, etc.
  Relics/                # Character relics
  Ammo/AmmoResource.cs   # Combat ammo counter
Struggler/
  images/                # Card art, UI (AI-generated — PRs welcome!)
  localization/eng/      # English strings
```

## Roadmap

- [ ] Steam Workshop release
- [ ] Community art pass (replace AI placeholders)
- [ ] Balance feedback from playtesting
- [ ] Additional localizations

## License

[MIT](LICENSE) — see file for details. Fan content and third-party game assets remain subject to their respective owners.
