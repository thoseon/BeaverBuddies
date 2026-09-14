using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;

namespace Timberborn.Wellbeing;

internal class WellbeingTrackerRegistrar : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly GlobalWellbeingTrackerRegistry _globalWellbeingTrackerRegistry;

	private WellbeingTracker _wellbeingTracker;

	private Citizen _citizen;

	private DistrictWellbeingTrackerRegistry _districtWellbeingTrackerRegistry;

	public WellbeingTrackerRegistrar(GlobalWellbeingTrackerRegistry globalWellbeingTrackerRegistry)
	{
		_globalWellbeingTrackerRegistry = globalWellbeingTrackerRegistry;
	}

	public void Awake()
	{
		_wellbeingTracker = GetComponent<WellbeingTracker>();
		_citizen = GetComponent<Citizen>();
	}

	public void InitializeEntity()
	{
		Character component = GetComponent<Character>();
		if (component != null && component.Alive)
		{
			_globalWellbeingTrackerRegistry.Registry.Register(_wellbeingTracker);
			_citizen.ChangedAssignedDistrict += OnChangedAssignedDistrict;
			component.Died += OnDied;
			UpdateDistrictWellbeingTrackerRegistry();
		}
	}

	private void OnDied(object sender, EventArgs e)
	{
		_globalWellbeingTrackerRegistry.Registry.Unregister(_wellbeingTracker);
		_districtWellbeingTrackerRegistry?.Registry.Unregister(_wellbeingTracker);
	}

	private void OnChangedAssignedDistrict(object sender, ChangeAssignedDistrictEventArgs e)
	{
		UpdateDistrictWellbeingTrackerRegistry();
	}

	private void UpdateDistrictWellbeingTrackerRegistry()
	{
		_districtWellbeingTrackerRegistry?.Registry.Unregister(_wellbeingTracker);
		DistrictCenter assignedDistrict = _citizen.AssignedDistrict;
		if (assignedDistrict != null)
		{
			_districtWellbeingTrackerRegistry = assignedDistrict.GetComponent<DistrictWellbeingTrackerRegistry>();
			_districtWellbeingTrackerRegistry.Registry.Register(_wellbeingTracker);
		}
	}
}
