using System;
using Timberborn.BaseComponentSystem;
using Timberborn.GameDistricts;
using Timberborn.PopulationStatisticsSystem;

namespace Timberborn.BeaverContaminationSystem;

public class DistrictBeaverContaminationStatisticsProvider : BaseComponent, IAwakableComponent, IContaminationStatisticsProvider
{
	private readonly BeaverContaminationRegistry _beaverContaminationRegistry = new BeaverContaminationRegistry();

	public event EventHandler ContaminationStatisticsChanged;

	public void Awake()
	{
		DistrictPopulation component = GetComponent<DistrictPopulation>();
		component.CitizenAssigned += OnCitizenAssigned;
		component.CitizenUnassigned += OnCitizenUnassigned;
	}

	public BeaverContaminationStatistics GetContaminationStatistics()
	{
		return new BeaverContaminationStatistics(_beaverContaminationRegistry.NumberOfContaminatedAdults, _beaverContaminationRegistry.NumberOfContaminatedChildren);
	}

	private void OnCitizenAssigned(object sender, CitizenAssignedEventArgs citizenAssignedEventArgs)
	{
		Contaminable component = citizenAssignedEventArgs.Citizen.GetComponent<Contaminable>();
		if (component != null)
		{
			_beaverContaminationRegistry.AddContaminable(component);
			component.ContaminationChanged += OnContaminationChanged;
		}
	}

	private void OnCitizenUnassigned(object sender, CitizenUnassignedEventArgs citizenUnassignedEventArgs)
	{
		Contaminable component = citizenUnassignedEventArgs.Citizen.GetComponent<Contaminable>();
		if (component != null)
		{
			_beaverContaminationRegistry.RemoveContaminable(component);
			component.ContaminationChanged -= OnContaminationChanged;
		}
	}

	private void OnContaminationChanged(object sender, EventArgs e)
	{
		ContaminationStatisticsChanged?.Invoke(this, EventArgs.Empty);
	}
}
