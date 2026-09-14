# BeaverBuddies - Claude Code Instructions

## Project Overview

BeaverBuddies is a **multiplayer co-op mod** for the Unity game **Timberborn**. It uses **deterministic lockstep** synchronization — player actions are intercepted via Harmony patches, serialized as events, and replayed on both client and server at the same tick.

## Key Architecture

- **Entry point**: `BeaverBuddies/Plugin.cs` (implements `IModStarter`)
- **Core sync**: `BeaverBuddies/ReplayService.cs` — tick sync, event recording/replay
- **Event system**: `BeaverBuddies/Events/` — `ReplayEvent` subclasses capture all player actions
- **Networking**: `TimberNet/` — standalone TCP/Steam library (no Timberborn deps)
- **DI**: Bindito (Timberborn's framework), two contexts: `"Game"` and `"MainMenu"`
- **Patching**: HarmonyLib for intercepting game methods
- **Settings**: `BeaverBuddies/Settings.cs` via Timberborn `ModSettings`

## Common Bug Categories

### Desyncs
The most common bug type. A desync means client and server game state has diverged. Causes:
- **Missing event patches**: A player action modifies game state but isn't intercepted/replayed. Look in `Events/` for the pattern — Harmony `[HarmonyPrefix]` on game methods calling `ReplayEvent.DoPrefix()`.
- **Non-deterministic code**: Something produces different results on client vs server. Check `DeterminismService.cs` and `DeterminismPatcher.cs`.
- **Automation/scheduling**: Automated game systems (water, power, work assignments) running differently. See `Events/AutomationEvents.cs` and `Fixes/`.
- **Order-dependent operations**: HashSets, dictionaries, or parallel iteration producing different ordering. See `DesyncDetecter/`.

### Connection Issues
- TCP: `TimberNet/TimberServer.cs`, `TimberNet/TimberClient.cs`
- Steam: `BeaverBuddies/Steam/`
- Connection UI: `BeaverBuddies/Connect/`

### Crash/Exception Bugs
- Usually null references from entities that exist on one side but not the other
- Check entity lookups in `ReplayEvent.GetEntityComponent()` and `GetComponent<T>()`

## Debugging

- Enable `AlwaysTrace` in mod settings for debug logging
- `DesyncDetecterService.cs` hashes game state each tick for comparison
- `Settings.Debug` flag gates trace-level logging throughout the codebase
- Unity log path printed at startup (`Plugin.cs:124`)
- `FileIO` can replay saved event logs for reproduction

## File Conventions

- Event classes live in `Events/` and extend `ReplayEvent`
- Game bug fixes/workarounds go in `Fixes/`
- Harmony patches use `[HarmonyPatch]` attribute, prefix pattern returns `false` to skip original
- Localizations are CSV files in `Localizations/`

## Agents

### bugfixer

Use subagent_type `general-purpose` with this system context for bug investigation and fixing:

**Bugfixer agent prompt template:**
```
You are a bugfixer for BeaverBuddies, a Timberborn multiplayer co-op mod.

CONTEXT:
- Deterministic lockstep sync: all player actions must be intercepted via Harmony patches, serialized as ReplayEvents, and replayed identically on client+server
- Desyncs are the #1 bug type — they mean game state diverged between client and server
- The event interception pattern is in Events/ReplayEvent.cs (DoPrefix method)
- Desync detection hashes game state per-tick (DesyncDetecter/)
- Fixes for specific game interactions go in Fixes/

INVESTIGATION APPROACH:
1. Reproduce understanding: What exact player action or game state triggers the bug?
2. Trace the event flow: Is the action captured by a Harmony patch? Is the ReplayEvent correct?
3. Check determinism: Could this code path produce different results on client vs server?
4. Look for missing patches: Search for the game method being called — is there a [HarmonyPatch] for it?
5. Check entity references: Are entity IDs valid on both sides? Could an entity exist on one side but not the other?

KEY FILES TO CHECK:
- ReplayService.cs — core event replay loop
- Events/*.cs — all event types and their Harmony patches
- DesyncDetecter/ — state hashing and comparison
- Fixes/ — existing workarounds for reference
- DeterminismService.cs — non-determinism patches

When proposing fixes, follow existing patterns in the codebase. New events extend ReplayEvent, new patches use [HarmonyPrefix] with DoPrefix().
```
