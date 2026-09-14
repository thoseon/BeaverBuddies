# Build, deploy, and test

## Solution layout

`BeaverBuddies.sln` has three projects:

| Project | Target | Purpose |
|---|---|---|
| `BeaverBuddies/` | `netstandard2.1` | The mod. |
| `TimberNet/` | `netstandard2.1`, nullable enabled | Networking library, no game dependencies. |
| `Inspector/` | `netcoreapp3.1` console | Dev tooling: ILSpy scans of game DLLs (`Program.cs`), localization sync (`UpdateLocalizations.cs`, what `Main` runs), manual `ConcurrentQueueWithWait` and HashSet-order experiments. |

Configurations: `Debug`, `Release`, `Debug Steam`, `Release Steam`. The Steam ones define `IS_STEAM`, which compiles the `Steam/` overlay code. v1.7.3 was released solely because a build lacked it.

## Environment

`BeaverBuddies/env.props` is git-ignored and auto-copied from `env.props.windows-template` / `env.props.unix-template` on first build (`EnsureEnvProps` target). All paths must end with a slash:

| Property | Points at |
|---|---|
| `TimberbornDataPath` | `<install>\Timberborn_Data\` |
| `DocumentsPath` | Documents folder (Windows template reads the registry so relocated Documents work) |
| `HarmonyPath` | `<Documents>\Timberborn\Mods\Harmony_2.4.1\` (the Harmony mod's folder; adjust the version) |
| `ModSettingsPath` | `<Documents>\Timberborn\Mods\modsettings-ey0f\version-1.0\Scripts\` |

`CheckEnv` hard-errors if any directory is missing; those are the "directory not found" errors the README mentions. Both Harmony and ModSettings must be installed (Workshop or mod.io) before building.

References: `Timberborn.*.dll`, `UnityEngine.*.dll`, `Unity.InputSystem.dll` with `Publicize="true"` (BepInEx.AssemblyPublicizer, NuGet feed in `BeaverBuddies/NuGet.Config`), `Bindito.*`, `com.rlabrecque.steamworks.net.dll`, `0Harmony.dll`, `ModSettings.*.dll`. Packages: `MonoMod.Core`, `MonoMod.RuntimeDetour`, `System.Collections.Immutable`, `Newtonsoft.Json` (via TimberNet).

## Build == deploy

```
dotnet build BeaverBuddies/BeaverBuddies.csproj -c Debug
```
or Ctrl+Shift+B in Visual Studio / Rider.

The `PostBuild` target **deletes and recreates** `<Documents>\Timberborn\Mods\BeaverBuddies\version-1.0\`, copies the build output plus `Localizations\`, `KeyBindings\`, `KeyBindingGroups\`, and drops `thumbnail.png` + `workshop_data.json` one level up. There is no separate install step. In the in-game mod list, the local build is the BeaverBuddies entry with a **folder icon**; enable it and disable the Workshop copy.

Optional `BeaverBuddies/pat.properties` (git-ignored, Airtable token) is embedded as a resource and enables the "Post Bug Report" button. Without it the log says `No access token found. Reporting will be disabled.`

## Versioning and release

- Bump `<Version>` in `BeaverBuddies/BeaverBuddies.csproj` and `Version` in `BeaverBuddies/manifest.json`.
- Add a user-facing entry at the top of `BeaverBuddies/changelog.txt` (shown in-game on version change by `Help/ChangeLogService.cs`).
- Localizations: 15 CSVs in `BeaverBuddies/Localizations/` (`ID,Text,Comment`; keys `BeaverBuddies.<Area>.<Key>`). Add the English key first, then run `Inspector` (`UpdateLocalizations.Go()`) to propagate; `Doc/TranslationPrompt.txt` is the prompt used for translations.
- Workshop/mod.io upload is done from the in-game mod manager; `workshop_data.json` holds the Steam item ID.

## Tests

There are **no automated tests** and no CI. `Inspector` references MSTest but has no runner; its test-like classes are manual console routines. Verification is manual with two game instances.

### Manual verification protocol

1. Build and confirm the folder-icon mod entry is selected on both instances (second key, VM, or a friend).
2. Enable `AlwaysTrace` in Mods → BeaverBuddies settings on **both** instances.
3. Host: Load game → "Host co-op game" on the reproduction save. Do not unpause.
4. Client: "Join co-op game" → `127.0.0.1` (same machine) or the host IP. Wait for load.
5. Host: confirm the client appears in the player list, start, unpause.
6. Perform the reproduction. Watch both `Player.log`s for `Desync detected` / `Random state mismatch`.
7. For a fix: reproduce first on clean `master`, then with the change; run 5+ minutes clean.
8. Also sanity-check: a plain single-player game still works (many patches early-return on `EventIO.IsNull`), and connecting via both TCP and Steam if you touched `IO/` or `TimberNet/`.

Running two instances on one machine: launch the second copy with a different Steam account or a non-Steam build; hosting on `127.0.0.1:25565` needs no port forwarding.

### After a Timberborn update

- Rebuild; fix compile errors from renamed/removed game APIs (publicized references make these loud).
- Grep `[ManualMethodOverwrite]` and diff each copied body against the new decompiled method.
- Check the automation method list (`Events/AutomationEvents.cs` ~95-183) for renamed methods; `No MethodInfo for:` at runtime means a key is stale.
- Re-run the `ReflectionUtils` probes (`Plugin.cs:87-96`, commented) and `Inspector` scans to refresh the audits in `BeaverBuddies/Doc/`.
- Update `MinimumGameVersion` in `manifest.json` and `<TimberbornTargetVersion>` (output folder name) if needed.

## Contributing

Wiki: https://github.com/thomaswp/BeaverBuddies/wiki/Contributing. Pick an issue, fix it, open a PR; ask on the Discord mod thread (link in `README.md`) or in the issue. Live-coding example of adding an event: https://www.youtube.com/watch?v=xD7x8R580N0.
