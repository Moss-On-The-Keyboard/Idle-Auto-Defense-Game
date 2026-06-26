# Moss Idle Auto Defense

This is the first Moss game project bootstrapped from the Deucarian Idle Auto Defense template.

The game is a normal Unity project, not a reusable UPM package. Deucarian packages are consumed through Git registry URLs in `Packages/manifest.json`, while game-specific content lives under `Assets/Moss/IdleAutoDefense`.

## Quick Start

1. Open the project with Unity `6000.3.5f1`.
2. Open `Assets/Moss/IdleAutoDefense/Scenes/MossIdleAutoDefense.unity`.
3. Press Play.
4. Watch the first loop: Dust Mites arrive first, Sticky Crumbs absorb a few hits, and Cable Beetles appear during the second wave as the pressure check.
5. Open `Tools > Deucarian > Game Content Authoring` and inspect the Content Pack / Content Library providers.
6. In `Content Library`, validate `Moss On The Keyboard Starter Pack` and confirm it has zero warnings.
7. Use `Tools > Deucarian > Moss Idle Auto Defense > Reset Dev Save` to clear the local smoke-save file.

The current starter loop is a short authored-content proof, not a full campaign. It should run for a compact session, show enemies spawning and waves escalating, select a few upgrades, and finish without falling back to template sample content.

## Project Layout

- `Assets/Moss/IdleAutoDefense/Scripts` contains Moss-owned runtime bootstrap code.
- `Assets/Moss/IdleAutoDefense/Scenes` contains the current smoke scene.
- `Assets/GameContent/MossOnTheKeyboard` contains the authored starter content pack, content set, attacks, enemies, waves, weapons, upgrades, placeholder visual prefabs, banner texture, and short preview audio clips.
- `Assets/Moss/IdleAutoDefense/Content` contains copied starter tuning data from the template sample.
- `Assets/Moss/IdleAutoDefense/Content/Overrides` contains product-owned placeholder override files for the next content pass.
- `Assets/Moss/IdleAutoDefense/Docs` documents the template flow contract and Moss override path.
- `Assets/Moss/IdleAutoDefense/Tests` contains EditMode and PlayMode smoke tests.

## Editing The Authored Game

Use `Tools > Deucarian > Game Content Authoring` for authored content work. The active starter recipe is `Assets/GameContent/MossOnTheKeyboard/ContentPacks/contentpack.moss-on-the-keyboard.starter/contentpack.moss-on-the-keyboard.starter_ContentPack.asset`, which points at `Moss Starter Run`.

To make a small asset-flip change, edit one asset under:

- `Assets/GameContent/MossOnTheKeyboard/Attacks`
- `Assets/GameContent/MossOnTheKeyboard/Enemies`
- `Assets/GameContent/MossOnTheKeyboard/Waves`
- `Assets/GameContent/MossOnTheKeyboard/Weapons`
- `Assets/GameContent/MossOnTheKeyboard/Upgrades`

Then return to `Content Library`, run validation, and press Play in `MossIdleAutoDefense.unity`. The scene should report that it is using the assigned Moss content pack and content set with no fallback warnings.

## Starter Loop Tuning

- `Desk Dust` is the teachable opener: a small Dust Mite trickle followed by two Sticky Crumbs.
- `Keyboard Bloom` starts earlier and ramps harder with more dust pressure, four Sticky Crumbs, and two Cable Beetles.
- `Keycap Spore Wand` is cheaper and snappier as the opening weapon.
- `Cursor Beam` is the mid-run precision answer for fast enemies.
- `Moss Seeker Sprout` is the late starter pressure tool for Cable Beetles and stragglers.
- The four upgrades now use supported template hooks where possible, mainly direct damage, projectile speed, and spawn-delay breathing room.

## Current Scope

This bootstrap intentionally avoids polished art, real monetization, store builds, and ECS. The authored starter pack proves the first concrete asset-flip path with names, balance, placeholder visuals, and optional audio references while keeping reusable gameplay behavior in Deucarian packages.

Placeholder visuals use primitive meshes with a moss, teal, charcoal, amber, and muted sticky accent direction. Richer upgrade semantics such as true weapon fire-rate/range changes remain reusable template/package follow-ups; Moss should not fork those systems locally.
