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

Current limitations:

- Placeholder prefabs are primitive Unity meshes intended for validation, not final art.
- Audio clips are short generated tones used to prove optional audio references serialize and preview.
- The older JSON override content remains as historical bootstrap material; the `Assets/GameContent` pack is the source of truth for the concrete playable recipe.
