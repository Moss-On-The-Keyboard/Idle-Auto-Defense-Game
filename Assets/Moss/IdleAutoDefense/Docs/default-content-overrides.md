# Default Content Overrides

Moss was bootstrapped from the Deucarian Idle Auto Defense template sample. The copied starter content currently lives under:

```text
Assets/Moss/IdleAutoDefense/Content
```

The first Moss content pass should edit the product-owned override files here instead of changing Deucarian package source.

## Edit First

Start with:

```text
Assets/Moss/IdleAutoDefense/Content/moss-content-overrides.json
```

Then edit the focused override files:

- `Overrides/Enemies/moss-enemies.json`
- `Overrides/Weapons/moss-weapons.json`
- `Overrides/Waves/moss-waves.json`
- `Overrides/Upgrades/moss-upgrades.json`
- `Overrides/Progression/moss-progression.json`
- `Monetization/moss-monetization-overrides.json`

## Override Enemies

Copy values from the template `DefaultEnemies` pack, then rename IDs away from `enemy.template.*`. Add Moss-specific prefab references later under `Assets/Moss/IdleAutoDefense/Prefabs`.

## Override Weapons

Copy values from the template `DefaultWeapons` pack, then rename weapon, attack, projectile, mount, and source IDs. Keep one direct weapon and one projectile weapon until the smoke scene is stable.

## Override Waves

Copy values from the template `DefaultWaves` pack, then rename encounter, wave, and group IDs. Tune channel timing and counts before adding new encounter rules.

## Override Upgrades

Copy values from the template `DefaultUpgrades` pack, then rename upgrade, effect, and target IDs. Keep a deterministic draft while testing.

## Override Progression And Rewards

Copy values from the template `DefaultProgression` pack, then rename currencies, save documents, operations, tracks, and unlocks. Moss should own product save DTOs before shipping real content.

## Override Monetization

Copy values from the template `DefaultMonetization` pack, then rename rewarded, interstitial, and placeholder IAP IDs into the `moss.*` namespace. Keep Phase 2H on mock/no-op providers only. Real provider adapters should be added through reusable Deucarian integration packages later.

## Keep In Deucarian Packages

Do not copy reusable package source into Moss. Keep runtime systems, catalogs, adapters, persistence, progression, spawning, navigation, projectiles, weapons, upgrades, and monetization abstractions in Deucarian packages.
