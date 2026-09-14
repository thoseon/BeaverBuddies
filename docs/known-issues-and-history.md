# Known issues and fix history

Check here before forming a hypothesis: the same desync classes recur.

## The maintainer's issue log (`DeterminismService.cs:51-116`)

Reproduced with annotations. Keep the source comment as the canonical copy and update both.

**Current desync issues**: "*knock on wood*" (none known at the time of v1.7.2).

**Theories for unexplained desyncs**
- Something is not captured in the save state (e.g. when to go to bed), so a reload produces different behaviour. *Relevant to the "late join impossible" limitation.*
- Floating-point rounding in movement. No evidence so far on the same OS. *Cross-OS play is reported to work but is untested by the maintainer.*
- Gameplay code in `OnDestroyed`, which can run at end of frame instead of during the tick.

**To try**
- Debug mode plus more `Trace` calls (the standard method now).
- Remove all randomness (`NO_RANDOM`), remove interpolating animations (`NO_SMOOTH_ANIMATION`), remove water logic.
- Log random calls during load to look for non-gameplay logic.

**Known issues**
- Either `NaturalResourceReproducer.TryReproduceResources` (less likely) or `WateredNaturalResource.StartDryingOut` (more likely) desyncs, probably via a timer being created. Time-trigger logging produces many false positives (which is why the `TimeTriggerService` traces are commented out in `DesyncPatches.cs`).

**Monitoring**
- Other singletons may have game logic in `UpdateSingleton` (see `TickReplacerService`).
- Some `TimeTrigger`s fire slightly out of sync, but only non-game ones observed so far.
- Game state seems synced at load; needs more confirmation.

**Ruled out**
- Unaccounted `UnityEngine.Random` calls at desync time: none; the state was altered earlier.
- New GUIDs created on load before RNG sync: just the patching.
- Inconsistent update order from hash codes/buckets: consistent.
- **HashSet enumeration order**: not a cause. Order depends only on the sequence of adds/removes, not on hash codes.

**Fixed**
- `Guid.NewGuid` is deterministic (Unity RNG).
- New entities get deterministic GUIDs and are added deterministically to the bucket service; the tick completes so `Start()` runs on the next update.
- `Time.time` is deterministic (was a primary cause; exact mechanism never identified).
- Beaver movement is tick-only, with a render-only animation layer that is undone before each tick (`AnimationFixes`, `TEBPatcher`).
- `WateredNaturalResource.Awake()` / `LivingWaterNaturalResource.Awake()` RNG during load: fixed by seeding RNG from the map hash on both sides.
- Singleton `UpdateSingleton` game logic moved to `TickReplacerService`.

## Open items

From `BeaverBuddies/Doc/ToTestV6.md`:
- 3D water and wonders may have new UI to sync.
- Parallelism of the new water logic.
- Index-out-of-bounds with swimming beavers.
- `WaterObjectService` update moved to tick; watch for desyncs.
- **Desyncs from lag** (reproducible with heavy tracing).
- Auto-host last map; auto-join testing.

Code TODOs with risk notes:

| Location | Note | Risk |
|---|---|---|
| `IO/FileIO.cs:69` | `Formatting.Indented` still on ("undo for production") | Bloats every network message; harmless to sync. |
| `ReplayService.cs:375` | `HandleDesync` relies on sends being synchronous ("short-term fix") | Trace may not reach the other side on desync. |
| `ReplayService.cs:441` | `waitUpdates = 2` hack before init | Fragile startup timing. |
| `ReplayService.cs:92` | `EventIO` should be instance-scoped | Static state across sessions. |
| `TimberNet/TimberServer.cs:66` | Accept loop may hang on dropped connection | Host hang reports. |
| `TimberNet/TimberServer.cs:161` | Events from a prior frame may be queued too early | Ordering during join. |
| `TimberNet/TimberNetBase.cs:256` | Read can hang if map transfer stalls | Client stuck on join. |
| `IO/ServerEventIO.cs:67,108` | Map kept in memory forever | Memory only. |
| `IO/NetIOBase.cs:64` | Wasteful `JObject` round-trip | Perf only. |
| `Events/AutomationEvents.cs:170` | Valve/water methods are not really "automation"; class needs splitting | Naming only. |
| `Events/ConnectionEvents.cs:40` | Debug mode should maybe come from the server | Host/client `AlwaysTrace` mismatch UX. |
| `Events/ToolEvents.cs:85` | Recheck preview instantiation under the new blueprint system | Preview side effects (district fix class). |
| `Events/ToolEvents.cs:168` | Deletion patch may affect other deletions | Over-capture. |
| `Connect/RehostingService.cs:52` | Should check `IAutosaveBlocker`s before saving | Corrupt rehost save. |
| `DesyncDetecter/DesyncPatches.cs:214` | Moisture tracing too laggy → lag-induced desync | Do not re-enable. |
| `DeterminismService.cs:522` | Many non-game RNG patchers redundant vs blacklist | Cleanup. |
| `DeterminismPatcher.cs:31,39` | Water runs in parallel; cannot create dynamic patch | Dead file. |

## Reference fixes (read these before writing a new one)

| Commit | Title | Root cause | Fix pattern |
|---|---|---|---|
| `c246fd1` | Fix rotation-based desyncs (#200) | `TEBPatcher` called `anim.UpdateTransform(0)` before each tick, leaving **rotation** mid-interpolation; per-frame drift leaked into gameplay (walker "arrived" on one side only). | Force rotation to complete before every tick: `UpdateTransform(Time.deltaTime * 1000)` (`DeterminismService.cs` ~998). Added `skipStackTrack` to `Trace` and four new traces. Bisected via `NO_SMOOTH_ANIMATION` and commenting out lines. |
| `87fb778` | Fix bad water discharge automation desyncs (#184) | Hand-written `WaterSourceRegulator` events missed the `Automate` path. | **Delete** the bespoke events; add `Open/Close/Automate` to the automation list (`AutomationEvents.cs` ~178-180). Net −56 lines. |
| `382e3c7` | Fix power clutch automation desyncs (#185) | `Clutch.SetMode` not intercepted. | One line in the automation list. The minimal "missing event" fix. |
| `5e7fd60` | Fix district-related building desyncs and crashes (#128) | (a) `BuildingPlacedEvent.Replay` validates by instantiating a preview; district-center previews register with the district map/nav mesh, so the real placement throws. (b) Destroying a district center triggers migration against an already-removed cluster. | (a) Skip validation for `DistrictCenter.*` prefabs (`ToolEvents.cs` ~47). (b) `HarmonyFinalizer`s swallowing specific exceptions and returning empty results (`Fixes/DistrictBuildingsFix.cs`). |
| `4bb4799` | Fix randomness with initial beaver naming (#147) | `BeaverNameService.RandomName` runs in `Start()`, outside a tick; the heuristic treated it as cosmetic, so host and client used different generators. | Added `activeGamePatchers` / `SetGamePatcherActive` (checked before the non-game set) and a prefix/postfix marker on the method. |
| `7787b1e` | Fix science unlocks for real | `BuildingUnlockedEvent.Replay` fired the UI event `OnToolUnlocked` directly, leaving `ToolUnlockingService._activeLockers` stale. | Call the authoritative service: `Traverse` to `_toolUnlockingService`, `Unlock(tool)` guarded by `IsLocked(tool)`. **Replay must call the real method, not re-post its side effect.** |
| `934c49c` | Automation (#154) | Dozens of near-identical setter patches needed for logic buildings. | One `UniversalPrefix` + method cache + argument (de)serialisation (`AutomationEvents.cs`); introduced `DoEntityPrefix`. |
| `d66dd76`, `04d9a2c` | Mechanical fluid pump (#161) | Private `WaterMoverToggle.SetWaterMovement` with two coupled bools. | Hand-written `WaterMoverModeChangedEvent` patching the private method by string name. |

Also instructive: `e0d465d` (save overflow, 4 lines in `DeterminismService.cs`), `cf2885e` (Steam overlay join dialog), `7787b1e`'s predecessor `37fc2a3`.

## Changelog desync entries

From `BeaverBuddies/changelog.txt`:

- v1.7.2: common desync when beavers entered buildings (the #200 rotation fix).
- v1.7.1: automatable inputs set to (None); power clutch; badwater discharge automation; district center placement crashes/desyncs.
- v1.7.0: various desyncs; floodgate automation; valves/fill valves.
- v1.6.6: automation support (except HTTP); unlock sync; water pump filter sync.
- v1.6.4: automation buildings known to desync.

## Recurring lessons

1. **Replay must call the authoritative game method.** Re-posting a UI event leaves service state stale.
2. **Preview objects have side effects.** Validation-by-instantiation can register districts/obstacles; either skip validation or finalize the exception.
3. **Gameplay RNG outside ticks** gets misclassified as cosmetic. Look for `Unknown random called outside of tick`.
4. **Frame time leaks** (`Time.time`, `deltaTime`, `UpdateSingleton`) are the deepest class; fix by moving onto the tick or completing interpolation before the tick.
5. **Parallel water** must not be mutated mid-simulation; buffer to `FinishParallelTick`.
6. **Prefer the automation list** over bespoke events for UI setters on `BaseComponent`s.
7. **Heavy tracing itself desyncs** via lag. Keep traces cheap.
