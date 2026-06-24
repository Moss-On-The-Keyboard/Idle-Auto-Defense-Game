# Monetization Overrides

Moss inherits the reusable mock/no-op monetization flow from the Deucarian Idle Auto Defense template and `com.deucarian.monetization`.

Phase 2H keeps Moss limited to product-owned override data:

```text
Assets/Moss/IdleAutoDefense/Content/Monetization/moss-monetization-overrides.json
```

The override file renames template placements into the `moss.*` namespace and records product decisions for rewarded offers, transition-only interstitials, and placeholder IAP entries.

Moss has no real ad SDKs in Phase 2H.

## Guardrails

- Do not add AdMob, Unity Ads, AppLovin, LevelPlay, billing, analytics, store config, or privacy policy generation in Moss during Phase 2H.
- Keep reusable provider abstractions, placement pacing, claim identity, no-ads entitlement, and consent gates in Deucarian packages.
- Add real provider adapters later as reusable integration packages, then map Moss placement IDs to those adapters from product config.
