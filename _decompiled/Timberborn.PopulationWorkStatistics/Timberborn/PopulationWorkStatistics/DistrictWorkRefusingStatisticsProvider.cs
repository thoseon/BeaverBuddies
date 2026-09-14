using Timberborn.BaseComponentSystem;
using Timberborn.GameDistricts;
using Timberborn.PopulationStatisticsSystem;
using Timberborn.WorkSystem;

namespace Timberborn.PopulationWorkStatistics;

public class DistrictWorkRefusingStatisticsProvider : BaseComponent, IAwakableComponent, IWorkRefusingStatisticsProvider
{
	private readonly WorkRefuserRegistry _workRefuserRegistry = new WorkRefuserRegistry();

	public void Awake()
	{
		DistrictPopulation component = GetComponent<DistrictPopulation>();
		component.CitizenAssigned += OnCitizenAssigned;
		component.CitizenUnassigned += OnCitizenUnassigned;
	}

	public WorkRefusingStatistics GetWorkRefusingStatistics(string workerType)
	{
		return _workRefuserRegistry.GetWorkRefusingStatistics(workerType);
	}

	private void OnCitizenAssigned(object sender, CitizenAssignedEventArgs citizenAssignedEventArgs)
	{
		WorkRefuser component = citizenAssignedEventArgs.Citizen.GetComponent<WorkRefuser>();
		if (component != null)
		{
			_workRefuserRegistry.AddWorkRefuser(component);
		}
	}

	private void OnCitizenUnassigned(object sender, CitizenUnassignedEventArgs citizenUnassignedEventArgs)
	{
		WorkRefuser component = citizenUnassignedEventArgs.Citizen.GetComponent<WorkRefuser>();
		if (component != null)
		{
			_workRefuserRegistry.RemoveWorkRefuser(component);
		}
	}
}
