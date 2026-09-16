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
    /// </summary>
    // ILoadableSingleton guarantees the game instantiates this service at scene
    // load even though nothing else depends on it.
    public class InstantNavMeshSyncService : RegisteredSingleton, ILoadableSingleton
    {
        private readonly NavigationSynchronizer _navigationSynchronizer;

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
         */
        public void Synchronize()
        {
            _navigationSynchronizer.LateUpdateSingleton();
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
