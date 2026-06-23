# Deucarian Package Usage

The project consumes the Deucarian package stack through Git URLs in `Packages/manifest.json`.

Key package roots:

- `com.deucarian.package-installer`
- `com.deucarian.auto-defense-suite`
- `com.deucarian.template.game.idle-auto-defense`
- `com.deucarian.test-automation`

The template package remains installed for this bootstrap smoke scene. Product-specific code lives in the `Moss.IdleAutoDefense` assembly and subclasses the template controller while Moss-specific save/reset behavior lives in `MossIdleAutoDefenseSave`.

When the game grows beyond the starter scene, copy only the behavior that must become product-owned and leave shared Deucarian packages unchanged.
