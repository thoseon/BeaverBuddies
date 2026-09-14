using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.GameDistrictsMigration;

public class PopulationDistributor : BaseComponent, IAwakableComponent, INamedComponent, IPersistentEntity
{
	private static readonly ComponentKey PopulationDistributorKey = new ComponentKey("PopulationDistributor");

	private static readonly PropertyKey<int> MinimumKey = new PropertyKey<int>("Minimum");

	private static readonly PropertyKey<bool> AllowEmigrationKey = new PropertyKey<bool>("AllowEmigration");

	private static readonly PropertyKey<bool> AllowImmigrationKey = new PropertyKey<bool>("AllowImmigration");

	private IDistributorTemplate _distributorTemplate;

	private readonly MigrationCoordinator _migrationCoordinator;

	private EntityComponent _entityComponent;

	public DistrictCenter DistrictCenter { get; private set; }

	public int Minimum { get; private set; }

	public bool AllowEmigration { get; private set; } = true;

	public bool AllowImmigration { get; private set; } = true;

	public string ComponentName => _distributorTemplate.ComponentName;

	public int Need => Minimum - Current;

	public int Spare => Current - Minimum;

	public int Current => _distributorTemplate.Current;

	public bool CanEmigrate
	{
		get
		{
			if (AllowEmigration)
			{
				return Spare > 0;
			}
			return false;
		}
	}

	public bool CanImmigrate
	{
		get
		{
			if (AllowImmigration)
			{
				return Need > 0;
			}
			return false;
		}
	}

	public bool Deleted => _entityComponent.Deleted;

	public PopulationDistributor(MigrationCoordinator migrationCoordinator)
	{
		_migrationCoordinator = migrationCoordinator;
	}

	public void Awake()
	{
		DistrictCenter = GetComponent<DistrictCenter>();
		_entityComponent = GetComponent<EntityComponent>();
	}

	public void Initialize(IDistributorTemplate distributorTemplate)
	{
		_distributorTemplate = distributorTemplate;
	}

	public void SetMinimumAndMigrate(int minimum)
	{
		minimum = Math.Max(minimum, 0);
		if (minimum != Minimum)
		{
			Minimum = minimum;
			_migrationCoordinator.ProcessAutomaticMigration(this);
		}
	}

	public void ToggleAllowEmigrationAndMigrate()
	{
		AllowEmigration = !AllowEmigration;
		_migrationCoordinator.ProcessAutomaticMigration(this);
	}

	public void ToggleAllowImmigrationAndMigrate()
	{
		AllowImmigration = !AllowImmigration;
		_migrationCoordinator.ProcessAutomaticMigration(this);
	}

	public PopulationDistributor GetOtherDistrictPopulationDistributor(DistrictCenter districtCenter)
	{
		return districtCenter.GetNamedComponent<PopulationDistributor>(ComponentName);
	}

	public void MigrateToAndCheckAutomaticMigration(DistrictCenter target, int amount)
	{
		MigrateTo(target, amount);
		PopulationDistributor otherDistrictPopulationDistributor = GetOtherDistrictPopulationDistributor(target);
		_migrationCoordinator.ProcessAutomaticMigration(otherDistrictPopulationDistributor);
		_migrationCoordinator.ProcessAutomaticMigration(this);
	}

	public void MigrateTo(DistrictCenter target, int amount)
	{
		_distributorTemplate.MigrateTo(target, amount);
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(PopulationDistributorKey, ComponentName);
		component.Set(MinimumKey, Minimum);
		component.Set(AllowEmigrationKey, AllowEmigration);
		component.Set(AllowImmigrationKey, AllowImmigration);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(PopulationDistributorKey, ComponentName);
		Minimum = component.Get(MinimumKey);
		AllowEmigration = component.Get(AllowEmigrationKey);
		AllowImmigration = component.Get(AllowImmigrationKey);
	}
}
