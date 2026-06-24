# Template Game Flow

Moss currently follows the Deucarian template canonical flow:

```text
Boot
-> load profile/save
-> resolve monetization availability
-> apply offline reward
-> optionally offer rewarded 2x offline reward
-> select/start stage run
-> spawn waves
-> auto weapons fire
-> upgrade draft moments
-> optional rewarded reroll
-> win/fail
-> optional rewarded revive on failure
-> apply rewards
-> optional rewarded double reward
-> optional interstitial at transition if pacing allows
-> save
-> restart/return
```

Canonical template docs:

```text
Packages/com.deucarian.template.game.idle-auto-defense/Documentation~/canonical-game-flow.md
Packages/com.deucarian.template.game.idle-auto-defense/Documentation~/default-content-and-balance.md
Packages/com.deucarian.template.game.idle-auto-defense/Documentation~/override-guide.md
```

Moss should customize content and balance before forking this flow. Phase 2I mirrors the template's First Orbit, Pressure Ring, Boss Pulse, Endless placeholder, enemy archetypes, module list, upgrade catalog, progression unlocks, and research-like defaults into product-owned override files. A future flow fork should be intentional and documented with the product reason.

Moss monetization work in Phase 2H is limited to product-owned override config and docs. Mock/no-op monetization behavior remains in `com.deucarian.monetization` and the reusable template package; no real ad SDKs or billing SDKs are added to Moss.

The current product-owned bootstrap is:

```text
Assets/Moss/IdleAutoDefense/Scripts/MossIdleAutoDefenseGameBootstrap.cs
```

That class stays thin: it subclasses the template controller, adds Moss save/reset behavior, and keeps reusable systems inside Deucarian packages.
