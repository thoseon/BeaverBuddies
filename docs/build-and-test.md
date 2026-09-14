# Build, deploy, and test

## Solution layout

`BeaverBuddies.sln` has three projects:

| Project | Target | Purpose |
|---|---|---|
| `BeaverBuddies/` | `netstandard2.1` | The mod. |
| `TimberNet/` | `netstandard2.1`, nullable enabled | Networking library, no game dependencies. |
| `Inspector/` | `netcoreapp3.1` console | Dev tooling: ILSpy scans of game DLLs (`Program.cs`), localization sync (`UpdateLocalizations.cs`, what `Main` runs), manual `ConcurrentQueueWithWait` and HashSet-order experiments. |

Configurations: `Debug`, `Release`, `Debug Steam`, `Release Steam`. The Steam ones define `IS_STEAM`, which compiles the `Steam/` overlay code. v1.7.3 was released solely because a build lacked it.

## Prerequisites

The mod compiles against the game's own assemblies and against two other mods' DLLs, so the build machine needs all three installed. Nothing is vendored.

| Need | Why | Where it usually is |
|---|---|---|
| Timberborn (1.0 or newer) | `Timberborn.*.dll`, `UnityEngine.*.dll`, `Bindito.*`, `Unity.InputSystem.dll`, `com.rlabrecque.steamworks.net.dll` are referenced from `Timberborn_Data\Managed\` and publicized at build time. | Steam: `<steam>\steamapps\common\Timberborn\Timberborn_Data\`. macOS: `.../Timberborn.app/Contents/Resources/Data/`. Steam libraries can live on any drive; check the game's Properties → Installed Files. |
| Harmony mod (`Id: Harmony`, 2.4.x) | `0Harmony.dll` is referenced, not bundled (since v1.7.0). | Workshop item `3284904751` → `<steam>\steamapps\workshop\content\1062090\3284904751\`. mod.io / manual: `Documents\Timberborn\Mods\Harmony_2.4.1\`. |
| Mod Settings mod (`Id: eMka.ModSettings`, ≥ 0.7.2) | `ModSettings.*.dll` are referenced. | Workshop item `3283831040` → `...\content\1062090\3283831040\version-1.0\Scripts\` (pick the `version-` folder matching the game's major version). mod.io / manual: `Documents\Timberborn\Mods\modsettings-ey0f\version-1.0\Scripts\`. |
| .NET SDK 6 or newer | Projects target `netstandard2.1`; `Inspector/` targets `netcoreapp3.1` (builds on SDK 10 with an end-of-support warning, only matters if you run it). | `dotnet --version`. |
| nuget.org as a NuGet source | `BeaverBuddies/NuGet.Config` only **adds** the BepInEx feed (for the assembly publicizer). Packages like `Microsoft.Build.Utilities.Core` and `MonoMod.*` come from nuget.org via your user-level config. | `dotnet nuget list source` must list nuget.org. If it lists nothing: `dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org`. |

Subscribing to the three Workshop items (BeaverBuddies itself is `3293380223`) and launching the game once is the quickest way to get the mod DLLs onto a Steam machine.

## Environment file

`BeaverBuddies/env.props` holds the machine-specific paths. It is git-ignored; the first build copies `env.props.windows-template` or `env.props.unix-template` into place if it is missing (`EnsureEnvProps` target), after which you edit it. All four paths must end with a slash. Forward or back slashes both work in MSBuild.

| Property | Points at | Windows example (Steam Workshop) |
|---|---|---|
| `TimberbornDataPath` | `<install>\Timberborn_Data\` | `C:\Gaming\Steam\steamapps\common\Timberborn\Timberborn_Data\` |
| `DocumentsPath` | The folder containing `Timberborn\Mods\` (the deploy target). The Windows template reads it from the registry so relocated Documents folders work. | leave the template value |
| `HarmonyPath` | Folder containing `0Harmony.dll` | `C:\Gaming\Steam\steamapps\workshop\content\1062090\3284904751\` |
| `ModSettingsPath` | Folder containing `ModSettings.*.dll` | `C:\Gaming\Steam\steamapps\workshop\content\1062090\3283831040\version-1.0\Scripts\` |

The `CheckEnv` target fails fast with `<Property> property directory not found` when a path is wrong; that is the "directory not found" error the README mentions. An old `TimberbornPath` property is still accepted but deprecated in favour of `TimberbornDataPath`.

## Build == deploy

```
dotnet restore BeaverBuddies/BeaverBuddies.csproj
dotnet build   BeaverBuddies/BeaverBuddies.csproj -c Debug
```
or build `BeaverBuddies.sln` / press Ctrl+Shift+B in Visual Studio or Rider. Configurations: `Debug`, `Release`, `Debug Steam`, `Release Steam` (the Steam ones define `IS_STEAM`).

The `PostBuild` target **deletes and recreates** `<DocumentsPath>Timberborn\Mods\BeaverBuddies\version-1.0\`, copies the build output plus `Localizations\`, `KeyBindings\`, `KeyBindingGroups\`, and drops `thumbnail.png` + `workshop_data.json` one level up. There is no separate install step. To compile without touching your mods folder (CI, or checking a branch builds), redirect the deploy root:

```
dotnet build BeaverBuddies/BeaverBuddies.csproj -c Debug -p:BeaverBuddiesModsPath=C:\temp\bb-deploy\
```

After a build, **restart the game fully** (mod DLLs are loaded once per process). In the mod list the local build is the BeaverBuddies entry with a **folder icon**; enable it and disable the Workshop copy. `Player.log` confirms with `BeaverBuddies vX.Y.Z is loaded!`.

### Common build failures

| Message | Cause | Fix |
|---|---|---|
| `NU1101: Unable to find package Microsoft.Build.Utilities.Core ... source(s): BepInEx` | No nuget.org source on this machine. | Add nuget.org (see Prerequisites), or pass `--source https://api.nuget.org/v3/index.json --source https://nuget.bepinex.dev/v3/index.json` to `dotnet restore`. |
| `TimberbornDataPath property directory not found` (or Harmony / ModSettings / Documents) | `env.props` path wrong or missing trailing slash, or the mod is not installed. | Fix the path; install the mod. |
| `CS0246 ... could not be found` for a `Timberborn.*` type after a game update | The game renamed or removed the API. | See "After a Timberborn update" below. |
| `NETSDK1138: netcoreapp3.1 is out of support` | `Inspector/` in the solution build. | Harmless; build the csproj instead of the sln if it bothers you. |
| `MissingFieldException: Field not found: System.Collections.Immutable.ImmutableArray`1<...> ... _tickableSingletons` at runtime (game loads fine, save fails on `TickableSingletonService.Load`) | The `System.Collections.Immutable` NuGet version in the csproj differs from the one in `Timberborn_Data\Managed\` (game 1.1 ships 8.0.0.0). The mod folder then loads a second copy and `ImmutableArray<T>` is a different type than the game's field. | Pin the `PackageReference` to the game's assembly version (check with `[System.Reflection.AssemblyName]::GetAssemblyName(<dll>)`), rebuild, restart. Upstream did the same in `84e8270` on the `v1.1` branch. |

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

- Rebuild; fix compile errors from renamed/removed game APIs (publicized references make these loud). The compiler reports them in **waves**: fixing one batch surfaces the next, so rebuild after each batch until clean.
- A clean compile is not enough. Harmony resolves patches at startup, and `harmony.PatchAll()` throws (killing the whole mod) when a `[HarmonyPatch(typeof(X), nameof(X.M), typeof(...))]` overload no longer exists or a `Prefix`/`Postfix` names a parameter the game renamed. Neither is a compile error. Check every prefix parameter name against `_decompiled/` (a script that lists `Type.Method` plus bound parameter names and greps the decompiled signature catches this in seconds), and every explicit overload list.
- The automation list (`Events/AutomationEvents.cs`) resolves methods by name with `GetMethod`; a removed method yields null and a crash in `OverrideMethod`. Compile catches renames only because `nameof` is used.
- Grep `[ManualMethodOverwrite]` and diff each copied body against the new decompiled method.
- Check the automation method list (`Events/AutomationEvents.cs` ~95-183) for renamed methods; `No MethodInfo for:` at runtime means a key is stale.
- Re-run the `ReflectionUtils` probes (`Plugin.cs:87-96`, commented) and `Inspector` scans to refresh the audits in `BeaverBuddies/Doc/`.
- Update `MinimumGameVersion` in `manifest.json` and `<TimberbornTargetVersion>` (output folder name) if needed.

## Contributing

Wiki: https://github.com/thomaswp/BeaverBuddies/wiki/Contributing. Pick an issue, fix it, open a PR; ask on the Discord mod thread (link in `README.md`) or in the issue. Live-coding example of adding an event: https://www.youtube.com/watch?v=xD7x8R580N0.
