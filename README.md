# Idle Auto Defense Game

This Unity project is a clean consumer of the Deucarian Idle Auto Defense template package. The playable project-owned starter game lives under `Assets/IdleAutoDefense`, while reusable gameplay systems stay in Deucarian packages resolved through `Packages/manifest.json`.

## Quick Start

1. Open the project in Unity `6000.3.5f1`.
2. Open `Assets/IdleAutoDefense/Scenes/BasicIdleAutoDefense.unity`.
3. Press Play.
4. Confirm the starter loop spawns enemies, fires the direct and projectile weapons, drafts upgrades, and writes/reset the sample save.

## Project Layout

- `Assets/IdleAutoDefense/Scenes` contains the neutral playable starter scene.
- `Assets/IdleAutoDefense/Scripts` contains the thin project bootstrap and sample save helper.
- `Assets/IdleAutoDefense/Content` contains the imported template content pack, content set, authored assets, default tuning data, progression data, and mock monetization placeholders.
- `Assets/IdleAutoDefense/Prefabs`, `Audio`, and `Visuals` contain placeholder presentation assets copied from the template sample.
- `Assets/IdleAutoDefense/Tests` contains EditMode and PlayMode smoke coverage for the imported starter game.

## Customization Rules

Start by editing project-owned content, prefabs, scene composition, and balance data under `Assets/IdleAutoDefense`. Keep reusable runtime behavior, package source, gameplay frameworks, and integration abstractions in Deucarian packages.

Use `Tools > Deucarian > Game Content Authoring` to inspect and validate the imported content pack/set before making content changes.
