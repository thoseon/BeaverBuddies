using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;

namespace Timberborn.GameDistricts;

public class DistrictBuildingRegistry : BaseComponent, IAwakableComponent
{
	private readonly EntityComponentRegistryFactory _entityComponentRegistryFactory;

	private EntityComponentRegistry _finishedBuildings;

	private EntityComponentRegistry _instantFinishedBuildings;

	public event EventHandler<FinishedBuildingRegisteredEventArgs> FinishedBuildingRegistered;

	public event EventHandler<FinishedBuildingUnregisteredEventArgs> FinishedBuildingUnregistered;

	public event EventHandler<FinishedBuildingInstantRegisteredEventArgs> FinishedBuildingInstantRegistered;

	public event EventHandler<FinishedBuildingInstantUnregisteredEventArgs> FinishedBuildingInstantUnregistered;

	public DistrictBuildingRegistry(EntityComponentRegistryFactory entityComponentRegistryFactory)
	{
		_entityComponentRegistryFactory = entityComponentRegistryFactory;
	}

	public void Awake()
	{
		_finishedBuildings = _entityComponentRegistryFactory.Create();
		_instantFinishedBuildings = _entityComponentRegistryFactory.Create();
	}

	public IEnumerable<T> GetEnabledBuildings<T>() where T : BaseComponent, IRegisteredComponent
	{
		return _finishedBuildings.GetEnabled<T>();
	}

	public void RegisterFinishedBuilding(EntityComponent entityComponent)
	{
		_finishedBuildings.Register(entityComponent);
		FinishedBuildingRegistered?.Invoke(this, new FinishedBuildingRegisteredEventArgs(entityComponent));
	}

	public void UnregisterFinishedBuilding(EntityComponent entityComponent)
	{
		_finishedBuildings.Unregister(entityComponent);
		FinishedBuildingUnregistered?.Invoke(this, new FinishedBuildingUnregisteredEventArgs(entityComponent));
	}

	public IEnumerable<T> GetEnabledBuildingsInstant<T>() where T : BaseComponent, IRegisteredComponent
	{
		return _instantFinishedBuildings.GetEnabled<T>();
	}

	public void RegisterInstantFinishedBuilding(EntityComponent entityComponent)
	{
		_instantFinishedBuildings.Register(entityComponent);
		FinishedBuildingInstantRegistered?.Invoke(this, new FinishedBuildingInstantRegisteredEventArgs(entityComponent));
	}

	public void UnregisterInstantFinishedBuilding(EntityComponent entityComponent)
	{
		_instantFinishedBuildings.Unregister(entityComponent);
		FinishedBuildingInstantUnregistered?.Invoke(this, new FinishedBuildingInstantUnregisteredEventArgs(entityComponent));
	}
}
