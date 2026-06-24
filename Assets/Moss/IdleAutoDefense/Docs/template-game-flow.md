# Template Game Flow

Moss currently follows the Deucarian template canonical flow:

```text
Boot
-> Load sample profile/save
-> Apply offline reward
-> Show/start run state
-> Start run
-> Spawn waves
-> Auto weapons fire
-> Run upgrade draft moments
-> Win/fail
-> Apply rewards
-> Save
-> Restart/return
```

Canonical template docs:

```text
Packages/com.deucarian.template.game.idle-auto-defense/Documentation~/canonical-game-flow.md
Packages/com.deucarian.template.game.idle-auto-defense/Documentation~/default-content-and-balance.md
Packages/com.deucarian.template.game.idle-auto-defense/Documentation~/override-guide.md
```

Moss should customize content and balance before forking this flow. A future flow fork should be intentional and documented with the product reason.

The current product-owned bootstrap is:

```text
Assets/Moss/IdleAutoDefense/Scripts/MossIdleAutoDefenseGameBootstrap.cs
```

That class stays thin: it subclasses the template controller, adds Moss save/reset behavior, and keeps reusable systems inside Deucarian packages.
