using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BeaverContaminationSystem;
using Timberborn.Beavers;
using Timberborn.BlockSystem;
using Timberborn.Bots;
using Timberborn.GameDistricts;
using Timberborn.Navigation;

namespace Timberborn.GameDistrictsMigration;

internal class MigrationTrigger : BaseComponent, IAwakableComponent, IInstantNavMeshListener, IFinishedStateListener
{
	private readonly MigrationCoordinator _migrationCoordinator;

	private readonly INavMeshListenerEntityRegistry _navMeshListenerEntityRegistry;

	private readonly PopulationDistributorRetriever _populationDistributorRetriever;

	private PopulationDistributor _adultsDistributor;

	private PopulationDistributor _botsDistributor;

	private PopulationDistributor _childrenDistributor;

	private PopulationDistributor _contaminatedDistributor;

	public MigrationTrigger(MigrationCoordinator migrationCoordinator, INavMeshListenerEntityRegistry navMeshListenerEntityRegistry, PopulationDistributorRetriever populationDistributorRetriever)
	{
		_migrationCoordinator = migrationCoordinator;
		_navMeshListenerEntityRegistry = navMeshListenerEntityRegistry;
		_populationDistributorRetriever = populationDistributorRetriever;
	}

	public void Awake()
	{
		_adultsDistributor = _populationDistributorRetriever.GetPopulationDistributor<AdultsDistributorTemplate>(this);
		_botsDistributor = _populationDistributorRetriever.GetPopulationDistributor<BotsDistributorTemplate>(this);
		_childrenDistributor = _populationDistributorRetriever.GetPopulationDistributor<ChildrenDistributorTemplate>(this);
		_contaminatedDistributor = _populationDistributorRetriever.GetPopulationDistributor<ContaminatedDistributorTemplate>(this);
		DistrictPopulation component = GetComponent<DistrictPopulation>();
		component.CitizenAssigned += OnCitizenAssigned;
		component.CitizenUnassigned += OnCitizenUnassigned;
		GetComponent<DistrictBeaverContaminationStatisticsProvider>().ContaminationStatisticsChanged += OnContaminationStatisticsChanged;
	}

	public void OnEnterFinishedState()
	{
		_navMeshListenerEntityRegistry.RegisterInstantNavMeshListener(this);
	}

	public void OnExitFinishedState()
	{
		_navMeshListenerEntityRegistry.UnregisterInstantNavMeshListener(this);
	}

	public void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		RegisterAllDistributorsToCheck();
	}

	private void OnCitizenAssigned(object sender, CitizenAssignedEventArgs e)
	{
		RegisterDistributorToCheck(e.Citizen);
	}

	private void OnCitizenUnassigned(object sender, CitizenUnassignedEventArgs e)
	{
		RegisterDistributorToCheck(e.Citizen);
	}

	private void OnContaminationStatisticsChanged(object sender, EventArgs e)
	{
		_migrationCoordinator.RegisterDistributorToCheck(_childrenDistributor);
		_migrationCoordinator.RegisterDistributorToCheck(_adultsDistributor);
		_migrationCoordinator.RegisterDistributorToCheck(_contaminatedDistributor);
	}

	private void RegisterDistributorToCheck(Citizen citizen)
	{
		if (citizen.HasComponent<AdultSpec>())
		{
			_migrationCoordinator.RegisterDistributorToCheck(_adultsDistributor);
			_migrationCoordinator.RegisterDistributorToCheck(_contaminatedDistributor);
			return;
		}
		if (citizen.HasComponent<Child>())
		{
			_migrationCoordinator.RegisterDistributorToCheck(_childrenDistributor);
			_migrationCoordinator.RegisterDistributorToCheck(_contaminatedDistributor);
			return;
		}
		if (citizen.HasComponent<BotSpec>())
		{
			_migrationCoordinator.RegisterDistributorToCheck(_botsDistributor);
			return;
		}
		throw new ArgumentOutOfRangeException($"Unexpected citizen type: {citizen.GameObject}");
	}

	private void RegisterAllDistributorsToCheck()
	{
		_migrationCoordinator.ProcessAutomaticMigration(_adultsDistributor);
		_migrationCoordinator.ProcessAutomaticMigration(_botsDistributor);
		_migrationCoordinator.ProcessAutomaticMigration(_childrenDistributor);
		_migrationCoordinator.ProcessAutomaticMigration(_contaminatedDistributor);
	}
}
