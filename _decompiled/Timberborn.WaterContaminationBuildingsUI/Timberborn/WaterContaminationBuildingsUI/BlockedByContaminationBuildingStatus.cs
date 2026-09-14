using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;
using Timberborn.WaterContaminationBuildings;

namespace Timberborn.WaterContaminationBuildingsUI;

internal class BlockedByContaminationBuildingStatus : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private static readonly string BlockedByContaminationLocKey = "Status.Buildings.BlockedByContamination";

	private static readonly string BlockedByContaminationShortLocKey = "Status.Buildings.BlockedByContamination.Short";

	private readonly ILoc _loc;

	private StatusToggle _statusToggle;

	private ContaminationBlockableBuilding _contaminationBlockableBuilding;

	public BlockedByContaminationBuildingStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_statusToggle = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon("BuildingBlockedByContamination", _loc.T(BlockedByContaminationLocKey), _loc.T(BlockedByContaminationShortLocKey));
		_contaminationBlockableBuilding = GetComponent<ContaminationBlockableBuilding>();
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
	}

	public void OnEnterFinishedState()
	{
		_contaminationBlockableBuilding.BlockedByContamination += OnBlockedByContamination;
		_contaminationBlockableBuilding.UnblockedByContamination += OnUnblockedByContamination;
		if (_contaminationBlockableBuilding.IsBlocked)
		{
			_statusToggle.Activate();
		}
	}

	public void OnExitFinishedState()
	{
		_contaminationBlockableBuilding.BlockedByContamination -= OnBlockedByContamination;
		_contaminationBlockableBuilding.UnblockedByContamination -= OnUnblockedByContamination;
		_statusToggle.Deactivate();
	}

	private void OnBlockedByContamination(object o, EventArgs eventArgs)
	{
		_statusToggle.Activate();
	}

	private void OnUnblockedByContamination(object o, EventArgs eventArgs)
	{
		_statusToggle.Deactivate();
	}
}
