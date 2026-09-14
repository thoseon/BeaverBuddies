using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.GameDistricts;

namespace Timberborn.ResourceCountingSystem;

internal class GoodProcessorRegistrar : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private IGoodProcessor _goodProcessor;

	private DistrictBuilding _districtBuilding;

	private DistrictResourceCounter _districtResourceCounter;

	public void Awake()
	{
		_districtBuilding = GetComponent<DistrictBuilding>();
		_goodProcessor = GetComponent<IGoodProcessor>();
	}

	public void OnEnterFinishedState()
	{
		if ((bool)_districtBuilding)
		{
			AddRegisteredGoodProcessor();
			_districtBuilding.ReassignedDistrict += OnReassignedDistrict;
		}
	}

	public void OnExitFinishedState()
	{
		if ((bool)_districtBuilding)
		{
			RemoveRegisteredGoodProcessor();
			_districtBuilding.ReassignedDistrict -= OnReassignedDistrict;
		}
	}

	private void OnReassignedDistrict(object sender, EventArgs e)
	{
		RemoveRegisteredGoodProcessor();
		AddRegisteredGoodProcessor();
	}

	private void RemoveRegisteredGoodProcessor()
	{
		if ((bool)_districtResourceCounter)
		{
			_districtResourceCounter.Remove(_goodProcessor);
			_districtResourceCounter = null;
		}
	}

	private void AddRegisteredGoodProcessor()
	{
		DistrictCenter districtCenter = _districtBuilding?.District;
		if ((bool)districtCenter)
		{
			_districtResourceCounter = districtCenter.GetComponent<DistrictResourceCounter>();
			_districtResourceCounter.Add(_goodProcessor);
		}
	}
}
