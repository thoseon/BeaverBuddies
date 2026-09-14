# Debugging a desync

A desync means the Host's game state has diverged from a Client's. This is the number-one bug class in BeaverBuddies. This page adapts the wiki's [Debugging a desync](https://github.com/thomaswp/BeaverBuddies/wiki/Debugging-a-desync) guide to the current code and adds a decision tree for agents.

Background you need first: [architecture.md](architecture.md) (the three invariants) and [determinism.md](determinism.md).

## Prerequisites

- **Two running Timberborn instances** (host and client). Usually two game keys; a VM or a patient friend also works.
- **Detailed logging on both sides**: Mods → BeaverBuddies settings → `AlwaysTrace` (this is `Settings.Debug`). Host and client must match; `InitializeClientEvent` warns if they differ (`Events/ConnectionEvents.cs:38-44`). The desync dialog's "Enable Logging" button sets `Settings.TemporarilyDebug` for the current session only.
- **A save right before the desync** that reproduces within about a minute at full speed. Save *after* doing the triggering action if there is one, so you do not have to repeat it.

If you cannot reproduce reliably you cannot know whether you fixed it.

## How detection works

There are two detectors; which one fires depends on `Settings.Debug`.

1. **Cheap, always on when tracing is off** (`ReplayService.ReplayEvents`, `ReplayService.cs:326-336`): every event carries `randomS0Before`, the sender's `UnityEngine.Random.state.s0` at record time. Before replaying, the receiver compares its own s0. Mismatch → `Random state mismatch: X != Y` → `HandleDesync()`.
2. **Trace comparison when tracing is on** (`DesyncDetecter/DesyncDetecterService.cs`): every `Trace(...)` call appends a message (and usually a stack trace) to the current tick's list. At the start of the next tick the **server** ships its lists as `TraceLoggedForTickEvent`s (`ReplayService.DoTick`, `ReplayService.cs:515-525`). The **client** compares messages index by index in `VerifyTraces` (line 142). First mismatch → the report below → `HandleDesync()`. Only the last 10 ticks are kept (`maxTraceTicks`).

`HandleDesync` (`ReplayService.cs:357`) creates a `ClientDesyncedEvent`, sets `IsDesynced` (after which `DoPrefix` lets everything run locally), flushes pending sends, pauses, and resets the IO. The dialog offers the host "Save and Rehost" and the client "Reconnect"; the info button is "Post Bug Report" (Debug + `pat.properties`) or "Enable Logging" (not Debug).

## Step 1: get the trace

Do not reload. Open the **client's** `Player.log` (path in [logging-reference.md](logging-reference.md)) and scroll to the bottom:

```
Desync detected for tick 94!
========== Trace history ==========
(more traces...)                       <- prior ticks; last 5 messages each, no stacks
22c38830-... going to: SmallTank.Folktails(Clone)
Tick RNG; s0 before: 247FFE2A; Last entity: Bot.Folktails Timberbot 187 - 29724f4b-...
----------------------------------     <- tick separator
========== Shared History for Desynced Tick 94 ==========
Tick 94 started
Updating water map columns with hash C3C5D2E3
Updating water map column counts with hash BC09FD2F
Updating moisture levels with hash A76564F0
Tick RNG; s0 before: 0646C6D4; Last entity:  -
...
2fb9b615-... going to: (103.76, 2.00, 129.28)
  at BeaverBuddies.DesyncDetecter.DesyncDetecterService.Trace (...)
  at BeaverBuddies.DesyncDetecter.WalkerFindPathPatcher.Prefix (...)
  at Timberborn.WalkingSystem.Walker.GoTo (...)
  ...                                   <- stacks only for the last 5 shared messages
========== Desynced Trace ==========
---------- My Trace ----------
Tick RNG; s0 before: B4FB5726; Last entity: BeaverAdult Jijel - 959892c0-...
  at ... RandomRangeIntPatcher.Prefix
  at Timberborn.SlotSystem.SlotManager.TryGetUnassignedSlot
  at Timberborn.EnterableSystem.Enterer.Enter
  at Timberborn.WalkingSystem.WalkInsideExecutor.Tick
  at Timberborn.BehaviorSystem.BehaviorManager.TickRunningExecutor
  ...
--------------
---------- Other Trace ----------
Tick RNG; s0 before: B4FB5726; Last entity: BeaverAdult Satizix - 9f67451b-...
  ...
========== Desynced Log End ==========
```

Paste the report (or the whole `Player.log`) into **https://thomaswp.github.io/BBDesyncViewer/**. It renders the three sections and shows the divergent tick as a diff: green rows happened on both sides, red/blue rows on one side only. Expand a row for its stack trace.

## Step 2: interpret the trace

Three sections:

1. **Trace history** – prior ticks, still in sync as far as we know.
2. **Shared history for the desynced tick** – identical prefix of the tick.
3. **Desynced trace** – the first ~10 messages on each side after they diverge.

Read the first red/blue row. Its stack trace is not an error; it is the call path that led to the traced event. In the wiki example the server had a `Tick RNG` the client lacked, and the stack showed a beaver entering a building and being assigned a random slot. Question: why did the beaver enter on the server only?

## Step 3: classify (decision tree)

Find the first divergent message and match it:

| First divergent message looks like | Likely class | Go to |
|---|---|---|
| A player action (`Doing: …`, a `RecordEvent`, or a building/recipe/priority change) present on one side only | **Missing or incomplete `ReplayEvent`.** The action was not intercepted, or `Replay()` did something different from the original method. | [events-and-patches.md](events-and-patches.md). Easiest class to fix. |
| `Tick RNG; …` with a stack ending in UI, audio, animation, textures, or a class flagged `~`/`+` in `Doc/ClassesWithRandom.txt` | **Cosmetic code using gameplay RNG.** | [determinism.md](determinism.md#how-to-add-a-determinism-fix): add to blacklist or mark the method. |
| `Tick RNG; …` with a gameplay stack, but preceded by rows that are also one-sided | The RNG is a *symptom*. Walk up to the first one-sided row. | Continue below. |
| `Unknown random called outside of tick` anywhere earlier in either log | **Gameplay RNG outside a tick** treated as cosmetic (fix `4bb4799` pattern). | [determinism.md](determinism.md#rng-gameplay-vs-cosmetic). |
| `… going to: …`, `… finished pathfinding …`, `Walker … stopping movement`, `Entity … entering …`, `SlotManager adding enterer` | **Movement / animation drift.** Position or rotation differs, so an executor succeeds on one side. | `Doc/Movement.md`, `TEBPatcher` (`DeterminismService.cs` ~948), `Fixes/AnimationFixes.cs`. Try `NO_SMOOTH_ANIMATION`. |
| `Updating N water sources with hash …` differs | **Water source strength or contamination** differs: a frame-time based strength modifier, or a missed regulator/discharge event. | `Fixes/WaterSourceStrengthFix.cs`; `WaterSourceRegulator` entries in the automation list. |
| `Updating water map columns with hash …` differs while `column counts` and `Updating moisture levels` still match | The simulation's **inputs** changed. Check the `water sources` trace one tick earlier first, then water changes (pumps, discharge), flow limits (floodgates, sluices), and obstacles. The simulation itself is row-partitioned and deterministic regardless of thread count. | [determinism.md](determinism.md#frame-time-timedeltatime), `Fixes/WaterSourceStrengthFix.cs`, `Fixes/WaterSourceFix.cs`. |
| `Updating moisture levels with hash …` differs first | **Soil moisture** inputs (terrain, barriers) or a real parallel race. | [determinism.md](determinism.md#parallel-simulation). Try `NO_PARALLEL`. |
| `Marking spots …`, `Spawning: …`, `Trying to spawn …`, `starting to dry out` | Natural-resource reproduction / drying timers (an open item in the maintainer's log). | `DesyncPatches.cs` ~24-120, `DeterminismService.cs:71-76`. |
| `Adding: <guid> at index N` differs | **Entity creation order / GUID** differs. | `EntityService.Instantiate` patch, `GuidPatcher`. Check for `Duplicate GUID` warnings. |
| `Generating new GUID` on one side | Something created an entity on one side only. Usually a missing event or a preview object. | [events-and-patches.md](events-and-patches.md), `Fixes/DistrictBuildingsFix.cs`. |
| Only reproduces with tracing **on** | **Lag-induced.** Heavy tracing stalls a side. `Doc/ToTestV6.md` lists this as known. | Remove or `skipStackTrack` the hot traces; do not trust the trace as the cause. |
| `Random state mismatch` with tracing off and nothing obvious | Turn tracing on and reproduce. | Step 1. |

Red herring: `Number of threads: N` at load differs between machines (it is `clamp(physicalCores - 1, 3, 8)`). The water and soil simulations partition work by map row and write only their own rows, so the thread count does not change results by itself. Look for an input that differs, not the partitioning.

Also check both logs for `Failed to replay event`, `Could not find entity`, `Warning, replaying events when bucket != 0`, and different `Received map … Hash` values. Any of those explains a desync without further tracing.

## Step 4: add traces (binary search)

If the first divergent row is a symptom, add traces further up its stack. If the new traces agree on both sides, the divergence is between them and the old row; otherwise it is earlier (maybe a previous tick).

All trace patches live in `DesyncDetecter/DesyncPatches.cs`. Pattern (from the wiki, now in the file at ~304):

```csharp
[HarmonyPatch(typeof(Enterer), nameof(Enterer.Enter))]
public class EntererEnterPatcher
{
    // void prefix: never interferes with the original method
    static void Prefix(Enterer __instance, Enterable enterable)
    {
        // REQUIRED: the string building below is not free
        if (!Settings.Debug) return;

        // Parameterise with anything that might differ between the two games.
        // Write it null-safe; a trace must never crash the game.
        var entererEntityId = __instance.GetComponent<EntityComponent>()?.EntityId;
        var enterableEntityId = enterable?.GetComponent<EntityComponent>()?.EntityId;
        var enterableName = enterable?.GameObject?.name;
        DesyncDetecterService.Trace($"Entity {entererEntityId} entering {enterableName} ({enterableEntityId})");
    }
}
```

Rules:

- Always start with `if (!Settings.Debug) return;`.
- Called 10+ times per tick → pass `skipStackTrack: true` (third argument) as `BehaviorManagerTickRunningExecutorPatcher` does.
- Called 50+ times per tick → do not trace it. Stack capture will cause lag-induced desyncs. `SoilMoistureMap.SetMoistureLevel` was commented out for exactly this reason (~216).
- Add a few traces at different stack depths per run; reproducing is the slow part.
- Traces that survive a fix are welcome in the PR; comment out expensive ones.

What is traced today (message shapes):

| Patch target | `DesyncPatches.cs` | Message |
|---|---|---|
| `NaturalResourceReproducer.MarkSpots` / `UnmarkSpots` | ~24, ~47 | `Marking spots for … at … (…)`, `Spots updated: n --> m` |
| `SpawnValidationService.CanSpawn` | ~101 | `Trying to spawn … at …: result` + breakdown |
| `NaturalResourceReproducer.SpawnNewResources` | ~114 | `Spawning: key, coords` |
| `Walker.FindPath` | ~128 | `<guid> going to: …` / `<guid> finished pathfinding; reachable = …` |
| `NaturalResourceModelRandomizer.RandomizeDiameterScale` | ~173 | diameter scale |
| `WalkToReservableExecutor.Launch` | ~184 | target reachable |
| `WateredNaturalResource.StartDryingOut` | ~199 | `[dead=…] starting to dry out; trigger delay = …` |
| `SoilMoistureService.UpdateMoistureLevels` | ~226 | `Updating moisture levels with hash XXXXXXXX` |
| `ThreadSafeWaterMap.Update` | ~251 | `Updating water map columns with hash …`, `… column counts with hash …` |
| `WaterSourceRegistry.Tick` | end of file | `Updating N water sources with hash …` (no stack) |
| `TickableEntityBucket.Add` | ~287 | `Adding: <guid> at index n` |
| `Enterer.Enter` | ~304 | `Entity … entering … (…)` |
| `SlotManager.AddEnterer` | ~328 | `SlotManager adding enterer …` |
| `BehaviorManager.TickRunningExecutor` | ~339 | executor type + elapsed time (no stack) |
| `Walker.StopMoving` | ~351 | `Walker … stopping movement` |
| `DeterminismService` | ~197, ~225, ~816 | `Load RNG; …`, `Tick RNG; …; Last entity: …`, `Generating new GUID: …` |
| `DesyncDetecterService.StartTick` | | `Tick n started` |

Commented-out traces worth knowing: `TimeTriggerService.Trigger/Add` (~72, ~88; noisy false positives), `SoilMoistureMap.SetMoistureLevel` (~216; too laggy), `PathFollower.ReachedLastPathCorner` (~363; used in the #200 hunt).

To read the vanilla method you are tracing, open the decompiled sources in `_decompiled/` (one folder per assembly; see `CLAUDE.md`), or decompile the game DLLs with ILSpy (see [timberborn-modding.md](timberborn-modding.md)).

## Bisect switches

Four compile-time switches remove whole subsystems. Uncomment, rebuild, reproduce:

| Switch | File | Removes |
|---|---|---|
| `NO_SMOOTH_ANIMATION` | `Fixes/AnimationFixes.cs:1` | The custom deterministic character animator. |
| `NO_RANDOM` | `DeterminismService.cs:5` | All RNG (constants) and GUIDs (counter). |
| `NO_PARALLEL` | `DeterminismService.cs:2` | Parallel singleton ticking (runs single-threaded). |
| `ONE_TICK_PER_UPDATE` | `ReplayService.cs:3` | Multi-tick frames; one full tick per Unity update. |

`Fixes/TestingStrategies_Scrap.cs` (not compiled) preserves a finer method: whitelist individual parallel and tickable singletons to find the one that diverges.

**Caveat from the maintainer**: the desync going away when a switch is off does *not* prove that subsystem is the cause. Timberborn is chaotic; any change can move a desync later or stop the guilty code from running. Test for several minutes and check the trace still points at the same thing.

## Worked example: PR #200 (rotation desync)

1. Trace: server-only `Tick RNG` from `SlotManager.TryGetUnassignedSlot` ← `Enterer.Enter` ← `WalkInsideExecutor.Tick`.
2. Added traces on `Enterer.Enter`, `SlotManager.AddEnterer`, `BehaviorManager.TickRunningExecutor`. New trace: the executor ticked on both sides; `Enter` happened only on the server. So the divergence is inside `WalkInsideExecutor.Tick`.
3. Decompiled `WalkInsideExecutor.Tick`: it enters when `_walker.Stopped()`. Hypothesis: the walker had arrived on the server but not the client.
4. Traced `PathFollower.ReachedLastPathCorner`: confirmed, positions differed.
5. `NO_SMOOTH_ANIMATION` made the desync go away, pointing at `AnimationFixes.cs`.
6. Commenting out lines of `AnimationFixes.cs` revealed **rotation**, not position, was drifting: `TEBPatcher` called `anim.UpdateTransform(0)` before each tick, leaving rotation mid-interpolation.
7. Fix: `anim.UpdateTransform(Time.deltaTime * 1000)` (`DeterminismService.cs` ~998) so rotation completes to its tick-determined target before every tick, like position already did. Also added `skipStackTrack` to `Trace`.

## Step 5: fix, verify, submit

- Reproduce on clean `master` first, then with the fix.
- Run 5+ minutes without the desync (longer is better). A new desync with an unrelated trace may be unrelated.
- Follow an existing pattern. The eight reference fixes are in [known-issues-and-history.md](known-issues-and-history.md).
- File or update the GitHub issue with: the save, repro steps, Timberborn + BeaverBuddies versions, the trace, and hypotheses. Example: issue #199 → PR #200.
- Keep your trace patches in the PR (comment out the expensive ones).

## Offline recording and replay

`IO/FileIO.cs` contains a single-player record/replay path that is **not wired up**:

- `RecordToFileService` (`IPostLoadableSingleton`) sets `EventIO` to a `FileWriteIO("Replays/<save>.json")` on load and logs `Recording to file`. It is not bound in any configurator; add `Bind<RecordToFileService>().AsSingleton()` to `ReplayConfigurator` (`Plugin.cs`) to use it. The `Replays/` directory must exist (relative to the process working directory), and a crashed session leaves an unterminated JSON array to hand-fix.
- `FileReadIO(path)` replays such a file with `UserEventBehavior.Play` and auto-pauses when out of events. Nothing constructs it; set it with `EventIO.Set(new FileReadIO(path))` before load, and call `DeterminismService.InitGameStartState(mapBytes)` with the same map so the seed matches.

The wired-up alternative is the desync report: `ClientDesyncedEvent.PostDesync` (`Events/ConnectionEvents.cs:95-127`) saves a rehost file and uploads trace + map + versions via `Reporting/ReportingService.cs` when `pat.properties` is present.

## User-facing tips that explain many reports

From the wiki's troubleshooting section:

- Host must load a fresh save and **not unpause** before the client connects (late join is impossible).
- Heavily progressed saves that were never played with the mod may contain unsupported state; a new game that works narrows it down.
- Any other mod running alongside BeaverBuddies is a likely desync source.
- After a desync, the host should save; both can reload from there.
