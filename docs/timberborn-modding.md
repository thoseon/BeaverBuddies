# Timberborn modding essentials

Distilled from the official [timberborn-modding wiki](https://github.com/mechanistry/timberborn-modding/wiki), limited to what a BeaverBuddies developer needs. Go to the wiki for blueprints, assets, Unity setup, and translations.

## How a code mod loads

- After all mod DLLs are loaded, the game finds every `IModStarter` implementation, instantiates it (parameterless constructor) and calls `StartMod(IModEnvironment)`. BeaverBuddies' is `Plugin` (`BeaverBuddies/Plugin.cs:109`).
- Game assemblies live in `<install>/Timberborn_Data/Managed/` (Windows) or `<install>/Contents/Resources/Data/Managed/` (macOS). `env.props` points the build at them ([build-and-test.md](build-and-test.md)).
- Compiled DLLs can sit anywhere in the mod folder; the game loads them at startup. Harmony/Cecil/MonoMod patching works without BepInEx. BeaverBuddies depends on the **Harmony mod** (not bundled since v1.7.0) and **ModSettings**.

## Mod folders and manifest

- Local mods: `Documents/Timberborn/Mods/<mod>/`. Steam Workshop mods: `<steam>/steamapps/workshop/content/1062090/`. A local build shows a folder icon in the mod list next to the Workshop copy.
- Version-specific builds go in `version-x` subfolders; the game picks the closest folder not above its version and ignores root content when any exist. BeaverBuddies builds into `version-1.0/`.
- `manifest.json` required fields: `Name`, `Version`, `Id`, `MinimumGameVersion`. Optional: `Description`, `RequiredMods`, `OptionalMods` (each `{ Id, MinimumVersion }`); dependencies load first. See `BeaverBuddies/manifest.json`.
- `-skipModManager` command-line flag bypasses the launch mod manager. Workshop upload happens from the in-game manager; `workshop_data.json` stores the Steam ID.

## Dependency injection: Bindito

- Configurators: classes deriving `Configurator` annotated `[Context("MainMenu" | "Game" | "MapEditor" | "Bootstrapper")]` are discovered automatically. `Bootstrapper` is the global context.
- `Bind<T>().AsSingleton()` – one instance per scene lifetime (services). `AsTransient()` – new instance per dependent (entity behaviour components). `MultiBind<T>().To<X>()` – injected as `IEnumerable<T>`.
- Constructor injection only; dependencies resolve recursively before instantiation.
- Singleton lifecycle: `ILoadableSingleton.Load()` (dependency-ordered at scene load), `IUpdatableSingleton.UpdateSingleton()` (every **frame**), `ITickableSingleton.Tick()` (every **tick**), `IParallelTickableSingleton` (ticked on worker threads), `IPostLoadableSingleton`.

**Why it matters here:** game logic in `UpdateSingleton` is frame-driven and therefore non-deterministic; BeaverBuddies moves such logic onto the tick (`TickReplacerService`). `IEarlyTickableSingleton` is BeaverBuddies' own marker to tick first. `Bindito.Core.dll`/`Bindito.Unity.dll` are referenced by the csproj.

## Entities and components

- World objects (beavers, buildings, plants) are **entities** made of **components**: `BaseComponent` subclasses or Specs from Blueprints.
- Lifecycle interfaces: `IAwakableComponent.Awake()` (creation), `IInitializableEntity.Initialize()` (placed on map), `IFinishedStateListener.OnEnterFinishedState()/OnExitFinishedState()` (construction done / destroyed), `IDeletableEntity.Delete()`, `IPersistentEntity.Save()/Load()`.
- **Decoration**: Specs cause components to be added at entity creation, which may add more (`TemplateModule.Builder.AddDecorator<TSource, TDecorator>()`).
- **Specs** are immutable `record : ComponentSpec` types with `[Serialize] { get; init; }` properties, `ImmutableArray` for collections.
- Every entity has an `EntityComponent` with a `Guid EntityId`. BeaverBuddies addresses entities in events by that GUID string and makes GUID generation deterministic (`GuidPatcher`). Objects without `EntityComponent` (prefabs, previews) are deliberately not synced.
- Ticking: `TickableEntity` → `TickableEntityBucket` → `TickableBucketService.TickBuckets`. Entities are spread across buckets ticked over several frames; BeaverBuddies replaces this loop ([architecture.md](architecture.md#the-tick-loop)).

## In-game debugging tools

| Tool | Toggle | What you get |
|---|---|---|
| Console | `Alt + ~` | Live `Debug.Log` output. |
| Developer mode | `Shift + Alt + Z` | Left panel of dev commands; Debugging/Control sections in the entity panel; extra toolbar tools (spawn beavers, map-editor objects); time-control keys `4`, `5`, `7`, `8`, `Shift+1`; `Ctrl`/`Shift` modifiers to unlock/spawn/plant instantly. |
| Debug mode | `Shift + Alt + X` | Debug panel (water, terrain, time diagnostics) and an **object debugger** to inspect fields of singletons or a selected entity's components. |
| Blueprint viewer | "Show Blueprint" in the entity panel (dev mode) | Full blueprint of the selected entity. |

Logs: `Debug.Log` writes to `Player.log` in `C:\Users\<user>\AppData\LocalLow\Mechanistry\Timberborn` (Windows) or `~/Library/Logs/Mechanistry/Timberborn` (macOS).

Caution in co-op: dev-mode actions (spawning, instant build) are **not** intercepted as events and will desync the other side. Use them only single-player or on both sides identically.

## Reading vanilla code

The repo carries a decompiled snapshot in `_decompiled/` (one folder per assembly, namespaces as subfolders; regenerate with `Export-TimberbornSource.ps1 -Force` after a game update). For anything not in the snapshot, use [ILSpy](https://github.com/icsharpcode/ilspy) (or dnSpy) on the `Timberborn.*.dll` assemblies in `Managed/`. The `Inspector/` project (`Inspector/Program.cs`) contains ILSpy-based scans that produced the audits in `BeaverBuddies/Doc/` (`ClassesWithRandom.txt`, `ClassesWithHashSets.txt`, `ClassesWithTimeTriggers.txt`, `DerivedClasses.txt`, `ParallelSingletons.txt`, `UIFragments.txt`, `AllEvents.txt`). Re-run or extend those after a game update.

The build publicizes the game assemblies, so private members are visible to the mod's code; when you copy a vanilla method into a patch, mark it `[ManualMethodOverwrite]` with the date and original code.

## User interface

Timberborn UI is Unity **UI Toolkit** (UXML structure + USS styling, packed into asset bundles). Useful for finding what to patch:

- `EntityPanel` (right side) is composed of `IEntityPanelFragment`s. Most building settings you need to sync live in a `*Fragment` class (`SluiceFragment`, `WaterInputDepthFragment`, `DistrictCenterFragment`, `AutomatableFragment`, …). `BeaverBuddies/Doc/UIFragments.txt` lists them.
- `VisualElementLoader` instantiates UXML; `UILayout` attaches elements (`AddBottomRight`, …); `PanelStack` shows modal panels; `DialogBoxShower` builds popups (BeaverBuddies uses it for the desync dialog).
- Toolbar buttons implement `ITool`; `ToolButtonFactory` creates them. `LocalizableX` elements auto-localize; BeaverBuddies' strings are in `Localizations/*.csv` (`ID,Text,Comment`, keys `BeaverBuddies.<Area>.<Key>`).
- BeaverBuddies injects its buttons by patching `MainMenuPanel.GetPanel`, `GameOptionsBox.GetPanel`, `LoadGameBox.GetPanel` (`Connect/`).

When syncing a fragment action, patch the method that changes **model state** (e.g. `SluiceState.SetAuto`) rather than the fragment's click handler when possible; the fragment usually delegates to a `BaseComponent` method that the automation list can cover.

## Links

- Modding wiki home: https://github.com/mechanistry/timberborn-modding/wiki
- Pages used here: Debugging, Timberborn architecture, Coding basics, Mod directory structure, Mod management, User interface, Quick start.
- Community modding guide: https://timberborn.wiki.gg/wiki/Creating_Mods
