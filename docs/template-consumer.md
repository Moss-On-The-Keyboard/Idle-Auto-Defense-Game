# Template Consumer Notes

This project imports the `Basic Idle Auto Defense Game` sample from `com.deucarian.template.game.idle-auto-defense` as project-owned content.

The active scene is:

```text
Assets/IdleAutoDefense/Scenes/BasicIdleAutoDefense.unity
```

The imported bootstrap keeps only project-specific scene UI and sample save/reset behavior. All reusable auto-defense, combat, progression, persistence, spawning, projectile, weapon, upgrade, and monetization behavior remains in Deucarian packages.

When turning this starter into a real product, rename IDs and assets only after a concrete product theme exists. Until then, keep the neutral `template.*` IDs and validate that the assigned content pack and content set run without fallback warnings.
