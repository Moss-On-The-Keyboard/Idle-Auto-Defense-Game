# Basic Idle Auto Defense

Open `Scenes/BasicIdleAutoDefense.unity` and press Play.

The imported scene contains:

- A central core objective.
- Four visible perimeter spawn markers.
- A direct weapon mount and a projectile weapon mount.
- A `Basic Idle Auto Defense Content Set` assigned through the template content pack.
- Deterministic upgrade drafts, offline rewards, mock monetization hooks, sample save/reset behavior, and progression smoke paths.
- A small on-screen status panel with save/reset buttons.

All visible gameplay objects are primitive placeholders. Replace content under `Content`, `Prefabs`, `Visuals`, and `Audio` when turning the starter into a production game.

## Folder Map

```text
Basic Idle Auto Defense
|-- Audio
|-- Content
|   |-- ContentPacks
|   |-- ContentSets
|   |-- DefaultBalance
|   |-- DefaultEnemies
|   |-- DefaultMonetization
|   |-- DefaultProgression
|   |-- DefaultStages
|   |-- DefaultUpgrades
|   |-- DefaultWaves
|   |-- DefaultWeapons
|   `-- starter-content.json
|-- Docs
|-- Prefabs
|-- Scenes
|   `-- BasicIdleAutoDefense.unity
|-- Scripts
|   `-- BasicIdleAutoDefenseGameBootstrap.cs
|-- Tests
`-- Visuals
```
