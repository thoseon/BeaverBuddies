using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.LinkedBuildingSystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.DistributionSystem;

internal class DistrictCrossingValidator : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private static readonly string InvalidConnectionLocKey = "Status.Distribution.InvalidConnection";

	private readonly ILoc _loc;

	private DistrictBuilding _districtBuilding;

	private StatusSubject _statusSubject;

	private DistrictCrossingValidator _linked;

	private StatusToggle _sameDistrictStatusToggle;

	private StatusToggle _linkedInvalidStatusToggle;

	public bool CanExport
	{
		get
		{
			if (IsValid && _linked.IsValid)
			{
				return !IsConnectedToSameDistrict;
			}
			return false;
		}
	}

	private bool IsValid
	{
		get
		{
			if (IsLinkedAndFinished)
			{
				return IsProperlyConnected;
			}
			return false;
		}
	}

	private bool IsLinkedAndFinished
	{
		get
		{
			if ((bool)_linked)
			{
				return base.Enabled;
			}
			return false;
		}
	}

	private bool IsProperlyConnected
	{
		get
		{
			if ((bool)_districtBuilding.District)
			{
				return _districtBuilding.District != _linked._districtBuilding.District;
			}
			return false;
		}
	}

	private bool IsConnectedToSameDistrict
	{
		get
		{
			if ((bool)_districtBuilding.District)
			{
				return _districtBuilding.District == _linked._districtBuilding.District;
			}
			return false;
		}
	}

	public DistrictCrossingValidator(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_districtBuilding = GetComponent<DistrictBuilding>();
		_statusSubject = GetComponent<StatusSubject>();
		GetComponent<LinkedBuilding>().BuildingLinked += OnBuildingLinked;
		_sameDistrictStatusToggle = StatusToggle.CreateNormalStatusWithFloatingIcon("UnreachableObject", _loc.T(InvalidConnectionLocKey));
		_linkedInvalidStatusToggle = StatusToggle.CreateNormalStatus("UnreachableObject", _loc.T(InvalidConnectionLocKey));
		DisableComponent();
	}

	public void InitializeEntity()
	{
		_statusSubject.RegisterStatus(_sameDistrictStatusToggle);
		_statusSubject.RegisterStatus(_linkedInvalidStatusToggle);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		_districtBuilding.ReassignedDistrict += OnReassignedDistrict;
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		_districtBuilding.ReassignedDistrict -= OnReassignedDistrict;
	}

	private void OnBuildingLinked(object sender, LinkedBuilding e)
	{
		_linked = e.GetComponent<DistrictCrossingValidator>();
	}

	private void OnReassignedDistrict(object sender, EventArgs e)
	{
		ValidateIfLinkedAndFinished();
	}

	private void ValidateIfLinkedAndFinished()
	{
		if (IsLinkedAndFinished && _linked.IsLinkedAndFinished)
		{
			Validate();
			_linked.Validate();
		}
	}

	private void Validate()
	{
		CheckLinkedConnection();
		CheckSameDistrictStatus();
	}

	private void CheckLinkedConnection()
	{
		if (_linked.IsProperlyConnected)
		{
			_linkedInvalidStatusToggle.Deactivate();
		}
		else
		{
			_linkedInvalidStatusToggle.Activate();
		}
	}

	private void CheckSameDistrictStatus()
	{
		if (IsConnectedToSameDistrict)
		{
			_sameDistrictStatusToggle.Activate();
			_linkedInvalidStatusToggle.Deactivate();
		}
		else
		{
			_sameDistrictStatusToggle.Deactivate();
		}
	}
}
