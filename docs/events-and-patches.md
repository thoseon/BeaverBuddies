# Events and Harmony patches

How player actions are intercepted, the catalog of every `ReplayEvent`, and every other patch in the mod.

## The contract: `ReplayEvent.DoPrefix`

`Events/ReplayEvent.cs:112`:

```csharp
public static bool DoPrefix(Func<ReplayEvent> getEvent)
{
    if (ReplayService.IsReplayingEvents) return true;   // nested call during Replay(): run original
    ReplayService replayService = GetReplayServiceIfReady();
    if (replayService == null) return true;             // not co-op / not loaded / desynced: run original
    ReplayEvent message = getEvent();
    if (message == null) return true;                   // patch opted out for this call
    Plugin.Log(message.ToActionString());
    replayService.RecordEvent(message);
    return EventIO.ShouldPlayPatchedEvents;             // false for host (QueuePlay) and client (Send): swallow now, run at the tick
}
```

`ShouldPlayPatchedEvents` is `true` only while replaying or for the offline `FileWriteIO`/`FileReadIO` (`UserEventBehavior.Play`). Host and client both swallow the original; the host executes it at the next tick together with the remote events, which is what keeps ordering identical.

`DoEntityPrefix(BaseComponent, Func<string, ReplayEvent>)` (`ReplayEvent.cs:136`) resolves the entity ID first and returns **null → run original unpatched** when the component has no `EntityComponent`. That is how prefab and preview objects are excluded.

Consequences:

- `Replay()` may call the *same* patched method; `IsReplayingEvents` short-circuits recursion.
- After a desync (`IsDesynced`) everything silently behaves like single player.
- The base class carries `ticksSinceLoad` and `randomS0Before`; `type` is the class name; `LocalPlayerID` is a real GUID via `GuidPatcher.RealNewGuid()`.

Entity helpers on the base class: `GetEntityComponent(context, id)`, `GetComponent<T>(context, id)`, `GetEntityID(component)`, `GetBuilding`, `GetBuildingName`. They log `Could not parse guid` / `Could not find entity` / `Could not find component T on entity` and return null rather than throwing. Those warnings are the classic sign of an entity that exists on one side only.

## Example A: a plain entity event

`Events/EntityUIEvents.cs` ~329-382:

```csharp
class BuildingPausedChangedEvent : ReplayEvent
{
    public string entityID;
    public bool wasPaused;

    public override void Replay(IReplayContext context)
    {
        var pausable = GetComponent<PausableBuilding>(context, entityID);
        if (!pausable) return;
        if (wasPaused) pausable.Pause();
        else pausable.Resume();
    }

    public override string ToActionString() => $"Building {entityID} paused set to: {wasPaused}";
}

[HarmonyPatch(typeof(PausableBuilding), nameof(PausableBuilding.Pause))]
class PausableBuildingPausePatcher
{
    static bool Prefix(PausableBuilding __instance)
    {
        if (__instance.Paused) return true;   // idempotence guard: no duplicate events
        return ReplayEvent.DoEntityPrefix(__instance, entityID =>
            new BuildingPausedChangedEvent { entityID = entityID, wasPaused = true });
    }
}
```

## Example B: the reflection-driven `AutomationEvent`

`Events/AutomationEvents.cs`. One `UniversalPrefix(BaseComponent __instance, MethodBase __originalMethod, object[] __args)` (~209) is applied by `ApplyAutomationPatches(Harmony)` (~88) to a hard-coded list of `(Type, methodName)` pairs (~95-183). It records an `AutomationEvent { entityID, methodKey, arguments }`; `Replay` looks up the `MethodInfo` by `"{DeclaringType.FullName}.{Name}"`, resolves the component on the entity, deserialises arguments (enums, `double→float`, `long→int`, `BaseComponent` args as entity IDs) and invokes it.

Constraint (comment ~90-94): the method must be **UI-only** and live on a **`BaseComponent`**. It is called from `Plugin.StartMod` (`Plugin.cs:118`), not by `PatchAll`.

Covered today: `Chronometer`, `ContaminationSensor`, `DepthSensor`, `FireworkLauncher`, `FlowSensor`, `Gate`, `Indicator`, `Lever`, `Memory`, `PopulationCounter`, `PowerMeter`, `Relay`, `ResourceCounter`, `ScienceCounter`, `Speaker`, `Timer`, `WeatherStation`, plus `Floodgate`, `ThrottlingValve` (formerly `Valve`), `FillValve`, `WaterSourceRegulator.Open/Close/Automate`, `Clutch.SetMode`. `Relay` inputs use `SetInput(Automator, int)` / `IncreaseInputs` / `RemoveInput`; `Automator` arguments serialise as entity ids.

## Adding a new event

Recipe (wiki Contributing page, plus what the code requires):

1. **Find the game method** that performs the action (not the button handler if the handler just delegates). Decompile with ILSpy; UI code lives in `*Fragment` classes (see [timberborn-modding.md](timberborn-modding.md#user-interface)).
2. **Prefer the automation list**: if the method is UI-only and on a `BaseComponent`, add `(typeof(X), nameof(X.Method))` to `ApplyAutomationPatches`. Fixes `382e3c7` (2 lines) and `87fb778` (deleted 56 lines of bespoke events) show this is the cheapest correct fix.
3. Otherwise **write a `ReplayEvent` subclass** in the matching `Events/*.cs` file: public fields for the minimal data, `Replay(IReplayContext)`, `ToActionString()`. Serialisation is automatic (`$type`); no registration.
4. **Singletons needed in `Replay`**: request them in the `ReplayService` constructor (`ReplayService.cs:147-221`) and register with `AddSingleton`. Otherwise `GetSingleton<T>` falls back to a repository search and logs `Searching for unregistered singleton`.
5. **Write the prefix**: `static bool Prefix(...) => ReplayEvent.DoPrefix(() => new XEvent { ... });` or `DoEntityPrefix(__instance, id => ...)`. Add an idempotence guard if the method can be re-entered with no change.
6. **Replay must call the authoritative method.** Do not re-post the UI's side-effect event; fix `7787b1e` (science unlocks) was needed because `Replay` fired `OnToolUnlocked` and left `ToolUnlockingService._activeLockers` stale.
7. **Preview objects**: placement validation instantiates throwaway previews that can register real side effects (district centers, nav obstacles). See `ToolEvents.cs` ~47-49 and `Fixes/DistrictBuildingsFix.cs`.
8. **Private methods**: reference by string (`"SetWaterMovement"` in `WaterMoverToggle`, `EntityUIEvents.cs` ~1427) when the reflection list cannot be used or several arguments must be applied atomically.
9. Test with two instances; watch for the `Doing:`/`RecordEvent:` lines on both sides.

Do not rename or move existing event classes without a version bump: the class name is the wire format ([architecture.md](architecture.md#serialization-and-wire-format)).

## Catalog of `ReplayEvent`s

Line numbers are approximate; search by class name.

### Tools and world building: `Events/ToolEvents.cs`

| Event | Patched method |
|---|---|
| `BuildingPlacedEvent` | `BuildingPlacer.Place` (plus `BlockObjectTool.Place` pre/postfix capturing `_duplicationSource`) |
| `BuildingsDeconstructedEvent` | `BlockObjectDeletionTool<BuildingSpec>.DeleteBlockObjects` |
| `PlantingAreaMarkedEvent` | `PlantingSelectionService.MarkArea` / `UnmarkArea` |
| `ClearResourcesMarkedEvent` | `DemolishableSelectionTool.ActionCallback` / `DemolishableUnselectionTool.ActionCallback` |
| `TreeCuttingAreaEvent` | `TreeCuttingArea.AddCoordinates` / `RemoveCoordinates` |
| `BuildingUnlockedEvent` | `BuildingUnlockingService.Unlock` |
| `WorkingHoursChangedEvent` | `WorkingHoursPanel.OnHoursChanged` |
| `DuplicationEvent` | `Duplicator.Duplicate` |

### Building panels: `Events/EntityUIEvents.cs` (largest file)

Generic bases: `BuildingDropdownEvent<Selector>`, `PriorityChangedEvent<T>`.

| Area | Event | Patched method |
|---|---|---|
| Work | `GatheringPrioritizedEvent` | `GatherablePrioritizer.PrioritizeGatherable` |
| Work | `ManufactoryRecipeSelectedEvent` | `Manufactory.SetRecipe` |
| Work | `PlantablePrioritizedEvent` | `PlantablePrioritizer.PrioritizePlantable` |
| Work | `FarmHousePrioritizePlantingChangedEvent` | `FarmHouse.PrioritizePlanting` / `UnprioritizePlanting` |
| Work | `WorkplacePriorityChangedEvent` | `WorkplacePriority.SetPriority` |
| Work | `ConstructionPriorityChangedEvent` | `BuilderPrioritizable.SetPriority` |
| Work | `WorkplaceDesiredWorkersChangedEvent` | `Workplace.IncreaseDesiredWorkers` / `DecreaseDesiredWorkers` |
| Work | `WorkerTypeUnlockedEvent` | `WorkplaceUnlockingService.Unlock`, `WorkerTypeToggle.TryToUnlock` |
| Work | `WorkerTypeSetEvent` | `WorkplaceWorkerType.SetWorkerType` |
| Work | `ToggleForresterReplantDeadTreesEvent` | `Forester.SetReplantDeadTrees` |
| Goods | `SingleGoodAllowedEvent` | `SingleGoodAllower.Allow` / `Disallow` |
| Goods | `StockpilePriorityChangedEvent` | `StockpilePriority.Accept/Empty/Obtain/Supply` |
| Goods | `GoodStackDeletedEvent` | `DeleteRecoveredGoodStackFragment.DeleteRecoveredGoodStack` |
| Goods | `HaulPrioritizablePrioritizedEvent` | `HaulPrioritizable.Prioritized` setter (`MethodType.Setter`) |
| Building | `BuildingPausedChangedEvent` | `PausableBuilding.Pause` / `Resume` |
| Building | `DemolishButtonClickedEvent` | `DemolishableFragment.ChangeDemolishState` |
| Building | `DynamiteTriggeredEvent` | `DynamiteFragment.DetonateSelectedDynamite` |
| Building | `EntityRenamedEvent` | `EntityNameDialog.SetEntityName` |
| Building | `WonderActivatedEvent` | `WonderFragment.ActivateWonder` |
| Building | (no event) | `DeleteBuildingFragment.DeleteBuilding` |
| Water | `FloodgateHeightChangedEvent` | `Floodgate.SetHeightAndSynchronize` |
| Water | `FloodgateSynchronizedChangedEvent` | `Floodgate.ToggleSynchronization` |
| Water | `WaterInputDepthActionEvent` | `WaterInputPipeDepthFragment.ToggleDepthLimit` / `IncreaseDepth` / `DecreaseDepth` (private, patched by name) |
| Water | (automation list) | Throttling valves (formerly sluices and `Valve`) are covered by `ThrottlingValve.*AndSynchronize` / `ToggleSynchronization` entries in `Events/AutomationEvents.cs`. The `Sluice*` event classes were removed with the September 2026 game update. |
| Water | `WaterMoverModeChangedEvent` | `WaterMoverToggle."SetWaterMovement"` (private, by name) |
| Ziplines | `ZiplineConnectionChangedEvent` | `ZiplineConnectionAddingTool.Connect`, `ZiplineConnectionButtonFactory.RemoveConnection` |
| Districts | `DefaultWorkerTypeChangedEvent` | `DistrictCenterFragment.SetBeaverWorkerType` / `SetBotWorkerType` |

### Districts, migration, distribution: `Events/BatchEvents.cs`

| Event | Patched method |
|---|---|
| `ManualMigrationEvent` | `ManualMigrationPopulationRow.MigratePopulation` |
| `SetDistrictMinimumPopulationEvent` | `PopulationDistributor.SetMinimumAndMigrate` |
| `SetDistrictMigrationToggledEvent` | `PopulationDistributor.ToggleAllowImmigrationAndMigrate` / `ToggleAllowEmigrationAndMigrate` |
| `GoodDistributionSettingChangedEvent` | `GoodDistributionSetting.SetExportThreshold` / `SetImportOption` / `SetDefault` |

Infrastructure: `DistrictDistributionSetting.AddGoodDistributionSetting` is patched to swap in `GoodDistributionSettingWithDistrict`, a subclass carrying the district back-reference the vanilla object lacks. `GoodDistributionSetting without district!!` means that swap failed. `DistributorUtils` maps `IDistributorTemplate` to an enum (`Unknown distributor type` = new template).

### Automation: `Events/AutomationEvents.cs`

| Event | Patched method |
|---|---|
| `AutomationEvent` | ~60 methods via `UniversalPrefix` (see Example B) |
| `SetAutomatableInputEvent` | `AutomatableFragment.SetInput` |
| `SetTimerIntervalEvent` | `TimerIntervalElement.SetTimeInterval` (with `TimerFragment.ShowFragment`/`ClearFragment` tracking `CurrentEditingTimer`) |
| `ResetTransmitterEvent` | `SequentialTransmitterResetFragment.OnReset` / `OnResetAll` |
| `WeatherStationSetActivateEarlyEvent` | `WeatherStationFragment.OnEarlyActivationToggleChanged` |

### Time and menus: `Events/TimeEvents.cs`

| Event | Patched method |
|---|---|
| `SpeedSetEvent` | `SpeedManager.ChangeSpeed(float)`; `SpeedChangePatcher.SetSpeedSilentlyNow` is the no-event escape hatch |
| `ShowOptionsMenuEvent : SpeedSetEvent` | `GameOptionsBox.Show` (a synced pause) |
| (no event) | `SpeedManager.ChangeAndLockSpeed` / `UnlockSpeed` (clients never freeze for dialogs), `OverlayPanelSpeedLocker.OnPanelShown` (gated by `PauseReduction`) |

### Connection and system

| Event | File | Purpose |
|---|---|---|
| `InitializeClientEvent` | `Events/ConnectionEvents.cs` | Version + debug-mode handshake; created by `ServerEventIO.CreateInitEvent`. |
| `ClientDesyncedEvent` | `Events/ConnectionEvents.cs` | The desync dialog, report upload, rehost/reconnect. |
| `AutosaveEvent` | `Events/SystemEvents.cs` | Patch on `Autosaver.Save` is **commented out** (saving added a spurious trace). |
| `GroupedEvent` | `ReplayService.cs:54` | Per-tick batch; flattened before replay. |
| `HeartbeatEvent` | `ReplayService.cs:70` | Server-only no-op so every tick has an event. |
| `TraceLoggedForTickEvent` | `DesyncDetecter/DesyncDetecterService.cs:19` | Carries the server's trace list; `Replay` verifies and may `HandleDesync`. |
| `PingEvent` | `Ping/PingEvent.cs` | Map ping; `CreatorID` prevents double-play. |

## All other `[HarmonyPatch]` sites (outside `Events/`)

| File | Target | Reason |
|---|---|---|
| `ReplayService.cs:753` | `TickableBucketService.TickBuckets` | **The lockstep tick loop** (`[ManualMethodOverwrite]`). |
| `ReplayService.cs:577` | `TickableSingletonService.Load` | Tick `IEarlyTickableSingleton`s first. |
| `DeterminismService.cs` | ~20 targets | RNG routing, GUIDs, time, entity order; see [determinism.md](determinism.md). |
| `DesyncDetecter/DesyncPatches.cs` | ~14 targets | Trace-only, gated on `Settings.Debug`. |
| `Fixes/AnimationFixes.cs:29` | `MovementAnimator.Update` | Deterministic smooth animation. |
| `Fixes/DistrictBuildingsFix.cs` | `DistrictMap.AddDistrictCenter`, `DistrictObstacleService.SetObstacle`/`UnsetObstacle`, `DistrictConnections.GetDistrictsConnectedWith`/`AreDistrictsConnected` | `HarmonyFinalizer`s swallow exceptions caused by preview registration and post-removal migration. |
| `Fixes/WaterSourceFix.cs` | `WaterSource.Tick`, `TickableSingletonService.FinishParallelTick` | Buffer water-source ticks until after the parallel sim. |
| `Fixes/WaterWheelFix.cs` | `MechanicalNodeFacingMarkerDrawer.GetTransput` | Try/catch around a vanilla UI exception. |
| `Fixes/TickOnlyArrayFix.cs` | `TickOnlyArrayService.AllowEdit` getter | Allow reads while saving off-tick. |
| `Connect/ClientConnectionUI.cs` | `MainMenuPanel.GetPanel`, `GameOptionsBox.GetPanel` | Inject "Join co-op game". |
| `Connect/ServerHostingUtils.cs` | `LoadGameBox.GetPanel` (+ copy of `LoadGame`) | Inject "Host co-op game". |
| `Editor/MapEditorPatches.cs` | `MapEditorBlockObjectButtons.GetElements`, `StartingLocationService.*`, `DuplicationValidator.CanDuplicateObject`, `MapMetadata*` | Multiple starting locations in the editor; persist `MaxPlayers`. |
| `MultiStart/MultiStartPatches.cs` | `StartingBuildingInitializer.Initialize`, `GameInitializer.SpawnBeavers`, `CustomNewGameModeController.*`, `NewGameModePanel.*` | Per-player starts in new games. |
| `GameSaveHelper.cs` | `DateSalter.Save`, `DateTime.ToString(string)` | Deterministic save bytes. |
| `Steam/SteamOverlayInputBlockerPatch.cs` | `SteamOverlayInputBlocker.SteamOverlayActivated` | Steam-build overlay handling. |
| `Fixes/SimulationUpdateFix.cs` | (all commented out) | Parked. |
| `Fixes/TestingStrategies_Scrap.cs` | (not compiled) | Bisection methodology. |

Non-Harmony patches: `GameSaverSavePatcher` (MonoMod `Hook`; `GameSaver.Save` has `try-catch-when`) and `TimeTimePatcher` (MonoMod native detour on `Time.time`). Both installed from `Plugin.StartMod`.

## `[ManualMethodOverwrite]`

`Attributes.cs` defines a marker for any patch whose body is a **copy of vanilla source** (with a dated comment containing the original). These break silently when Timberborn changes the copied method. Sites at the time of writing:

`ReplayService.cs:744`, `DeterminismService.cs:930, 1053`, `Fixes/AnimationFixes.cs:12`, `Events/TimeEvents.cs:33, 86, 120`, `Events/ToolEvents.cs:490`, `Events/EntityUIEvents.cs:794, 893`, `Fixes/WaterSourceStrengthFix.cs`, `Connect/ServerHostingUtils.cs:41, 78`, `Editor/MapEditorPatches.cs:127, 187`, `MultiStart/MultiStartPatches.cs:27, 92`.

After every game update: `grep -rn "ManualMethodOverwrite\]"` and diff each body against the new decompiled method.

## Why patches can touch private members

`BeaverBuddies.csproj` references `Timberborn.*.dll` and `UnityEngine.*.dll` with `Publicize="true"` (BepInEx.AssemblyPublicizer). That is why code reads `__instance._tickableEntities` or `_animatedPathFollower` directly instead of through `Traverse`/reflection. `Traverse.Create(...).Field<T>("...")` still appears in older code (e.g. the science unlock fix).
