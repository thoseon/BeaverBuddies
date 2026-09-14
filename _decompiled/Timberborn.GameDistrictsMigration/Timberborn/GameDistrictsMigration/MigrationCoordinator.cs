using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.GameDistricts;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;

namespace Timberborn.GameDistrictsMigration;

public class MigrationCoordinator : ILoadableSingleton, ITickableSingleton
{
	private readonly EventBus _eventBus;

	private readonly MigrationNeighbours _migrationNeighbours;

	private readonly ISpecService _specService;

	private readonly HashSet<PopulationDistributor> _populationDistributorsToCheck = new HashSet<PopulationDistributor>();

	private readonly List<PopulationDistributor> _populationDistributorsProcessed = new List<PopulationDistributor>();

	private int _maxAutomaticMigration;

	private bool _migrating;

	public MigrationCoordinator(EventBus eventBus, MigrationNeighbours migrationNeighbours, ISpecService specService)
	{
		_eventBus = eventBus;
		_migrationNeighbours = migrationNeighbours;
		_specService = specService;
	}

	public void Load()
	{
		MigrationCoordinatorSpec singleSpec = _specService.GetSingleSpec<MigrationCoordinatorSpec>();
		_maxAutomaticMigration = singleSpec.MaxAutomaticMigration;
	}

	public void RegisterDistributorToCheck(PopulationDistributor populationDistributor)
	{
		_populationDistributorsToCheck.Add(populationDistributor);
	}

	public void Tick()
	{
		CheckMigrationTriggers();
	}

	public void ProcessAutomaticMigration(PopulationDistributor populationDistributor)
	{
		_migrating = false;
		ProcessAutomaticMigrationInternal(populationDistributor);
		FinalizeMigrationProcess();
	}

	private void CheckMigrationTriggers()
	{
		if (_populationDistributorsToCheck.Count > 0)
		{
			RunMigrationTriggers();
		}
	}

	private void RunMigrationTriggers()
	{
		_populationDistributorsToCheck.CopyTo(_populationDistributorsProcessed);
		_populationDistributorsToCheck.Clear();
		for (int i = 0; i < _populationDistributorsProcessed.Count; i++)
		{
			PopulationDistributor populationDistributor = _populationDistributorsProcessed[i];
			if ((bool)populationDistributor && !populationDistributor.Deleted)
			{
				ProcessAutomaticMigration(populationDistributor);
			}
		}
		_populationDistributorsProcessed.Clear();
	}

	private void ProcessAutomaticMigrationInternal(PopulationDistributor populationDistributor)
	{
		if (populationDistributor.CanImmigrate)
		{
			ProcessAutomaticImmigration(populationDistributor);
		}
		else if (populationDistributor.CanEmigrate)
		{
			ProcessAutomaticEmigration(populationDistributor);
		}
	}

	private void ProcessAutomaticImmigration(PopulationDistributor populationDistributor)
	{
		for (int i = 0; i < _maxAutomaticMigration; i++)
		{
			if (populationDistributor.Need <= 0)
			{
				break;
			}
			AutomaticImmigration(populationDistributor);
		}
	}

	private void ProcessAutomaticEmigration(PopulationDistributor populationDistributor)
	{
		for (int i = 0; i < _maxAutomaticMigration; i++)
		{
			if (populationDistributor.Spare <= 0)
			{
				break;
			}
			AutomaticEmigration(populationDistributor);
		}
	}

	private void AutomaticImmigration(PopulationDistributor populationDistributor)
	{
		PopulationDistributor highestSpareNeighbour = _migrationNeighbours.GetHighestSpareNeighbour(populationDistributor);
		if ((bool)highestSpareNeighbour)
		{
			Migrate(highestSpareNeighbour, populationDistributor, 1);
		}
	}

	private void AutomaticEmigration(PopulationDistributor populationDistributor)
	{
		PopulationDistributor lowestSpareNeighbour = _migrationNeighbours.GetLowestSpareNeighbour(populationDistributor);
		if ((bool)lowestSpareNeighbour)
		{
			Migrate(populationDistributor, lowestSpareNeighbour, 1);
		}
	}

	private void Migrate(PopulationDistributor from, PopulationDistributor to, int amount)
	{
		if (amount > 0)
		{
			_migrating = true;
			from.MigrateTo(to.DistrictCenter, amount);
		}
	}

	private void FinalizeMigrationProcess()
	{
		if (_migrating)
		{
			_eventBus.Post(new MigrationEvent());
			_migrating = false;
			CheckMigrationTriggers();
		}
	}
}
