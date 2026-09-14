using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Illumination;

namespace Timberborn.GameDistricts;

internal class DistrictBuildingIlluminator : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private IlluminatorToggle _illuminatorToggle;

	private DistrictBuilding _districtBuilding;

	public void Awake()
	{
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
		_districtBuilding = GetComponent<DistrictBuilding>();
	}

	public void OnEnterFinishedState()
	{
		_districtBuilding.ReassignedInstantDistrict += OnReassignedInstantDistrict;
		UpdateIlluminator();
	}

	public void OnExitFinishedState()
	{
		_districtBuilding.ReassignedInstantDistrict -= OnReassignedInstantDistrict;
	}

	private void OnReassignedInstantDistrict(object sender, EventArgs e)
	{
		UpdateIlluminator();
	}

	private void UpdateIlluminator()
	{
		if ((bool)_districtBuilding.InstantDistrict)
		{
			_illuminatorToggle.TurnOn();
		}
		else
		{
			_illuminatorToggle.TurnOff();
		}
	}
}
