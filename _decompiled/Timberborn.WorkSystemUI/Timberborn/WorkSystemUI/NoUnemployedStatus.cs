using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Localization;
using Timberborn.StatusSystem;
using Timberborn.WorkSystem;

namespace Timberborn.WorkSystemUI;

internal class NoUnemployedStatus : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private static readonly string NoUnemployedLocKey = "Status.Work.NoUnemployed";

	private static readonly string NoUnemployedShortLocKey = "Status.Work.NoUnemployed.Short";

	private readonly ILoc _loc;

	private Workplace _workplace;

	private DistrictBuilding _districtBuilding;

	private BlockableObject _blockableObject;

	private StatusToggle _statusToggle;

	public NoUnemployedStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_workplace = GetComponent<Workplace>();
		_districtBuilding = GetComponent<DistrictBuilding>();
		_blockableObject = GetComponent<BlockableObject>();
		_workplace.WorkerAssigned += delegate
		{
			UpdateStatus();
		};
		_workplace.WorkerUnassigned += delegate
		{
			UpdateStatus();
		};
		_blockableObject.ObjectUnblocked += delegate
		{
			UpdateStatus();
		};
		_blockableObject.ObjectBlocked += delegate
		{
			UpdateStatus();
		};
		_statusToggle = StatusToggle.CreateNormalStatusWithAlert("NoUnemployed", _loc.T(NoUnemployedLocKey), _loc.T(NoUnemployedShortLocKey), 0.1f);
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
	}

	public void OnEnterFinishedState()
	{
		UpdateStatus();
		_districtBuilding.ReassignedInstantDistrict += OnReassignedInstantDistrict;
	}

	public void OnExitFinishedState()
	{
		_statusToggle.Deactivate();
		_districtBuilding.ReassignedInstantDistrict -= OnReassignedInstantDistrict;
	}

	private void OnReassignedInstantDistrict(object sender, EventArgs e)
	{
		UpdateStatus();
	}

	private void UpdateStatus()
	{
		if (_workplace.NumberOfAssignedWorkers == 0 && _blockableObject.IsUnblocked && (bool)_districtBuilding.InstantDistrict)
		{
			_statusToggle.Activate();
		}
		else
		{
			_statusToggle.Deactivate();
		}
	}
}
