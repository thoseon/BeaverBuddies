# Determinism reference

Everything that makes Timberborn compute the same result on every machine lives in `BeaverBuddies/DeterminismService.cs` (plus `Fixes/AnimationFixes.cs` and `GameSaveHelper.cs`). This page is the map of that file.

Compile-time bisect switches at the top of the file:

```csharp
//#define NO_PARALLEL   // DeterminismService.cs:2 - run parallel singletons on the main thread
//#define NO_RANDOM     // DeterminismService.cs:5 - replace RNG with constants, GUIDs with a counter
```
Related: `//# define NO_SMOOTH_ANIMATION` (`Fixes/AnimationFixes.cs:1`) and `//#define ONE_TICK_PER_UPDATE` (`ReplayService.cs:3`). See [debugging-desyncs.md](debugging-desyncs.md#bisect-switches) for how and when to flip them.

## The maintainer's issue log

`DeterminismService.cs:51-116` is a long comment with sections *Current desync issues*, *Theories*, *To try*, *Known Issues*, *Monitoring*, *Ruled out*, *Fixed*. Read it before forming a hypothesis. Reproduced and annotated in [known-issues-and-history.md](known-issues-and-history.md). Two headline facts:

- **HashSet enumeration order is ruled out** as a desync cause (lines 91-95). Order depends only on the sequence of adds/removes, which is itself deterministic.
- `Time.time` was a primary historic cause (line 103).

## RNG: gameplay vs cosmetic

### The decision: `ShouldFreezeSeed` (`DeterminismService.cs` ~line 175)

"Should this random call use the *non-game* generator?" Evaluated in this order:

1. `EventIO.IsNull` → **no** (single player, do not care).
2. `IsNonGameplay` flag → **yes**.
3. `!ReplayService.IsLoaded` → **no**. Randomness during load (tree lifespans, etc.) *is* gameplay. Traces `Load RNG; s0 before: XXXXXXXX`.
4. Not on the Unity main thread → **yes**, and `LogUnknownRandomCalled()`.
5. `activeGamePatchers.Count > 0` → **no** (a method explicitly marked as game logic is on the stack).
6. `activeNonGamePatchers.Count > 0` → **yes** (a method explicitly marked cosmetic is on the stack).
7. `IsTicking` → **no**. Traces `Tick RNG; s0 before: XXXXXXXX; Last entity: <name> - <guid>`.
8. `ReplayService.IsReplayingEvents` → **no**.
9. Otherwise → **yes**, and logs **`Unknown random called outside of tick`** with a stack trace (`DeterminismService.cs` ~line 310).

**If you see `Unknown random called outside of tick` in a log, that stack trace is a lead.** It means gameplay-looking code used RNG outside a tick and BeaverBuddies guessed "cosmetic". If the guess is wrong (as with beaver naming, fix `4bb4799`), host and client draw from different generators.

The non-game generator is a plain `System.Random` (line ~124), so it never advances Unity's state.

### Interception points

| Patched API | Where | What it does |
|---|---|---|
| `RandomNumberGenerator.Range(float,float)` | `DeterminismService.cs` ~331 | Route to non-game RNG when `ShouldFreezeSeed`. |
| `RandomNumberGenerator.Range(int,int)` | ~351 | Same. |
| `RandomNumberGenerator.InsideUnitCircle()` | ~371 | Same; restores `Random.state` afterward. |
| `ParameterProvider.GetParameters` | ~457 | **The systematic approach.** DI-level swap: any class in the `blacklist` (~462-481) is injected a `NonTickRandomNumberGenerator` instead of the real generator. |

The current blacklist: `BeaverTextureSetter`, `BotManufactoryAnimationController`, `BasicSelectionSound`, `BrushProbabilityMap`, `DateSalter`, `GameMusicPlayer`, `NaturalResourceModelRandomizer`, `RuinModelFactory`, `RuinModelUpdater`, `LoopingSoundPlayer`, `Sounds`, `GoodColumnVariantsService`, `GoodPileVariantsService`, `StockpileGoodPileVisualizer`, `TerrainBlockRandomizer`, `ObservatoryAnimator`, `WaterInputPipeSegmentCreator`.

Legacy prefix/postfix scope markers (mostly superseded by the blacklist; a comment at ~522 says so): `InputService.UpdateSingleton`, `Sounds.GetRandomSound`, `SoundEmitter.Update`, `DateSalter.GenerateRandomNumber`, `PlantableDescriber.GetPreviewFromTemplate`, `StockpileGoodPileVisualizer.Awake`, `RecoveredGoodStackFactory.RandomizeRotation`, `LoopingSoundPlayer.PlayLooping`, `BotManufactoryAnimationController.ResetRingRotation`, `TerrainBlockRandomizer.PickVariation`, `BeaverTextureSetter.Start` (~524-700). Each brackets the call with `SetNonGamePatcherActive(typeof(X), true/false)`.

The one **game**-marked method: `BeaverNameService.RandomName` (~589). Names are chosen in `Start()`, outside a tick, but must be identical on both sides, so it uses `SetGamePatcherActive`.

### The RNG audit

`BeaverBuddies/Doc/ClassesWithRandom.txt` lists every Timberborn class that uses RNG, with a legend:

```
=  Fully addressed
#  Verified game logic or irrelevant
>  At least partially addressed
~  Needs investigation
+  Unaddressed concern
```

When a `Tick RNG` trace diverges and the stack ends in a class you do not recognise, look it up here. Lines with `~` or `+` or no marker are the suspects. The list was generated with the ILSpy scans in `Inspector/Program.cs`; the commented-out `ReflectionUtils` probes in `Plugin.cs:87-96` can list derived classes, HashSet fields, and static fields after a game update.

## Non-RNG determinism

| Patched API | Where | Why |
|---|---|---|
| `Guid.NewGuid` | `DeterminismService.cs` ~786 (`GuidPatcher`) | Entity IDs are built from 16 bytes of `UnityEngine.Random`, so both sides create the same GUIDs. `GuidPatcher.RealNewGuid()` escapes the patch (used for `ReplayEvent.LocalPlayerID`). Traces `Generating new GUID: …` in Debug. |
| `EntityService.Instantiate(Blueprint, Guid)` | ~843 | Retries up to 100 times on a duplicate GUID (collisions happen against a save during preload; warns `Duplicate GUID … Attempt #n` after tick 0). Sets `TickingService.ShouldInterruptTicking = true` so the new entity's `Start()` runs before the next bucket. |
| `Time.time` getter | ~891 (`TimeTimePatcher`) | **Native Unity property**, so it is patched with a MonoMod `CreateSimpleDetour`, not Harmony. Returns `ticks * Time.fixedDeltaTime`; falls back to `Time.timeAsDouble` when `EventIO.IsNull`. Installed from `Plugin.cs:122`. |
| `DayNightCycle.FluidSecondsPassedToday` | ~935 `[ManualMethodOverwrite]` | Drops the frame-based `_secondsPassedThisTick` term. |
| `GameSaver.Save` | ~728 (`GameSaverSavePatcher`) | Has a `try-catch-when` that Harmony cannot patch (Harmony issue #563), so it uses a MonoMod `Hook`. Defers the save until a full tick completes. Installed from `Plugin.cs:121`. |
| `Autosaver.CreateExitSave` | ~760 | Sets `IsSaving` so exit-save skips tick sync. |
| `Ticker.Update` | ~773 | Sets/clears `DeterminismService.IsTicking`. |
| `TickableEntity.Tick` | ~248 | Records `currentlyTickingEntity` so trace messages can name the entity. |
| `TickableEntityBucket.TickAll` | ~948 (`TEBPatcher`) | Per entity before it ticks: fold `EntityUpdateHash`, snap `AnimatedPathFollower.CurrentPosition` to the deterministic `PathFollower` transform, fold `PositionHash`, force-complete rotation via `anim.UpdateTransform(Time.deltaTime * 1000)` (~998, the #200 fix). This is the anti-animation-drift guard. |
| `TickableBucketService.FinishFullTick` | ~708 | Warns `Finishing full tick - this probably is bad!` unless saving. |
| `TickableSingletonService.StartParallelTick` | ~1067 | Only under `NO_PARALLEL`. |
| `RecoveredGoodStackSpawner.UpdateSingleton` | ~1089 | Suppresses the frame-driven spawn; re-driven from `TickReplacerService.Tick()`. |
| `DateSalter.Save`, `DateTime.ToString(string)` | `GameSaveHelper.cs:23, 32` | Zero timestamps while saving deterministically so save bytes match between sides. |

## Movement and animation

Timberborn's `MovementAnimator.Update` uses wall-clock `Time.time`, which drifts between machines. `Fixes/AnimationFixes.cs` replaces it (`[ManualMethodOverwrite]`, line 12) with an interpolation from `TickProgressService.TimeAtLastTick(entity) + fixedDeltaTime * PercentTicked(entity)`: smooth per frame, identical at tick boundaries. `TEBPatcher` then snaps position and completes rotation before each tick so nothing frame-based leaks into gameplay.

The chain `Walker → PathFollower → MovementAnimator → AnimatedPathFollower → CharacterModel` and where float drift enters is written up in `BeaverBuddies/Doc/Movement.md`. Read it for any desync whose trace involves `going to:`, `finished pathfinding`, `stopping movement`, `Enterer.Enter`, or `SlotManager`.

## Frame time (`Time.deltaTime`)

`TimeTimePatcher` detours `Time.time` only. **`Time.deltaTime` is untouched**, so any gameplay code that integrates with it advances by a machine-dependent amount per tick. Known case: `WaterDepthStrengthModifier.GetStrengthModifier` fades a spring's strength in with `FadeInSpeed * Time.deltaTime`, once per tick from `WaterSource.Tick`; `WaterSourceRegistry.Tick` snapshots the strength and the parallel water simulation consumes it, so the water map diverges one tick later. Fixed in `Fixes/WaterSourceStrengthFix.cs` by advancing with `ITickService.TickIntervalInSeconds`.

When a trace diverges in a value that is neither random nor movement, grep the decompiled class on the stack for `Time.deltaTime`. The audit of all game assemblies is in [known-issues-and-history.md](known-issues-and-history.md#frame-time-audit-timedeltatime); the open item is delayed dynamite (`UnstableCore.Update`).

The game never reads or sets `Time.fixedDeltaTime`. The mod's animation code uses it as a unit, but do not treat it as the tick length in new code.

## Frame-based nav mesh updates

Timberborn keeps a "regular" nav mesh/district map (updated in `NavigationSynchronizer.Tick()`, deterministic) and an "instant" one (updated in `NavigationSynchronizer.LateUpdateSingleton()`, once per rendered frame, listeners notified there too). Gameplay reads the instant copy in `Walker.PathIsTooFarFromDistrict`, `Citizen.UnassignDistrictIfCutOff` (via `GlobalReachabilityService`), `ReachableConstructionSite`, `ReachableDemolishable`, `RecoveredGoodStackAccessible` and the `IInstantNavMeshListener`s in `Timberborn.GameDistricts`. Because entity buckets are spread over frames, `Fixes/InstantNavMeshFix.cs` applies the instant changes at the start of every bucket as well; the per-frame call stays so previews update while paused. In Debug, `Applying instant navmesh changes: …` traces each application.

## Parallel simulation

Four Timberborn singletons tick in parallel (`BeaverBuddies/Doc/ParallelSingletons.txt`): `WaterSimulationController`, `WaterRenderer`, `SoilMoistureSimulationController`, `SoilContaminationSimulationController`.

- `WaterSource.Tick` mutated water state while the parallel sim was running, so the result depended on thread timing. `Fixes/WaterSourceFix.cs` buffers those calls in `LateTickableBuffer` and replays them from a postfix on `TickableSingletonService.FinishParallelTick`.
- The water map and moisture levels are hashed every tick in Debug (`DesyncDetecter/DesyncPatches.cs` ~226, ~251), so a water desync shows up as `Updating water map columns with hash` diverging.
- `Fixes/TestingStrategies_Scrap.cs` (not compiled) preserves a whitelist-bisection method for finding which parallel or tickable singleton diverges.
- `NO_PARALLEL` runs them single-threaded. The maintainer has never definitively tied a desync to parallelism but keeps it on the suspect list.

## How to add a determinism fix

Pick the least invasive route that applies:

1. **A class that should not use gameplay RNG** → add its type to the `blacklist` HashSet (`DeterminismService.cs` ~462). Preferred.
2. **A single method** → add a `[HarmonyPatch]` class with `Prefix`/`Postfix` calling `DeterminismService.SetNonGamePatcherActive(typeof(X), true/false)`, or `SetGamePatcherActive` for the reverse. Copy the pattern at ~544 (`Sounds.GetRandomSound`) or ~589 (`BeaverNameService.RandomName`). Remember `Reset()` clears both sets (~134).
3. **A singleton doing game logic in `UpdateSingleton`** → prefix-return-false it and re-invoke from `TickReplacerService.Tick()`. The comment at ~1086 notes that a generalised approach is warranted once there are more than ~3.
4. **Frame-time leaking into gameplay** → copy the vanilla method into a `[ManualMethodOverwrite]` patch with a dated comment containing the original code, and replace `Time.time`/`deltaTime` with tick-derived values (see `AnimationFixes.cs`, `DayNightCycle` patch).
5. Do **not** extend `DeterminismPatcher.cs`. It is an abandoned attempt to do route 2 dynamically and is never called from `Plugin.cs`.

After any Timberborn update, grep `[ManualMethodOverwrite]` (16 sites) and diff each copied body against the new decompiled source.
