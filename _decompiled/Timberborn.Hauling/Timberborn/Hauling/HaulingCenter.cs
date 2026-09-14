using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.WorkSystem;

namespace Timberborn.Hauling;

public class HaulingCenter : BaseComponent, IAwakableComponent, IFinishedStateListener, IRegisteredComponent
{
	private DistrictBuilding _districtBuilding;

	public void Awake()
	{
		_districtBuilding = GetComponent<DistrictBuilding>();
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public void GetWorkplaceBehaviorsOrdered(IList<WorkplaceBehavior> workplaceBehaviors)
	{
		DistrictCenter district = _districtBuilding.District;
		if ((bool)district)
		{
			district.GetComponent<DistrictHaulCandidates>().GetWorkplaceBehaviorsOrdered(workplaceBehaviors);
		}
	}
}
