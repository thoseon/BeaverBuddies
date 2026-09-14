using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.BuildingsReachability;

internal class UnconnectedBuildingStatus : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private static readonly string UnconnectedLocKey = "Status.Buildings.Unconnected";

	private static readonly string UnconnectedShortLocKey = "Status.Buildings.Unconnected.Short";

	private readonly ILoc _loc;

	private DistrictBuilding _districtBuilding;

	private BlockObject _blockObject;

	private List<IUnconnectedBuildingBlocker> _unconnectedBuildingBlockers;

	private StatusToggle _statusToggle;

	private bool Blocked
	{
		get
		{
			if (_unconnectedBuildingBlockers != null)
			{
				return _unconnectedBuildingBlockers.FastAny((IUnconnectedBuildingBlocker blocker) => blocker.IsUnconnectedBlocked);
			}
			return false;
		}
	}

	public UnconnectedBuildingStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_districtBuilding = GetComponent<DistrictBuilding>();
		_blockObject = GetComponent<BlockObject>();
		_unconnectedBuildingBlockers = GetComponentsAllocating<IUnconnectedBuildingBlocker>();
		_statusToggle = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon("UnconnectedBuilding", _loc.T(UnconnectedLocKey), _loc.T(UnconnectedShortLocKey));
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
		foreach (IUnconnectedBuildingBlocker unconnectedBuildingBlocker in _unconnectedBuildingBlockers)
		{
			unconnectedBuildingBlocker.IsUnconnectedBlockedChanged += OnIsUnconnectedBlockedChanged;
		}
		UpdateStatus();
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

	private void OnIsUnconnectedBlockedChanged(object sender, EventArgs e)
	{
		UpdateStatus();
	}

	private void UpdateStatus()
	{
		_statusToggle.Toggle(_blockObject.IsFinished && !_districtBuilding.InstantDistrict && !Blocked);
	}
}
