# First Concrete Asset Flip Proof

The concrete Moss content lives under:

```text
Assets/GameContent/MossOnTheKeyboard
```

Open `Tools/Deucarian/Game Content Authoring`, then use `Content Library` to validate the pack:

- Content Pack: `contentpack.moss-on-the-keyboard.starter`
- Content Set: `contentset.moss-on-the-keyboard.starter-run`
- Attacks: `Spore Pop`, `Cursor Ray`, `Moss Seeker`
- Enemies: `Dust Mite`, `Sticky Crumb`, `Cable Beetle`
- Waves: `Desk Dust`, `Keyboard Bloom`
- Weapons: `Keycap Spore Wand`, `Cursor Beam`, `Moss Seeker Sprout`
- Upgrades: `Faster Typing`, `Spore Pressure`, `Longer Reach`, `Sticky Spores`

The scene `Assets/Moss/IdleAutoDefense/Scenes/MossIdleAutoDefense.unity` has the starter pack and starter set assigned to `MossIdleAutoDefenseGameBootstrap`. The template controller should report that it is using the assigned content pack and content set, with no fallback warnings.

Game-specific work in this proof is limited to authored content, placeholder prefabs/materials/audio, the thin Moss bootstrap/save shell, and project documentation. Reusable gameplay behavior remains in the Deucarian template and gameplay packages.

## How To Inspect Or Change It

1. Open `Tools/Deucarian/Game Content Authoring`.
2. Select `Content Library`.
3. Select `Moss On The Keyboard Starter Pack` and confirm the library shows no blockers or warnings.
4. Select `Moss Starter Run` to review the starting weapon, enemy pool, wave list, and upgrade pool.
5. Edit one authored asset under `Assets/GameContent/MossOnTheKeyboard`, then re-run Content Library validation.
6. Open `Assets/Moss/IdleAutoDefense/Scenes/MossIdleAutoDefense.unity` and press Play.

The active authored objects are ScriptableObject assets. Their root assets own sub-assets for sections such as delivery, targeting, enemy stats, presentation, wave entries, weapon firing, upgrade economy, and upgrade effects.

## Asset-Flip Boundary

Allowed concrete-game changes in this proof:

- Authored ScriptableObject content and sub-assets.
- Placeholder materials, prefabs, VFX, banner texture, and preview audio.
- Scene assignment of the Moss content pack/content set.
- Thin project bootstrap/save shell and docs.

Not owned by this repo:

- Runtime combat, spawning, projectile, weapon, upgrade, progression, or content-authoring systems.
- Shared Deucarian editor styling.
- Template fallback behavior.

Current limitations:

- Placeholder prefabs are primitive Unity meshes intended for validation, not final art.
- Audio clips are short generated tones used to prove optional audio references serialize and preview.
- The older JSON override content remains as historical bootstrap material; the `Assets/GameContent` pack is the source of truth for the concrete playable recipe.
