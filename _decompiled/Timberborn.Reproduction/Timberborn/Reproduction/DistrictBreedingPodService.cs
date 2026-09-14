using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.GameDistricts;
using Timberborn.TickSystem;

namespace Timberborn.Reproduction;

public class DistrictBreedingPodService : TickableComponent, IAwakableComponent
{
	private DistrictBuildingRegistry _districtBuildingRegistry;

	private readonly Queue<BreedingPod> _breedingPodsNeedingNutrients = new Queue<BreedingPod>();

	public void Awake()
	{
		_districtBuildingRegistry = GetComponent<DistrictBuildingRegistry>();
	}

	public override void Tick()
	{
		_breedingPodsNeedingNutrients.Clear();
		foreach (BreedingPod enabledBuilding in _districtBuildingRegistry.GetEnabledBuildings<BreedingPod>())
		{
			if (enabledBuilding.NeedsNutrients)
			{
				_breedingPodsNeedingNutrients.Enqueue(enabledBuilding);
			}
		}
	}

	public bool TryDequeueNeedingNutrients(out BreedingPod breedingPod)
	{
		return _breedingPodsNeedingNutrients.TryDequeue(out breedingPod);
	}
}
