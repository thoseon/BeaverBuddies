using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;
using Timberborn.WaterBuildings;

namespace Timberborn.WaterBuildingsUI;

internal class NeedsWaterBuildingStatus : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string NeedsWaterLocKey = "Status.Buildings.NeedsWater";

	private static readonly string NeedsWaterShortLocKey = "Status.Buildings.NeedsWater.Short";

	private readonly ILoc _loc;

	private StatusToggle _statusToggle;

	private IWaterNeedingBuilding _tickableWaterBuilding;

	public NeedsWaterBuildingStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_statusToggle = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon("BuildingNeedsWater", _loc.T(NeedsWaterLocKey), _loc.T(NeedsWaterShortLocKey));
		_tickableWaterBuilding = GetComponent<IWaterNeedingBuilding>();
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
		_tickableWaterBuilding.StartedNeedingWater += OnStartedNeedingWater;
		_tickableWaterBuilding.StoppedNeedingWater += OnStoppedNeedingWater;
	}

	private void OnStartedNeedingWater(object o, EventArgs eventArgs)
	{
		_statusToggle.Activate();
	}

	private void OnStoppedNeedingWater(object o, EventArgs eventArgs)
	{
		_statusToggle.Deactivate();
	}
}
