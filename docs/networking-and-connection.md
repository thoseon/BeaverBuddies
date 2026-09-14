# Networking and connection

`TimberNet/` is a standalone TCP/Steam library with no Timberborn dependencies. `BeaverBuddies/IO/` adapts it to `ReplayEvent`s; `BeaverBuddies/Connect/` and `BeaverBuddies/Steam/` handle the UI and transports.

## Wire protocol (`TimberNet/TimberNetBase.cs`)

- Constants (~20-25): `HEADER_SIZE = 4`, `TICKS_KEY = "ticksSinceLoad"`, `TYPE_KEY = "type"`, `SET_STATE_EVENT = "SetState"`, `HEARTBEAT_EVENT = "Heartbeat"`, `MAX_BUFFER_SIZE = 32K`.
- Each message: **4-byte big-endian length** followed by a **gzip-compressed JSON object** (`MessageToBuffer` → `CompressionUtils.Compress`).
- Sends are chunked and throttled (`SendDataWithLength`, sleeps `MaxChunkSize*1000/MaxBytesPerSecond` between chunks) because Steam's buffer is small.
- A **running hash** folds every event's JSON (`AddEventToHash`, `CombineHash = h1*31 + h2`). `SetState` overwrites it wholesale. Log lines carry it as `T{tick:D4} [{hash:X8}]`.
- `ReadEvents(tick)` pops events with tick ≤ current and filters out `SetState`/`Heartbeat` before the game sees them. `HasEventsForTick` gates client ticking. `TicksBehind` = last received tick − current tick, drives catch-up speed.
- Read loop `StartListening(client, isClient)`: message #0 on a client is always the map. A zero-length first message means an error string follows (`ReadErrorMessage` → `OnError`).

## Server (`TimberNet/TimberServer.cs`)

Per accepted client, in order:

1. `SendMap` – logs `Sending map with length …` then `Sent map with length … and Hash: XXXXXXXX`.
2. `StartQueuing` – events broadcast while the map is in flight are buffered so nothing is lost.
3. `SendState` – `{ ticksSinceLoad: 0, type: "SetState", hash }`.
4. Broadcast the init event (`InitializeClientEvent` via `initEventProvider`).
5. `FinishQueuing`, then `StartListening` (blocking).

`StopAcceptingClients(msg)` sets an error message; later joiners get a zero-length message plus the text (`The Host has already started the game…`). Called from `ReplayService.DoTick` at tick 1. `SendHeartbeat` is called every tick by `ServerEventIO`.

Known suspicion (comment ~66-68): the accept loop's `catch { continue; }` may hang on dropped connections.

## Client (`TimberNet/TimberClient.cs`)

- `Start()` waits **3000 ms** on `ConnectAsync` and throws `ConnectionFailureException("Client connection timed out")`.
- User events are sent, never applied locally (`DoUserInitiatedEvent`).
- `ShouldTick` requires at least one queued event.
- Every received event is folded into the hash.

## Transports

- `ISocketStream` / `ISocketListener` abstract TCP (`TCPClientWrapper`, `TCPListenerWrapper`) and Steam (`Steam/SteamSocket.cs`, `Steam/SteamListener.cs`, `Steam/SteamPacketListener.cs`).
- `MultiSocketListener` listens on TCP and Steam simultaneously; `ServerEventIO.Start` adds the Steam listener only when `Settings.EnableSteam`.
- Steam: `SteamSocket` chunks at 8 KB (throws `IOException` above), throttles to 128 KB/s, and logs `SteamSocket read N bytes, but M bytes were left over. This is probably a bug!` if packet framing breaks. Lobby joinability follows `Settings.LobbyJoinable`. `SteamOverlayConnectionService` handles overlay invites; the whole Steam layer is compiled only under `IS_STEAM` (the "Debug Steam"/"Release Steam" configurations).
- Default TCP port **25565** (`Settings.DefaultPort`). The obsolete `ReplayConfig.json` mentioned in older wiki text is superseded by mod settings.

## The BeaverBuddies IO layer (`BeaverBuddies/IO/`)

| File | Role |
|---|---|
| `EventIO.cs` | Interface + static singleton (`EventIO.Get/Set/Reset/IsNull`), `UserEventBehavior` enum, `ShouldPlayPatchedEvents`, `SkipRecording`. `Reset()` logs `Closing EventIO...` / `Success!`. |
| `NetIOBase.cs` | JSON ↔ `ReplayEvent` bridge (`ToEvent`, `WriteEvents` via `JObject.Parse(JsonSettings.Serialize(e))`, flagged as wasteful); Steam packet pump registration. |
| `ServerEventIO.cs` | Host: records replayed events, sends heartbeats, `QueuePlay`. `Start(byte[] mapBytes)` builds the listeners; `Failed to start server` on exception. Keeps the map bytes in memory for later joiners. Comment at 14-22 explains why late join is impossible. |
| `ClientEventIO.cs` | Client: no recording, no heartbeats, `Send`. `Create` returns **null** on failure; `OnError` → `Plugin.LogError` + `FailedToConnect`. |
| `FileIO.cs` | `JsonSettings` (the serializer for both file and network), `Vector3`/`Vector3Int` converters, offline `FileWriteIO`/`FileReadIO` (unbound; see [debugging-desyncs.md](debugging-desyncs.md#offline-recording-and-replay)). |

## Map transfer, end to end

1. Host picks a save in `LoadGameBox`. `ServerHostingUtils.LoadAndHost` (`Connect/ServerHostingUtils.cs` ~141) reads the raw `.timber` bytes (`GetMapBtyes` ~123) → `ServerEventIO.Start(data)` → `EventIO.Set(io)`.
2. The host dialog polls `io.NetBase.GetConnectedClients()` each frame and lists joined players.
3. On "Start Game" the host calls `DeterminismService.InitGameStartState(data)` (RNG seed = hash of map bytes) then `sceneLoader.StartSaveGame`.
4. For each client, `TimberServer.SendMap` streams the bytes.
5. Client: `TimberNetBase.ReceiveFile` reads them, folds them into the hash, logs `Received map with length … and Hash: XXXXXXXX`, then `ClientConnectionService.LoadMap` (`Connect/ClientConnectionService.cs` ~179) does `SingletonManager.Reset()`, writes the bytes as a save named `<hash:X8>` under settlement **"Online Games"**, calls the same `InitGameStartState(mapBytes)`, and `StartSaveGame`.

**The map hash is both the save name and the RNG seed.** If host and client log different hashes, stop: nothing downstream can sync.

## Rehosting after a desync

`Connect/RehostingService.cs`: the host saves (TODO at ~52: should respect `IAutosaveBlocker`s) and relaunches as host; clients reconnect. Failures log `Error occured while saving: …` / `Failed to rehost: …`.

## Connection-failure signals

See the Connection table in [logging-reference.md](logging-reference.md#connection). The user-facing dialog keys are `BeaverBuddies.JoinCoopGame.Error.InvalidFormat` / `.InvalidAddress` / `.CouldNotConnect` (`Localizations/enUS_BeaverBuddie.csv`).

## User-side setup (for triaging reports)

Condensed from the wiki's [Installation and Running](https://github.com/thomaswp/BeaverBuddies/wiki/Installation-and-Running) and [Port Forwarding Guide](https://github.com/thomaswp/BeaverBuddies/wiki/Port-Forwarding-Guide):

- Install via Steam Workshop (preferred) or mod.io / Mod Manager. Requires the **Harmony** mod and **ModSettings ≥ 0.7.2.0** (`manifest.json`). Other mods should be disabled; they are a leading desync cause.
- Two network modes: **Steam P2P** (beta, slower, both on Steam, needs P2P-friendly NAT) or **direct TCP** with the host forwarding **port 25565** TCP (the guide says TCP and UDP) to their LAN IP. Hamachi-style VPNs are a workaround.
- Host: find public IP, Load game → "Host co-op game", wait until the client appears in the player list, then start and unpause.
- Client: "Join co-op game" with the host's IP, or accept the Steam invite; the save downloads and loads automatically; tell the host when loaded.
- Troubleshooting: wrong or stale IP; port not actually open (check with an open-port tester while hosting); host unpaused before the client joined (late join impossible); old save never played with the mod; another mod enabled.
- Bug reports: do not restart either game before saving `Player.log` (it is overwritten). Provide the host's save (`.timber` renamed to `.zip`), `server.txt`/`client.txt` logs, both OS/Timberborn/BeaverBuddies versions. Template: `.github/ISSUE_TEMPLATE/bug_report.md`.
