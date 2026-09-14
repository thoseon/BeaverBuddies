# Logging reference

## Where logs go

Everything goes through `Plugin.Log` / `LogWarning` / `LogError` (`Plugin.cs:107-151`) → `UnityLogger` (`Util/Logging/UnityLogger.cs`) → `UnityEngine.Debug.*` → Timberborn's **`Player.log`**.

| Platform | Path |
|---|---|
| Windows | `C:\Users\<user>\AppData\LocalLow\Mechanistry\Timberborn\Player.log` |
| macOS | `~/Library/Logs/Mechanistry/Timberborn/Player.log` |

The exact path is printed at startup (`Plugin.cs:124`, `Application.consoleLogPath`). `Player-prev.log` is the previous run. **Restarting the game overwrites `Player.log`**, which is why the bug-report template says not to restart until the log is saved.

The in-game console is `Alt + ~` (Timberborn dev feature; see [timberborn-modding.md](timberborn-modding.md)).

### Mechanics

- Every BeaverBuddies message is prefixed `[HH-mm-ss.ff]` by `Plugin.GetWithDate`.
- `Plugin.Log` (info) is suppressed when the `SilenceLogging` setting is on (`Settings.VerboseLogging`). **Warnings and errors are never suppressed.**
- Network messages arrive via `TimberNetBase.OnLog += Plugin.Log` with their own prefix: `T{tick:D4} [{hash:X8}] : message` (`TimberNet/TimberNetBase.cs:78`). The hash is the running fold of all event JSON on that side; it is not compared automatically, but host and client values should match at the same tick.
- `Plugin.LogStackTrace()` dumps a `new StackTrace()`.

### Trace vs log

`DesyncDetecterService.Trace(message, warnIfNotDebug = true, skipStackTrack = false)` (`DesyncDetecter/DesyncDetecterService.cs:107`) is **not** a log call. It appends to the current tick's trace list, which the server ships to clients for comparison. It is a no-op unless `Settings.Debug` and warns `DesyncDetectorService.Trace called not in debug mode` if reached without the guard. See [debugging-desyncs.md](debugging-desyncs.md).

## Per-tick line

Emitted from `ReplayService.DoTick` (`ReplayService.cs:545-548`), every tick, both sides:

```
Tick 00123 IO done; Order hash: 1A2B3C4D; Move hash: DEADBEEF; Random s0: 0646C6D4
```

| Field | Source | Meaning |
|---|---|---|
| `Order hash` | `TEBPatcher.EntityUpdateHash` | Fold of the order in which entities ticked last tick. Divergence means entity ordering differs. |
| `Move hash` | `TEBPatcher.PositionHash` | Fold of walker positions at tick start. Divergence means movement desynced. |
| `Random s0` | `UnityEngine.Random.state.s0` | RNG state at tick start. Divergence means some RNG call happened on one side only, *before* this tick. |

Comparing this line across host and client logs at the same tick number is the fastest way to find the **first** desynced tick without tracing. Divergence in `Random s0` alone points to an RNG or missing-event problem; divergence in `Move hash` first points to movement/animation.

## Diagnostic strings, grouped

### Desync detection

| String | Where | Meaning |
|---|---|---|
| `Random state mismatch: XXXXXXXX != YYYYYYYY` | `ReplayService.cs:332` | Non-Debug detector: an event's recorded `randomS0Before` differs from local RNG state. Calls `HandleDesync()` immediately. |
| `Desync detected for tick N!` | `DesyncDetecterService.cs:202` | Debug detector: trace lists differ. Start of the trace report. |
| `========== Trace history ==========` / `========== Shared History for Desynced Tick N ==========` / `========== Desynced Trace ==========` / `---------- My Trace ----------` / `---------- Other Trace ----------` / `========== Desynced Log End ==========` | `DesyncDetecterService.cs:200-226` | Report sections. Paste into https://thomaswp.github.io/BBDesyncViewer/. |
| `Verifying future tick! N > M` | `DesyncDetecterService.cs:153` | Client received traces for a tick it has not reached. |
| `Ticks cannot decrease!` | `DesyncDetecterService.cs:91` | Tick counter went backwards. |
| `Attempting to verify already deleted tick N` | `DesyncDetecterService.cs:163` | Trace window (10 ticks) already pruned. |
| `DesyncDetectorService.Trace called not in debug mode` | `DesyncDetecterService.cs:107+` | A trace call site lacks `if (!Settings.Debug) return;`. |

### Determinism

| String | Where | Meaning |
|---|---|---|
| `Unknown random called outside of tick` + stack | `DeterminismService.cs` ~310 | RNG used outside a tick with no classification. If the stack is gameplay, that is a desync source. |
| `Tick RNG; s0 before: …; Last entity: …` | `DeterminismService.cs` ~225 | Trace, Debug only. Gameplay RNG call during a tick. |
| `Load RNG; s0 before: …` | `DeterminismService.cs` ~197 | Trace, Debug only. RNG call during load. |
| `Generating new GUID: …` | `DeterminismService.cs` ~816 | Trace, Debug only. |
| `Duplicate GUID … detected, generating new GUID. Attempt #n.` | `DeterminismService.cs` ~860 | Warning after tick 0. Expected occasionally during preload. |
| `Finishing full tick - this probably is bad!` | `DeterminismService.cs` ~718 | Vanilla forced a full tick outside a save. Investigate what triggered it. |

### Tick loop and replay

| String | Where | Meaning |
|---|---|---|
| `Client trying to tick before receiving Heartbeat at tick: N` | `ReplayService.cs:663` | Client is ahead of the server's events. Usually network lag, not a desync. |
| `Warning, replaying events when bucket != 0: N` | `ReplayService.cs:296` | Events replayed mid-tick. Ordering bug. |
| `Event past time: A < B` | `ReplayService.cs:318` | Received an event for a tick already passed. |
| `Failed to replay event: <exception>` | `ReplayService.cs:350` | `Replay()` threw. The event is dropped on this side only, which **will** desync. |
| `RecordEvent: {json}` | `ReplayService.cs:263` | Every recorded event, with payload. |
| `Searching for unregistered singleton T; found = …` | `ReplayService.cs:251` | `Replay()` asked for a singleton not added in the `ReplayService` constructor. Add it. |
| `Doing: <ToActionString()>` | `Events/ReplayEvent.cs:127` | An event was intercepted. |

### Entity resolution (crash precursors)

| String | Where | Meaning |
|---|---|---|
| `Could not parse guid: …` | `Events/ReplayEvent.cs:46+` | Malformed entity ID in an event. |
| `Could not find entity: …` | `Events/ReplayEvent.cs:46+` | The entity exists on the sender but not here. Classic one-side-only entity. |
| `Could not find component T on entity …` | `Events/ReplayEvent.cs:61+` | Entity exists but lacks the component; wrong entity or preview object. |
| `Could not find building prefab: …` | `Events/ReplayEvent.cs:78+` | Template name mismatch (game update or mod difference). |
| `GoodDistributionSetting without district!!` | `Events/BatchEvents.cs:300` | The district back-reference swap failed. |
| `Unknown distributor type: …` | `Events/BatchEvents.cs:34,54` | New distributor template not mapped. |
| `No MethodInfo for: …` / `Argument count mismatch for …` | `Events/AutomationEvents.cs` ~32-47 | Automation event key does not match current game API. |
| `missing entity component!` | `Fixes/AnimationFixes.cs` | Animator update on an entity without `EntityComponent`. |

### Connection

| String | Where | Meaning |
|---|---|---|
| `Client connection timed out` (`ConnectionFailureException`) | `TimberNet/TimberClient.cs:11,47` | TCP connect exceeded 3 s. |
| `Sending map with length …` / `Sent map with length … and Hash: XXXXXXXX` | `TimberNet/TimberServer.cs` ~155 | Server side of map transfer. |
| `Received map with length … and Hash: XXXXXXXX` | `TimberNet/TimberNetBase.cs` ~325 | Client side. **Hash must equal the server's.** |
| `Error accepting client.` | `TimberServer.cs:78` | Accept loop exception. |
| `Warning! Missing client!` | `TimberServer.cs:142` | Broadcast to a client that vanished. |
| `Error sending event: …` / `Error receiving event: …` | `TimberServer.cs:204`, `TimberNetBase.cs:342` | Socket errors. |
| `Received message of length 0; aborting listen` | `TimberNetBase.cs:251` | Peer closed or server sent an error message. |
| `The Host has already started the game…` | `IO/ServerEventIO.cs:105` | Late joiner rejected. |
| `Failed to start server` | `IO/ServerEventIO.cs:77` | Listener could not bind (port in use?). |
| `Client creation failed.` / `Could not resolve hostname: …` | `Connect/ClientConnectionService.cs:102,263` | Client-side connect failures. |
| `Failed to create lobby: …` | `Steam/SteamListener.cs:59` | Steam lobby creation failed. |
| `SteamSocket read N bytes, but M bytes were left over. This is probably a bug!` | `Steam/SteamSocket.cs:85` | Packet framing broke. |
| `Error occured while saving: …` / `Failed to rehost: …` | `Connect/RehostingService.cs:85,91` | Rehost flow. |
| `Closing EventIO...` / `Success!` | `IO/EventIO.cs` | Session teardown. |

### Settings and startup

| String | Where | Meaning |
|---|---|---|
| `BeaverBuddies v1.x.y is loaded!` | `Plugin.cs:113` | Mod started. Version check. |
| `No access token found. Reporting will be disabled.` | `Reporting/ReportingService.cs` ~36 | `pat.properties` absent; "Post Bug Report" hidden. |
| `Recording to file` / `Unknown save name` / `Failed to load json: …` | `IO/FileIO.cs` | Offline record/replay path. |

## Comparing host and client logs

1. Rename per the issue template: `server.txt` and `client.txt`.
2. Find the first tick where `Tick NNNNN IO done` lines differ. Compare `Random s0` first, then `Move hash`, then `Order hash`.
3. With tracing on, jump to `Desync detected for tick` in the **client** log (the client is the side that verifies).
4. Look *above* the first divergence on both sides for `Unknown random called outside of tick`, `Failed to replay event`, `Could not find entity`, and `Warning, replaying events when bucket != 0`.
5. Check both logs for the same `Received map … Hash` / `Sent map … Hash` value.
