# BeaverBuddies - Claude Code Instructions

BeaverBuddies is a **multiplayer co-op mod** for the Unity game **Timberborn**, built on **deterministic lockstep**: all players load the same save, every player action is intercepted by a Harmony prefix and serialised as a `ReplayEvent`, the Host broadcasts events per tick, and every machine replays them at the same tick. The game itself is patched to be deterministic (RNG isolation, tick-based time). Any divergence is a **desync**, the number-one bug class.

This file is the entry point. Deep detail lives in `docs/` (index: `docs/README.md`). Both project wikis are already incorporated there; do not fetch them.

## Documentation map

| Task | Open first |
|---|---|
| Any desync | `docs/debugging-desyncs.md` |
| A player action does not sync / adding an event | `docs/events-and-patches.md` |
| RNG, GUIDs, `Time.time`, movement/animation, parallel water | `docs/determinism.md` |
| Cannot connect, map transfer, Steam vs TCP, user setup | `docs/networking-and-connection.md` |
| What does this log line mean, where is `Player.log` | `docs/logging-reference.md` |
| Timberborn API: Bindito DI, entities, dev/debug mode, UI Toolkit, decompiling | `docs/timberborn-modding.md` |
| Has this been seen before; reference fixes; open TODOs | `docs/known-issues-and-history.md` |
| Build, deploy, versioning, manual verification, after a game update | `docs/build-and-test.md` |
| How the whole thing fits together | `docs/architecture.md` |
| Movement internals, RNG audit, parallel singletons, UI fragments | `BeaverBuddies/Doc/` (see `docs/README.md`) |

## Repository map

```
BeaverBuddies/                  the mod (netstandard2.1)
  Plugin.cs                     IModStarter entry; two Bindito configurators ("Game", "MainMenu"); applies patches; logs Player.log path
  ReplayService.cs              CORE: tick counter, event record/replay, heartbeats, speed, desync handling; TickingService (replaces bucket loop)
  DeterminismService.cs         RNG gameplay-vs-cosmetic routing, Guid/Time.time/entity-order patches, TEBPatcher; maintainer's issue log (lines 51-116)
  Settings.cs                   ModSettings: AlwaysTrace (= Settings.Debug), SilenceLogging, DefaultPort, EnableSteamConnection, PauseReduction, ping
  SingletonManager.cs           static singleton map; IResettableSingleton.Reset() on scene change
  TickProgressService.cs        per-entity progress through the tick (smooth animation)
  TickReplacerService.cs        re-runs game logic moved off UpdateSingleton onto the tick
  GameSaveHelper.cs             deterministic saves (timestamps zeroed)
  Attributes.cs                 [ManualMethodOverwrite] marker for copied vanilla code
  Events/                       every ReplayEvent + its Harmony prefix (ReplayEvent.cs = base + DoPrefix)
  DesyncDetecter/               DesyncDetecterService (per-tick trace lists, VerifyTraces) + DesyncPatches (all trace patches)
  Fixes/                        workarounds: AnimationFixes, DistrictBuildingsFix, WaterSourceFix, WaterWheelFix, TickOnlyArrayFix
  IO/                           EventIO interface; ServerEventIO / ClientEventIO; FileIO (JsonSettings, offline record/replay)
  Connect/                      host/join UI, map transfer, rehosting
  Steam/                        Steam P2P transport and overlay (IS_STEAM builds)
  Ping/  Help/  Reporting/      map pings; first-timer + changelog dialogs; desync report upload
  MultiStart/  Editor/          multiple starting locations (new game, map editor)
  Util/                         logging, reflection probes, localization, link helpers
  Localizations/                15 CSVs, keys BeaverBuddies.<Area>.<Key>
  Doc/                          developer notes and ILSpy audits (Movement.md, ClassesWithRandom.txt, ...)
TimberNet/                      standalone TCP/Steam protocol library (no Timberborn deps)
Inspector/                      dev console app: ILSpy scans, localization sync; no real tests
docs/                           this documentation set
```

Dead or parked code, do not extend: `DeterminismPatcher.cs` (never called), `Fixes/SimulationUpdateFix.cs` (commented out), `Fixes/TestingStrategies_Scrap.cs` (not compiled; preserved bisection method), `ReplayConfig.cs` (`[Obsolete]`), `TickWatcherService.cs`.

## Timberborn API reference

Decompiled Timberborn sources are in `_decompiled/`, one folder per assembly,
with namespaces mirrored as subdirectories.

**Look things up there. Do not guess Timberborn class names, properties, method
signatures, or namespaces from memory** — the game is closed-source and not in
your training data. A plausible-looking identifier that does not exist costs a
full build-and-restart cycle to discover.

Read-only reference: never build, edit, or stage anything under `_decompiled/`.
It is excluded via `.git/info/exclude` and must not appear in any diff or commit.

Finding things:

- `BeaverBuddies/Doc/UIFragments.txt` lists every `IEntityPanelFragment`
  implementation with its fully qualified name. The namespace tells you which
  assembly folder to open. Start here for any UI-related work.
- UI controls are reusable fragments and often do not live in the fragment named
  after the building they appear on. Searching by building name will mislead you.
- Grep across the export:
  `Select-String -Path '_decompiled\*\*.cs' -Pattern 'class WonderFragment' -List`

The export is a snapshot. If the game has been updated since it was generated,
re-run `Export-TimberbornSource.ps1 -Force`.

## Core flow in ten lines

1. Tick entry: `TickableBucketService.TickBuckets` is replaced (`ReplayService.cs:753`, `[ManualMethodOverwrite]`) by `TickingService.TickBuckets` (710) → `TickReplayServiceOrNextBucket` (685) → `ReplayService.DoTick` (511) runs as pseudo-bucket 0.
2. `DoTick`: flush traces → `ticksSinceLoad++` → server enqueues `HeartbeatEvent` → `DoTickIO` (427) = `ReplayEvents()` + `SendEvents()` → `UpdateSpeed`.
3. Client gate: `IsReadyToStartTick` (121) waits for `io.HasEventsForTick(tick+1)`.
4. Interception: `ReplayEvent.DoPrefix` (`Events/ReplayEvent.cs:112`) records the event and returns `EventIO.ShouldPlayPatchedEvents`: **host `true` (run now + broadcast), client `false` (swallow, wait for echo)**. Returns `true` early when replaying, not loaded, desynced, or the event factory returns null.
5. One `EventIO` per session: `ServerEventIO` (records replays, heartbeats, `QueuePlay`) vs `ClientEventIO` (no record, `Send`). `EventIO.IsNull` = not in co-op; co-op services are bound only when it is non-null (`Plugin.cs:45`).
6. Determinism: `ShouldFreezeSeed` decides gameplay vs cosmetic RNG; `Guid.NewGuid` and `Time.time` are patched; `TEBPatcher` snaps position and completes rotation before each entity ticks.
7. Detection: tracing off → per-event `randomS0Before` check (`ReplayService.cs:326`); tracing on → server ships trace lists, client diffs them in `VerifyTraces`. Either → `HandleDesync` (357) → dialog, pause, IO reset.
8. Serialization: Newtonsoft with `TypeNameHandling.All` (`IO/FileIO.cs:65`). No event registry; **class names are the wire format**.
9. Start state: map bytes streamed to clients; map hash = save name = RNG seed. Late join is impossible.
10. Speed: `SpeedChangePatcher.SetSpeedSilentlyNow` for internal changes; clients never auto-freeze for dialogs.

## Bug taxonomy and first moves

**Desync** (state diverged)
- *Missing or incomplete event*: an action ran on one side only, or `Replay()` differs from the original. Grep `Events/` for a `[HarmonyPatch]` on the game method; prefer adding UI-only `BaseComponent` setters to the automation list in `Events/AutomationEvents.cs`. Replay must call the authoritative method, not re-post a UI event.
- *Non-determinism*: cosmetic code using gameplay RNG, gameplay RNG outside a tick, frame time leaking in, parallel water writes. `DeterminismService.cs`, `Fixes/AnimationFixes.cs`, `Fixes/WaterSourceFix.cs`, `Doc/ClassesWithRandom.txt`, `Doc/Movement.md`.
- *Order/timing*: entity created or ticked in a different order; `Adding: <guid> at index` or `Order hash` diverges. `EntityService.Instantiate` patch, `GuidPatcher`. HashSet enumeration order is **ruled out** (`DeterminismService.cs:91-95`).
- *Lag-induced*: only with heavy tracing on. Make traces cheaper.

**Crash / exception in co-op only**: usually an entity that exists on one side. Look for `Could not find entity` / `Could not find component`, preview objects registering side effects (`Fixes/DistrictBuildingsFix.cs`), `Failed to replay event`.

**Connection**: TCP `TimberNet/TimberServer.cs`, `TimberClient.cs` (3 s timeout, port 25565); Steam `BeaverBuddies/Steam/`; UI `Connect/`. First check both logs show the same map `Hash`.

**Game update broke the mod**: grep `[ManualMethodOverwrite]` (16 sites) and diff against decompiled vanilla; check `No MethodInfo for:` from the automation list.

## Desync checklist (short form)

Full version with decision tree: `docs/debugging-desyncs.md`.

1. Two instances, `AlwaysTrace` on **both**, a save that reproduces within a minute.
2. Do not reload. Client `Player.log` → `Desync detected for tick N!` → paste into https://thomaswp.github.io/BBDesyncViewer/.
3. First red/blue row in the desynced tick is the lead; its stack is a call path, not an error. If it is `Tick RNG`, walk up to the first one-sided row.
4. Classify with the decision tree; check logs for `Unknown random called outside of tick`, `Failed to replay event`, `Could not find entity`, `bucket != 0`.
5. Add traces in `DesyncDetecter/DesyncPatches.cs` up the stack (binary search). Always `if (!Settings.Debug) return;` first; `skipStackTrack: true` above 10 calls/tick; nothing above 50/tick.
6. Bisect switches: `NO_SMOOTH_ANIMATION` (`Fixes/AnimationFixes.cs:1`), `NO_RANDOM` (`DeterminismService.cs:5`), `NO_PARALLEL` (`DeterminismService.cs:2`), `ONE_TICK_PER_UPDATE` (`ReplayService.cs:3`). A switch removing the desync is a hint, not proof.
7. Fix following a reference fix in `docs/known-issues-and-history.md`. Reproduce on clean master, then 5+ clean minutes with the fix. Keep useful traces in the PR.

## Log cheat-sheet

`Player.log`: `%USERPROFILE%\AppData\LocalLow\Mechanistry\Timberborn\` (path also printed at startup, `Plugin.cs:124`). Restarting the game overwrites it. Full table: `docs/logging-reference.md`.

| String | Meaning |
|---|---|
| `Tick 00123 IO done; Order hash: …; Move hash: …; Random s0: …` | Per-tick state summary, both sides. First differing tick = first desynced tick. |
| `Desync detected for tick N!` … `========== Desynced Log End ==========` | Trace report (tracing on). |
| `Random state mismatch: X != Y` | Desync detector (tracing off). |
| `Unknown random called outside of tick` | Unclassified RNG call with stack. Strong lead if gameplay. |
| `Client trying to tick before receiving Heartbeat at tick: N` | Client ahead of server events; lag, not desync. |
| `Failed to replay event: …` | `Replay()` threw on this side only. Will desync. |
| `Could not find entity: …` / `Could not find component T on entity …` | Entity exists on one side only. |
| `Finishing full tick - this probably is bad!` | Vanilla forced a full tick outside a save. |
| `Duplicate GUID … Attempt #n` | GUID collision; expected rarely at preload. |
| `Received map … Hash: X` / `Sent map … Hash: X` | Must match on both sides. |

## Conventions

- New events: subclass `ReplayEvent` in the matching `Events/*.cs`; public fields; `Replay(IReplayContext)`; `ToActionString()`; prefix via `DoPrefix`/`DoEntityPrefix`; add an idempotence guard. Singletons used in `Replay` go in the `ReplayService` constructor + `AddSingleton`.
- Prefer the automation list (`Events/AutomationEvents.cs` ~95-183) for UI-only setters on a `BaseComponent`.
- Never rename or move an event class without a version bump (wire format).
- Trace calls: `if (!Settings.Debug) return;` first, null-safe, cheap.
- Copied vanilla code gets `[ManualMethodOverwrite]` and a dated comment containing the original.
- Static per-game state is reset through `IResettableSingleton`.
- Internal speed changes use `SetSpeedSilentlyNow`.
- Game bug workarounds go in `Fixes/`; `HarmonyFinalizer` is the idiom for "vanilla throws where multiplayer timing makes it legal".
- Localizations: add the English key to `Localizations/enUS_BeaverBuddie.csv`, propagate with `Inspector`.
- No automated tests. Verification is two game instances (`docs/build-and-test.md`). Say what was verified by reading versus what needs a manual run.
- `dotnet build` deploys to `Documents/Timberborn/Mods/BeaverBuddies/version-1.0/` and wipes that folder first.

## Debug settings

| Setting (Mods → BeaverBuddies) | Effect |
|---|---|
| `AlwaysTrace` | = `Settings.Debug`. Enables trace patches and trace exchange; disables the cheap s0 check. Must match on host and client. |
| "Enable Logging" button in the desync dialog | Sets `Settings.TemporarilyDebug` until restart. |
| `SilenceLogging` | Suppresses info logs only. |
| `DefaultPort`, `EnableSteamConnection`, `FriendsCanJoinSteamGame` | Transport. |
| `PauseReduction` | Off / MenuOnly / NeverAutoPause; gates the speed-lock patches. |

## Agents

Use the `bugfixer` subagent (`.claude/agents/bugfixer.md`) for bug investigation and fixing. It reads this file and the relevant `docs/` pages, follows the investigation protocol, and reports root cause, evidence, fix, and verification steps.
