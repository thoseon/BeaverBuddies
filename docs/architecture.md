# Architecture: how BeaverBuddies keeps two games in sync

Adapted from the wiki page [How does it work?](https://github.com/thomaswp/BeaverBuddies/wiki/How-does-it-work%3F) and the current source. Line numbers are from `master` at the time of writing; prefer the symbol names when they drift.

## The three invariants

BeaverBuddies is a **deterministic lockstep** multiplayer mod. Two (or more) copies of Timberborn stay identical because:

1. **Same start state** – every player loads the exact same save bytes.
2. **Mirrored inputs** – every player action is intercepted, sent to the Host, and executed by *everyone* at the *same tick*.
3. **Determinism** – given the same state and the same inputs, the game must compute the same next state on every machine.

A **desync** is any violation of (3) or (2). Almost every bug in this project is one of those two.

## 1. Start state

- The Host reads the raw `.timber` save bytes (`Connect/ServerHostingUtils.cs`, `GetMapBtyes`) and streams them to each client as the first network message.
- The client writes those bytes to a save named by the map hash under the settlement "Online Games" (`Connect/ClientConnectionService.cs`, `LoadMap`).
- Both sides call `DeterminismService.InitGameStartState(mapBytes)` (`DeterminismService.cs` ~line 314), which seeds `UnityEngine.Random` from a fold of the map bytes. **The map hash is both the save name and the RNG seed.** If host and client log different `Hash:` values for the map, nothing else will work.
- **Late join is impossible.** Loading a save re-rolls random state (tree lifespans, etc.), so the in-memory state of a running game is not reproducible from a save. See the comment at `IO/ServerEventIO.cs:14-22` and issue #3. The server stops accepting clients at tick 1 (`ReplayService.DoTick`, `StopAcceptingClients`).

## 2. Mirrored inputs

### Host/Client model

There is one Host (server) and N Clients. All inputs go to the Host, which broadcasts them. From the wiki:

- Client clicks "build" on tick 32. The event is recorded and sent, **not** executed locally.
- Client waits for tick 33's events. Host sends a Heartbeat for tick 33; client advances one tick.
- Host receives the build event and executes it on tick 34, then broadcasts it.
- Client receives the build event for tick 34 and executes it there.

Both are on tick 34 when the building appears. Two ticks (~0.66 s) of client delay.

### Interception: `ReplayEvent.DoPrefix`

Every player action is a Harmony prefix on the game method that performs the action. The prefix builds a `ReplayEvent` and hands it to `ReplayService`. The return value of `DoPrefix` (`Events/ReplayEvent.cs:112`) is the Harmony "run original?" flag:

- Host: `false` – the action is swallowed and queued to play at the next tick, where it is also broadcast (`UserEventBehavior.QueuePlay`). This keeps the host's own actions at the same point in the tick as remote ones.
- Client: `false` – the action is swallowed and only runs when the Host echoes it back (`UserEventBehavior.Send`).
- Offline recording/replay (`FileWriteIO`, `FileReadIO`, `UserEventBehavior.Play`): `true` – runs immediately.
- Nested calls during a replay, before load, or after a desync: `true` – behave like vanilla. Because the original is swallowed at record time, a patched method that internally calls another patched method never records twice; the nested call only happens during replay, where `IsReplayingEvents` short-circuits.

Full contract and examples: [events-and-patches.md](events-and-patches.md).

### The tick loop

BeaverBuddies **replaces** Timberborn's bucket ticking loop:

```
TickableBucketService.TickBuckets            (vanilla, patched at ReplayService.cs:753 [ManualMethodOverwrite])
  -> TickingService.TickBuckets              (ReplayService.cs:710)
     -> TickReplayServiceOrNextBucket        (ReplayService.cs:685)  bucket index 0 == ReplayService.DoTick
        -> ReplayService.DoTick              (ReplayService.cs:511)
           flush traces -> ticksSinceLoad++ -> enqueue HeartbeatEvent (server) -> DoTickIO -> io.Update -> UpdateSpeed
              -> DoTickIO                    (ReplayService.cs:427) == ReplayEvents() + SendEvents()
```

Key points:

- `DoTick` runs at the very start of a tick, before any entity bucket, after the previous tick's parallel work has finished (`FinishParallelTick` is called first).
- `ticksSinceLoad` setter (`ReplayService.cs:98-107`) fans out to `TimeTimePatcher.SetTicksSinceLoaded` (deterministic `Time.time`) and `DesyncDetecterService.StartTick` (new trace bucket).
- **Client gate**: `IsReadyToStartTick` (`ReplayService.cs:121`) refuses to start tick N+1 until `io.HasEventsForTick(N+1)`. The server enqueues a `HeartbeatEvent` every tick so this is always satisfiable. Log line when the gate blocks: `Client trying to tick before receiving Heartbeat at tick: N`.
- `ReplayEvents` (`ReplayService.cs:292`) warns if replaying when `NextBucket != 0`, flattens `GroupedEvent`s, and (when tracing is off) compares `randomS0Before` per event to detect desyncs.
- `IEarlyTickableSingleton` + `TickableSingletonServicePatcher` (`ReplayService.cs:577`) reorder singletons so BeaverBuddies' own tick first.
- `//#define ONE_TICK_PER_UPDATE` (`ReplayService.cs:3`) forces exactly one full tick per Unity `Update`, useful when debugging.

### Event IO: one object decides host vs client

There is exactly one `EventIO` per session (`IO/EventIO.cs`, `EventIO.Get()`), and code branches on its properties, not on a boolean:

| Property | `ServerEventIO` | `ClientEventIO` | `FileWriteIO` | `FileReadIO` |
|---|---|---|---|---|
| `RecordReplayedEvents` | true | false | true | false |
| `ShouldSendHeartbeat` | true | false | false | false |
| `UserEventBehavior` | `QueuePlay` | `Send` | `Play` | `Play` |

`EventIO.IsNull` means "not in a co-op game" and gates nearly every patch. `EventIO.ShouldPlayPatchedEvents` is what `DoPrefix` returns. Direct type checks exist in only three places (`ReplayService.cs:126, 558` and `Events/ConnectionEvents.cs:169`).

`FileWriteIO`/`FileReadIO` (`IO/FileIO.cs`) are an offline record/replay path for reproduction. They are **not bound anywhere**; see [debugging-desyncs.md](debugging-desyncs.md#offline-recording-and-replay).

### Speed and pause

- `TargetSpeed` is the player's intent; `SpeedManager.CurrentSpeed` is what the engine runs at.
- `UpdateSpeed` (`ReplayService.cs:475`): if the IO is out of events, silently force speed 0; if `io.TicksBehind > targetSpeed`, speed up (capped at 10) to catch up.
- `SpeedChangePatcher.SetSpeedSilentlyNow` (`Events/TimeEvents.cs:49`) changes speed *without* emitting a `SpeedSetEvent`. Use it for any internal speed change.
- Clients never auto-freeze for dialogs (`SpeedLockPatcher`, `SpeedUnlockPatcher` in `TimeEvents.cs`). Opening the options menu *is* a synced pause (`ShowOptionsMenuEvent`). The `PauseReduction` setting gates `OverlayPanelSpeedLockerShowPatcher`.
- While fully paused, `UpdateSingleton` still calls `DoTickIO()` so paused players stay responsive.
- Saves always land on a tick boundary via `FinishFullTickIfNeededAndThen` (`ReplayService.cs:564`) and the MonoMod hook on `GameSaver.Save`.

### Serialization and wire format

- Newtonsoft.Json with `JsonSettings` (`IO/FileIO.cs:65`): `TypeNameHandling.All`, `Formatting.Indented` (a TODO says undo for production), plus `Vector3`/`Vector3Int` converters.
- **There is no event registry.** The `$type` field carries the assembly-qualified class name, so a new event needs no registration. The flip side: **renaming or moving a `ReplayEvent` class is a wire-protocol break** between versions. `InitializeClientEvent` version-checks guard this.
- Fields must be public or `[JsonProperty]` (see `Ping/PingEvent.cs`). Other Unity types need a converter added next to the existing ones.
- All events for one tick are wrapped in a `GroupedEvent` (`ReplayService.cs:54`) so they arrive atomically; `ReadEventsFromIO` (`ReplayService.cs:281`) flattens them again.
- Two magic `type` values bypass the game entirely: `SetState` and `Heartbeat` (`TimberNet/TimberNetBase.cs:23-24`).

Wire details, map transfer, and Steam transport: [networking-and-connection.md](networking-and-connection.md).

## 3. Determinism

Two classes of change make the game deterministic:

1. **RNG isolation** – only gameplay may use `UnityEngine.Random` / Timberborn's `RandomNumberGenerator`. Music, sounds, animations, UI, and texture variants are rerouted to a separate `System.Random`. Also `Guid.NewGuid` is replaced so entity IDs match on both sides.
2. **Frame-rate independence** – `Time.time` is detoured to `ticks * fixedDeltaTime`; frame-based code paths (`MovementAnimator.Update`, `DayNightCycle.FluidSecondsPassedToday`, some `UpdateSingleton`s) are rewritten or moved onto the tick.

Everything lives in `DeterminismService.cs`; the reference is [determinism.md](determinism.md).

## Dependency injection and lifetime

Timberborn uses **Bindito** (see [timberborn-modding.md](timberborn-modding.md)). BeaverBuddies registers two configurators in `Plugin.cs`:

- `ReplayConfigurator` (`[Context("Game")]`, `Plugin.cs:22`): always binds the connection UI, settings, localization, and `MultiStartConfigurator`. Then **`if (EventIO.IsNull) return;`** (`Plugin.cs:45`): `ReplayService`, `TickingService`, `TickProgressService`, `DeterminismService`, `TickReplacerService`, `RehostingService`, `ReportingService`, `LateTickableBuffer`, `PingService`, and `DesyncDetecterService` exist **only in a co-op game**. A bug that only appears in co-op often lives in one of these.
- `ConnectionMenuConfigurator` (`[Context("MainMenu")]`, `Plugin.cs:66`): calls `SingletonManager.Reset()` and `EventIO.Reset()`, tearing down the session when you return to the main menu.

Static state must survive scene changes safely: `SingletonManager` (`SingletonManager.cs`) tracks `RegisteredSingleton`s and calls `IResettableSingleton.Reset()` on all of them from both configurators. If you add a static field that holds per-game state, reset it there.

`Plugin.StartMod` (`Plugin.cs:109`) applies patches in this order: `harmony.PatchAll()` → `AutomationEvent.ApplyAutomationPatches(harmony)` → `GameSaverSavePatcher.Install()` (MonoMod hook) → `TimeTimePatcher.Install()` (MonoMod native detour) → logs the `Player.log` path (`Plugin.cs:124`).

## Other services worth knowing

| Service | File | Role |
|---|---|---|
| `TickProgressService` | `TickProgressService.cs` | How far through the current tick a given entity's bucket is; drives smooth animation. |
| `TickReplacerService` | `TickReplacerService.cs` | `ITickableSingleton` that re-runs game logic removed from `UpdateSingleton` (currently `RecoveredGoodStackSpawner`). |
| `LateTickableBuffer` | `Fixes/WaterSourceFix.cs` | Buffers `WaterSource.Tick` calls and replays them after the parallel water simulation finishes. |
| `RehostingService` | `Connect/RehostingService.cs` | Save + relaunch as host after a desync. |
| `ReportingService` | `Reporting/ReportingService.cs` | Uploads desync trace + map to Airtable when `pat.properties` is present. |
| `PingService` | `Ping/` | Map pings; `PingEvent` carries `CreatorID = LocalPlayerID` so the originator does not double-play it. |
| `MultiStart/`, `Editor/` | | Multiple starting locations per player in new games and the map editor. Not sync-critical but heavily patched. |
| `GameSaveHelper` | `GameSaveHelper.cs` | Deterministic saves (timestamps zeroed) so save bytes hash identically. |
