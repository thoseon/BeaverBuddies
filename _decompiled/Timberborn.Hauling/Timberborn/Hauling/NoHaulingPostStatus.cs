using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.GameDistricts;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.Hauling;

public class NoHaulingPostStatus : BaseComponent, IAwakableComponent
{
	private static readonly string NoHaulingPostLocKey = "Status.Hauling.NoHaulingPost";

	private readonly ILoc _loc;

	private HaulCandidate _haulCandidate;

	private DistrictBuilding _districtBuilding;

	private StatusToggle _noHaulingPostStatus;

	private Func<bool> _activePredicate;

	public NoHaulingPostStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_haulCandidate = GetComponent<HaulCandidate>();
		_districtBuilding = GetComponent<DistrictBuilding>();
		_noHaulingPostStatus = StatusToggle.CreateNormalStatusWithFloatingIcon("OutOfHaulersRange", _loc.T(NoHaulingPostLocKey));
	}

	public void Initialize(Func<bool> activePredicate)
	{
		Asserts.FieldIsNull(this, _activePredicate, "_activePredicate");
		_activePredicate = activePredicate;
		GetComponent<StatusSubject>().RegisterStatus(_noHaulingPostStatus);
		_haulCandidate.InHaulingCenterRangeChanged += OnInHaulingCenterRangeChanged;
		_districtBuilding.ReassignedDistrict += OnDistrictReassigned;
		UpdateStatus();
	}

	public void Disable()
	{
		_haulCandidate.InHaulingCenterRangeChanged -= OnInHaulingCenterRangeChanged;
		DeactivateStatus();
	}

	public void UpdateStatus()
	{
		if (_activePredicate() && !_haulCandidate.IsInHaulingCenterRange)
		{
			DistrictCenter districtOrConstructionDistrict = _districtBuilding.GetDistrictOrConstructionDistrict();
			if (districtOrConstructionDistrict != null && (bool)districtOrConstructionDistrict)
			{
				_noHaulingPostStatus.Activate();
				return;
			}
		}
		DeactivateStatus();
	}

	private void OnInHaulingCenterRangeChanged(object sender, EventArgs e)
	{
		UpdateStatus();
	}

	private void OnDistrictReassigned(object sender, EventArgs e)
	{
		UpdateStatus();
	}

	private void DeactivateStatus()
	{
		_noHaulingPostStatus.Deactivate();
	}
}
