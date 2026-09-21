using BeaverBuddies.IO;
using HarmonyLib;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;

namespace BeaverBuddies.Fixes
{
    /// <summary>
    /// Timberborn keeps two copies of the nav mesh and the district map. The
    /// "regular" copy is updated in NavigationSynchronizer.Tick() and is
    /// therefore deterministic. The "instant" copy is updated in
    /// NavigationSynchronizer.LateUpdateSingleton(), i.e. once per rendered
    /// frame, and gameplay reads it: Walker.PathIsTooFarFromDistrict
    /// (IsOnInstantDistrictRoadSpill), Citizen.UnassignDistrictIfCutOff
    /// (GlobalReachabilityService), ReachableConstructionSite,
    /// ReachableDemolishable, RecoveredGoodStackAccessible, and the instant
    /// listeners DistrictBuildingAssigner, DistrictConstructionAssigner and
    /// DistrictConnections. Entity buckets are spread over several frames, so
    /// whether a bucket sees an instant change enqueued earlier in the same
    /// tick depends on where a frame boundary fell, which differs per machine.
    ///
    /// Symptom in a desync trace: "&lt;guid&gt; finished pathfinding;
    /// reachable = False" on one side and "reachable = True" on the other
    /// right after a construction changed the nav mesh; the other side reaches
    /// the same state one tick later.
    ///
    /// Fix: also apply the instant changes at the start of every entity
    /// bucket. Entities inside one bucket never straddle a frame, so the
    /// per-frame LateUpdate can no longer change what gameplay sees. The call
    /// is cheap when the queues are empty. The LateUpdate itself is left
    /// alone so previews keep updating while the game is paused.
    ///
    /// In co-op the per-frame call must not apply instant changes at all:
    /// otherwise changes queued after the last bucket of a tick are applied
    /// before or after the next tick's replayed events depending on the frame,
    /// and the desync trace lands at a machine-dependent position (seen as
    /// false desyncs on 2026-09-21). InstantNavMeshPerFramePatcher therefore
    /// skips ProcessInstantChanges unless Synchronize() is running.
    /// </summary>
    // ILoadableSingleton guarantees the game instantiates this service at scene
    // load even though nothing else depends on it.
    public class InstantNavMeshSyncService : RegisteredSingleton, ILoadableSingleton
    {
        private readonly NavigationSynchronizer _navigationSynchronizer;

        public static bool IsSynchronizing { get; private set; }

        public InstantNavMeshSyncService(NavigationSynchronizer navigationSynchronizer)
        {
            _navigationSynchronizer = navigationSynchronizer;
        }

        public void Load() { }

        /*
            09/16/2026 - NavigationSynchronizer (Timberborn.Navigation):
            public void LateUpdateSingleton()
            {
                ProcessPreviewChanges();
                ProcessInstantChanges();
                NotifyAllNavmeshChanges();
            }
            private void ProcessInstantChanges()
            {
                _navMeshUpdater.ProcessInstantChanges(_instantNavMeshUpdateBuilder);
                _districtUpdater.ProcessInstantChanges(_instantNavMeshUpdateBuilder);
            }
         */
        public void Synchronize()
        {
            IsSynchronizing = true;
            try
            {
                _navigationSynchronizer.ProcessInstantChanges();
                _navigationSynchronizer.NotifyAllNavmeshChanges();
            }
            finally
            {
                IsSynchronizing = false;
            }
        }
    }

    // Stops the per-frame LateUpdateSingleton (and anything else outside
    // Synchronize) from applying instant changes during a co-op game. The
    // game's own PostLoad call runs before ReplayService.IsLoaded and is kept.
    [HarmonyPatch(typeof(NavigationSynchronizer), "ProcessInstantChanges")]
    class InstantNavMeshPerFramePatcher
    {
        static bool Prefix()
        {
            if (EventIO.IsNull) return true;
            if (!ReplayService.IsLoaded) return true;
            return InstantNavMeshSyncService.IsSynchronizing;
        }
    }

    [HarmonyPatch(typeof(TickableEntityBucket), nameof(TickableEntityBucket.TickAll))]
    class TickableEntityBucketInstantNavMeshPatcher
    {
        static void Prefix()
        {
            if (EventIO.IsNull) return;
            SingletonManager.GetSingleton<InstantNavMeshSyncService>()?.Synchronize();
        }
    }
}
