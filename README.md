# Moss Idle Auto Defense

This is the first Moss game project bootstrapped from the Deucarian Idle Auto Defense template.

The game is a normal Unity project, not a reusable UPM package. Deucarian packages are consumed through Git registry URLs in `Packages/manifest.json`, while game-specific content lives under `Assets/Moss/IdleAutoDefense`.

## Quick Start

1. Open the project with Unity `6000.3.5f1`.
2. Open `Assets/Moss/IdleAutoDefense/Scenes/MossIdleAutoDefense.unity`.
3. Press Play.
4. Use `Tools > Moss > Idle Auto Defense > Reset Dev Save` to clear the local smoke-save file.

## Project Layout

- `Assets/Moss/IdleAutoDefense/Scripts` contains Moss-owned runtime bootstrap code.
- `Assets/Moss/IdleAutoDefense/Scenes` contains the current smoke scene.
- `Assets/Moss/IdleAutoDefense/Content` contains copied starter tuning data from the template sample.
- `Assets/Moss/IdleAutoDefense/Content/Overrides` contains product-owned placeholder override files for the next content pass.
- `Assets/Moss/IdleAutoDefense/Docs` documents the template flow contract and Moss override path.
- `Assets/Moss/IdleAutoDefense/Tests` contains EditMode and PlayMode smoke tests.

## Current Scope

This bootstrap intentionally avoids polished art, audio, monetization, store builds, and ECS. The next pass should add real Moss gameplay content on top of the validated Deucarian package stack.
