using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;

namespace Timberborn.Wellbeing;

public class WellbeingService : ITickableSingleton, ILoadableSingleton, IPostLoadableSingleton
{
	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly EventBus _eventBus;

	private readonly GlobalWellbeingTrackerRegistry _globalWellbeingTrackerRegistry;

	private DistrictCenter _districtCenter;

	public int AverageGlobalWellbeing { get; private set; }

	public int AverageDistrictWellbeing { get; private set; }

	internal WellbeingService(EntityComponentRegistry entityComponentRegistry, EventBus eventBus, GlobalWellbeingTrackerRegistry globalWellbeingTrackerRegistry)
	{
		_entityComponentRegistry = entityComponentRegistry;
		_eventBus = eventBus;
		_globalWellbeingTrackerRegistry = globalWellbeingTrackerRegistry;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void PostLoad()
	{
		UpdateAverageGlobalWellbeing();
	}

	public void Tick()
	{
		UpdateAverageGlobalWellbeing();
		UpdateAverageDistrictWellbeing();
	}

	[OnEvent]
	public void OnMigration(MigrationEvent migrationEvent)
	{
		UpdateAverageDistrictWellbeing();
	}

	[OnEvent]
	public void OnNewGameInitialized(NewGameInitializedEvent newGameInitializedEvent)
	{
		UpdateAverageGlobalWellbeing();
	}

	public void SwitchDistrict(DistrictCenter districtCenter)
	{
		_districtCenter = districtCenter;
		UpdateAverageDistrictWellbeing();
	}

	public void GlobalAppliedNeeds(Dictionary<string, int> appliedNeeds)
	{
		AppliedNeeds(_entityComponentRegistry.GetEnabled<NeedManager>(), appliedNeeds);
	}

	public void DistrictAppliedNeeds(Dictionary<string, int> appliedNeeds)
	{
		AppliedNeeds(_districtCenter.DistrictPopulation.GetEnabledCharacters<NeedManager>(), appliedNeeds);
	}

	public int GetAverageDistrictWellbeing(DistrictCenter districtCenter)
	{
		return districtCenter.GetComponent<DistrictWellbeingTrackerRegistry>().Registry.GetAverageWellbeing();
	}

	private static void AppliedNeeds(IEnumerable<NeedManager> needManagers, IDictionary<string, int> appliedNeeds)
	{
		foreach (NeedManager needManager in needManagers)
		{
			if (!needManager.HasComponent<WellbeingTrackerRegistrar>())
			{
				continue;
			}
			foreach (NeedSpec needSpec in needManager.NeedSpecs)
			{
				string id = needSpec.Id;
				if (NeedShouldBeCounted(needManager, id))
				{
					int orAdd = appliedNeeds.GetOrAdd(id);
					orAdd = (appliedNeeds[id] = orAdd + 1);
				}
			}
		}
	}

	private void UpdateAverageGlobalWellbeing()
	{
		AverageGlobalWellbeing = _globalWellbeingTrackerRegistry.Registry.GetAverageWellbeing();
	}

	private void UpdateAverageDistrictWellbeing()
	{
		if ((bool)_districtCenter)
		{
			AverageDistrictWellbeing = _districtCenter.GetComponent<DistrictWellbeingTrackerRegistry>().Registry.GetAverageWellbeing();
		}
	}

	private static bool NeedShouldBeCounted(NeedManager needManager, string needId)
	{
		if (!needManager.GetNeedSpec(needId).IsNeverPositive)
		{
			return needManager.NeedIsFavorable(needId);
		}
		return !needManager.NeedIsFavorable(needId);
	}
}
