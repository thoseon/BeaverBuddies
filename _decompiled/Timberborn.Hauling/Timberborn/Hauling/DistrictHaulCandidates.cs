using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.WorkSystem;

namespace Timberborn.Hauling;

internal class DistrictHaulCandidates : BaseComponent, IAwakableComponent
{
	private readonly HashSet<HaulCandidate> _haulCandidates = new HashSet<HaulCandidate>();

	private readonly HashSet<HaulingCenter> _haulingCenters = new HashSet<HaulingCenter>();

	private readonly List<WeightedBehavior> _weightedBehaviors = new List<WeightedBehavior>();

	public bool HasHaulingCenter => _haulingCenters.Count > 0;

	public event EventHandler HasHaulingCenterChanged;

	public void Awake()
	{
		DistrictBuildingRegistry component = GetComponent<DistrictBuildingRegistry>();
		component.FinishedBuildingRegistered += OnFinishedBuildingRegistered;
		component.FinishedBuildingUnregistered += OnFinishedBuildingUnregistered;
	}

	public void GetWorkplaceBehaviorsOrdered(IList<WorkplaceBehavior> workplaceBehaviors)
	{
		foreach (HaulCandidate haulCandidate in _haulCandidates)
		{
			haulCandidate.GetWeightedBehaviors(_weightedBehaviors);
		}
		_weightedBehaviors.Sort((WeightedBehavior a, WeightedBehavior b) => b.Weight.CompareTo(a.Weight));
		foreach (WeightedBehavior weightedBehavior in _weightedBehaviors)
		{
			workplaceBehaviors.Add(weightedBehavior.WorkplaceBehavior);
		}
		_weightedBehaviors.Clear();
	}

	private void OnFinishedBuildingRegistered(object sender, FinishedBuildingRegisteredEventArgs e)
	{
		EntityComponent building = e.Building;
		HaulCandidate component = building.GetComponent<HaulCandidate>();
		if (component != null)
		{
			_haulCandidates.Add(component);
		}
		HaulingCenter component2 = building.GetComponent<HaulingCenter>();
		if (component2 != null)
		{
			Add(component2);
		}
	}

	private void OnFinishedBuildingUnregistered(object sender, FinishedBuildingUnregisteredEventArgs e)
	{
		EntityComponent building = e.Building;
		HaulCandidate component = building.GetComponent<HaulCandidate>();
		if (component != null)
		{
			_haulCandidates.Remove(component);
		}
		HaulingCenter component2 = building.GetComponent<HaulingCenter>();
		if (component2 != null)
		{
			Remove(component2);
		}
	}

	private void Add(HaulingCenter haulingCenter)
	{
		if (_haulingCenters.Add(haulingCenter) && _haulingCenters.Count == 1)
		{
			HasHaulingCenterChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void Remove(HaulingCenter haulingCenter)
	{
		if (_haulingCenters.Remove(haulingCenter) && _haulingCenters.Count == 0)
		{
			HasHaulingCenterChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
