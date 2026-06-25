# Default Content Overrides

Moss was bootstrapped from the Deucarian Idle Auto Defense template sample. The copied starter content currently lives under:

```text
Assets/Moss/IdleAutoDefense/Content
```

The first Moss content pass now lives as authored ScriptableObject content under `Assets/GameContent/MossOnTheKeyboard`. Keep these JSON files as historical bootstrap/reference material unless a specific legacy-flow test still needs them.

## Edit First

Start with:

```text
Assets/Moss/IdleAutoDefense/Content/moss-content-overrides.json
```

Then edit the focused override files:

- `Overrides/Stages/moss-stages.json`
- `Overrides/Enemies/moss-enemies.json`
- `Overrides/Weapons/moss-weapons.json`
- `Overrides/Waves/moss-waves.json`
- `Overrides/Upgrades/moss-upgrades.json`
- `Overrides/Progression/moss-progression.json`
- `Monetization/moss-monetization-overrides.json`

## Override Stages

Copy values from the template `DefaultStages` pack, then rename stage, encounter, reward, enemy, weapon, and upgrade references into the `moss.*` namespace. Keep First Orbit, Pressure Ring, Boss Pulse, and Endless placeholder represented until Moss has a product-specific progression map.

## Override Enemies

Copy values from the template `DefaultEnemies` pack, then rename IDs away from `enemy.template.*`. Moss mirrors Swarm, Runner, Tank, Shielded, Elite, and Boss so product tuning can change names, numbers, and prefabs without rewriting the template loop. Add Moss-specific prefab references later under `Assets/Moss/IdleAutoDefense/Prefabs`.

## Override Weapons

Copy values from the template `DefaultWeapons` pack, then rename weapon, attack, projectile, mount, and source IDs. Keep Pulse Cannon and Shard Launcher behavior until the smoke scene is stable. Arc Emitter and Orbital Shot stay as future intent records until reusable Deucarian systems support those behaviors.

## Override Waves

Copy values from the template `DefaultWaves` pack, then rename encounter, wave, and group IDs for each stage. Tune channel timing and counts before adding new encounter rules.

## Override Upgrades

Copy values from the template `DefaultUpgrades` pack, then rename upgrade, effect, and target IDs. Keep the 14-upgrade coverage shape while testing: damage, fire-rate intent, projectile count intent, projectile speed, health, repair, shield intent, rewards, offline gains, reroll intent, crit intent, and direct/projectile specializations.

## Override Progression And Rewards

Copy values from the template `DefaultProgression` pack, then rename currencies, save documents, operations, tracks, unlocks, and research nodes. Moss should own product save DTOs before shipping real content.

## Override Monetization

Copy values from the template `DefaultMonetization` pack, then rename rewarded, interstitial, and placeholder IAP IDs into the `moss.*` namespace. Keep Phase 2H on mock/no-op providers only. Real provider adapters should be added through reusable Deucarian integration packages later.

## Keep In Deucarian Packages

Do not copy reusable package source into Moss. Keep runtime systems, catalogs, adapters, persistence, progression, spawning, navigation, projectiles, weapons, upgrades, and monetization abstractions in Deucarian packages.
