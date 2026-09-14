using System;
using Timberborn.BaseComponentSystem;
using Timberborn.GameDistricts;
using Timberborn.SelectionSystem;

namespace Timberborn.GameDistrictsUI;

internal class SelectableDistrictBuilding : BaseComponent, IAwakableComponent, ISelectionListener
{
	private readonly DistrictContextService _districtContextService;

	private DistrictBuilding _districtBuilding;

	public SelectableDistrictBuilding(DistrictContextService districtContextService)
	{
		_districtContextService = districtContextService;
	}

	public void Awake()
	{
		_districtBuilding = GetComponent<DistrictBuilding>();
	}

	public void OnSelect()
	{
		UpdateDistrictSelection();
		_districtBuilding.ReassignedInstantDistrict += OnReassignedInstantDistrict;
	}

	public void OnUnselect()
	{
		_districtContextService.UnselectDistrict();
		_districtBuilding.ReassignedInstantDistrict -= OnReassignedInstantDistrict;
	}

	private void OnReassignedInstantDistrict(object sender, EventArgs e)
	{
		UpdateDistrictSelection();
	}

	private void UpdateDistrictSelection()
	{
		DistrictCenter instantOrConstructionDistrict = _districtBuilding.GetInstantOrConstructionDistrict();
		if (instantOrConstructionDistrict != null)
		{
			_districtContextService.SelectDistrict(instantOrConstructionDistrict);
		}
		else
		{
			_districtContextService.UnselectDistrict();
		}
	}
}
