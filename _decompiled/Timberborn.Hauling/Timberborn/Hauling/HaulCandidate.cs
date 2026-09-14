using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using UnityEngine;

namespace Timberborn.Hauling;

public class HaulCandidate : BaseComponent, IAwakableComponent, IRegisteredComponent, IFinishedStateListener
{
	private static readonly float PriorityFactor = 0.5f;

	private HaulPrioritizable _haulPrioritizable;

	private DistrictBuilding _districtBuilding;

	private DistrictHaulCandidates _districtHaulCandidates;

	private readonly List<IHaulBehaviorProvider> _providers = new List<IHaulBehaviorProvider>();

	private readonly List<WeightedBehavior> _weightedBehaviorsCache = new List<WeightedBehavior>();

	public bool IsInHaulingCenterRange
	{
		get
		{
			if ((bool)_districtHaulCandidates)
			{
				return _districtHaulCandidates.HasHaulingCenter;
			}
			return false;
		}
	}

	public event EventHandler InHaulingCenterRangeChanged;

	public void Awake()
	{
		_haulPrioritizable = GetComponent<HaulPrioritizable>();
		_districtBuilding = GetComponent<DistrictBuilding>();
		GetComponents(_providers);
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		if ((bool)_districtBuilding)
		{
			EnableComponent();
			_districtBuilding.ReassignedDistrict += OnReassignedDistrict;
			SetDistrictHaulCandidates();
		}
	}

	public void OnExitFinishedState()
	{
		if ((bool)_districtBuilding)
		{
			DisableComponent();
			_districtBuilding.ReassignedDistrict -= OnReassignedDistrict;
		}
	}

	public void GetWeightedBehaviors(IList<WeightedBehavior> weightedBehaviors)
	{
		foreach (IHaulBehaviorProvider provider in _providers)
		{
			provider.GetWeightedBehaviors(_weightedBehaviorsCache);
			foreach (WeightedBehavior item in _weightedBehaviorsCache)
			{
				weightedBehaviors.Add(new WeightedBehavior(PrioritizeAndValidate(item.Weight), item.WorkplaceBehavior));
			}
			_weightedBehaviorsCache.Clear();
		}
	}

	private void OnReassignedDistrict(object sender, EventArgs e)
	{
		ClearDistrictHaulCandidates();
		SetDistrictHaulCandidates();
	}

	private void ClearDistrictHaulCandidates()
	{
		if ((bool)_districtHaulCandidates)
		{
			_districtHaulCandidates.HasHaulingCenterChanged -= OnHasHaulingCenterChanged;
			_districtHaulCandidates = null;
		}
	}

	private void SetDistrictHaulCandidates()
	{
		DistrictCenter district = _districtBuilding.District;
		if ((bool)district)
		{
			_districtHaulCandidates = district.GetComponent<DistrictHaulCandidates>();
			_districtHaulCandidates.HasHaulingCenterChanged += OnHasHaulingCenterChanged;
			InvokeInHaulingCenterRangeChanged();
		}
	}

	private void OnHasHaulingCenterChanged(object sender, EventArgs e)
	{
		InvokeInHaulingCenterRangeChanged();
	}

	private void InvokeInHaulingCenterRangeChanged()
	{
		InHaulingCenterRangeChanged?.Invoke(this, EventArgs.Empty);
	}

	private float PrioritizeAndValidate(float weight)
	{
		if (weight < 0f || weight > 1f)
		{
			Debug.LogWarning("weight should be between 0 and 1!");
		}
		if (_haulPrioritizable.Prioritized && (double)weight >= 0.5)
		{
			return weight + PriorityFactor;
		}
		return weight;
	}
}
