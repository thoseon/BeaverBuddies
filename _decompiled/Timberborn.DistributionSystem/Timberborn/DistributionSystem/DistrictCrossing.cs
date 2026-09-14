using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.LinkedBuildingSystem;
using UnityEngine;

namespace Timberborn.DistributionSystem;

public class DistrictCrossing : BaseComponent, IAwakableComponent, IFinishedStateListener, IRegisteredComponent
{
	private DistrictBuilding _districtBuilding;

	private DistrictCrossingValidator _districtCrossingValidator;

	private DistrictCrossing _linked;

	public DistrictDistributableGoodProvider DistrictDistributableGoodProvider { get; private set; }

	public bool CanExport => _districtCrossingValidator.CanExport;

	public void Awake()
	{
		_districtBuilding = GetComponent<DistrictBuilding>();
		_districtCrossingValidator = GetComponent<DistrictCrossingValidator>();
		GetComponent<LinkedBuilding>().BuildingLinked += OnBuildingLinked;
	}

	public void OnEnterFinishedState()
	{
		_districtBuilding.ReassignedDistrict += OnReassignedDistrict;
	}

	public void OnExitFinishedState()
	{
		_districtBuilding.ReassignedDistrict -= OnReassignedDistrict;
	}

	public DistributableGood GetMyDistributableGood(string goodId)
	{
		return DistrictDistributableGoodProvider.GetDistributableGoodForExport(goodId);
	}

	public bool TryGetLinkedDistrictDistributableGood(string goodId, out DistributableGood distributableGood)
	{
		return _linked.DistrictDistributableGoodProvider.TryGetDistributableGoodForImport(goodId, out distributableGood);
	}

	public void GetLinkedDistrictDistributableGoods(List<DistributableGood> distributableGoods)
	{
		_linked.DistrictDistributableGoodProvider.GetDistributableGoodsForImport(distributableGoods);
	}

	public bool CanExportGood(DistributableGood myDistributableGood, DistributableGood linkedDistributableGood)
	{
		if (myDistributableGood.CanExport)
		{
			return myDistributableGood.FillRate > linkedDistributableGood.FillRate;
		}
		return false;
	}

	public int GetAmountToExport(DistributableGood myDistributableGood, DistributableGood linkedDistributableGood)
	{
		if (myDistributableGood.Capacity <= 0)
		{
			return Math.Max(0, linkedDistributableGood.FreeCapacity);
		}
		return GetAmountToEqualizeFillRates(myDistributableGood, linkedDistributableGood);
	}

	private void OnBuildingLinked(object sender, LinkedBuilding e)
	{
		_linked = e.GetComponent<DistrictCrossing>();
	}

	private void OnReassignedDistrict(object sender, EventArgs e)
	{
		DistrictDistributableGoodProvider = (_districtBuilding.District ? _districtBuilding.District.GetComponent<DistrictDistributableGoodProvider>() : null);
	}

	private static int GetAmountToEqualizeFillRates(DistributableGood myDistributableGood, DistributableGood linkedDistributableGood)
	{
		float num = myDistributableGood.FillRate - linkedDistributableGood.FillRate;
		int capacity = myDistributableGood.Capacity;
		int capacity2 = linkedDistributableGood.Capacity;
		return Mathf.FloorToInt(Mathf.Min((float)(capacity * capacity2) * num / (float)(capacity + capacity2), myDistributableGood.MaxExportAmount));
	}
}
