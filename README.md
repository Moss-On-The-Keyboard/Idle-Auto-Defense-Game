# Moss Idle Auto Defense

This is the first Moss game project bootstrapped from the Deucarian Idle Auto Defense template.

The game is a normal Unity project, not a reusable UPM package. Deucarian packages are consumed through Git registry URLs in `Packages/manifest.json`, while game-specific content lives under `Assets/Moss/IdleAutoDefense`.

## Quick Start

1. Open the project with Unity `6000.3.5f1`.
2. Open `Assets/Moss/IdleAutoDefense/Scenes/MossIdleAutoDefense.unity`.
3. Press Play.
4. Open `Tools > Deucarian > Game Content Authoring` and inspect the Content Pack / Content Library providers.
5. Use `Tools > Deucarian > Moss Idle Auto Defense > Reset Dev Save` to clear the local smoke-save file.

## Project Layout

- `Assets/Moss/IdleAutoDefense/Scripts` contains Moss-owned runtime bootstrap code.
- `Assets/Moss/IdleAutoDefense/Scenes` contains the current smoke scene.
- `Assets/GameContent/MossOnTheKeyboard` contains the authored starter content pack, content set, attacks, enemies, waves, weapons, upgrades, placeholder visual prefabs, banner texture, and short preview audio clips.
- `Assets/Moss/IdleAutoDefense/Content` contains copied starter tuning data from the template sample.
- `Assets/Moss/IdleAutoDefense/Content/Overrides` contains product-owned placeholder override files for the next content pass.
- `Assets/Moss/IdleAutoDefense/Docs` documents the template flow contract and Moss override path.
- `Assets/Moss/IdleAutoDefense/Tests` contains EditMode and PlayMode smoke tests.

## Current Scope

This bootstrap intentionally avoids polished art, real monetization, store builds, and ECS. The authored starter pack proves the first concrete asset-flip path with names, balance, placeholder visuals, and optional audio references while keeping reusable gameplay behavior in Deucarian packages.
