using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Beavers;
using Timberborn.Bots;
using Timberborn.Common;
using Timberborn.EntitySystem;

namespace Timberborn.GameDistricts;

public class DistrictPopulation : BaseComponent, IAwakableComponent
{
	private readonly EntityComponentRegistryFactory _entityComponentRegistryFactory;

	private EntityComponentRegistry _entityComponentRegistry;

	private readonly BeaverCollection _beaverCollection = new BeaverCollection();

	private readonly List<Bot> _bots = new List<Bot>();

	public ReadOnlyList<Bot> Bots => _bots.AsReadOnlyList();

	public ReadOnlyList<Beaver> Adults => _beaverCollection.Adults;

	public ReadOnlyList<Beaver> Children => _beaverCollection.Children;

	public ReadOnlyList<Beaver> Beavers => _beaverCollection.Beavers;

	public int NumberOfAdults => _beaverCollection.NumberOfAdults;

	public int NumberOfChildren => _beaverCollection.NumberOfChildren;

	public int NumberOfBots => _bots.Count;

	public event EventHandler<CitizenAssignedEventArgs> CitizenAssigned;

	public event EventHandler<CitizenUnassignedEventArgs> CitizenUnassigned;

	public DistrictPopulation(EntityComponentRegistryFactory entityComponentRegistryFactory)
	{
		_entityComponentRegistryFactory = entityComponentRegistryFactory;
	}

	public void Awake()
	{
		_entityComponentRegistry = _entityComponentRegistryFactory.Create();
	}

	public IEnumerable<T> GetEnabledCharacters<T>() where T : BaseComponent, IRegisteredComponent
	{
		return _entityComponentRegistry.GetEnabled<T>();
	}

	public void AssignCitizen(Citizen citizen)
	{
		Beaver component = citizen.GetComponent<Beaver>();
		if (component != null)
		{
			_beaverCollection.AddBeaver(component);
		}
		Bot component2 = citizen.GetComponent<Bot>();
		if (component2 != null)
		{
			_bots.Add(component2);
		}
		_entityComponentRegistry.Register(citizen.GetComponent<EntityComponent>());
		CitizenAssigned?.Invoke(this, new CitizenAssignedEventArgs(citizen));
	}

	public void UnassignCitizen(Citizen citizen)
	{
		Beaver component = citizen.GetComponent<Beaver>();
		if (component != null)
		{
			_beaverCollection.RemoveBeaver(component);
		}
		Bot component2 = citizen.GetComponent<Bot>();
		if (component2 != null)
		{
			_bots.Remove(component2);
		}
		_entityComponentRegistry.Unregister(citizen.GetComponent<EntityComponent>());
		CitizenUnassigned?.Invoke(this, new CitizenUnassignedEventArgs(citizen));
	}
}
