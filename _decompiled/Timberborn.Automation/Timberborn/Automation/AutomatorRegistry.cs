using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Timberborn.Common;
using Timberborn.EntityNaming;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Automation;

public class AutomatorRegistry : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly List<Automator> _automators = new List<Automator>();

	private readonly List<Automator> _transmitters = new List<Automator>();

	private readonly SortedList<NamedEntitySortingKey, string> _sortedTransmitterIds = new SortedList<NamedEntitySortingKey, string>();

	public ReadOnlyList<Automator> Automators => _automators.AsReadOnlyList();

	public ReadOnlyList<Automator> Transmitters => _transmitters.AsReadOnlyList();

	public ReadOnlyCollection<string> SortedTransmitterIds => new ReadOnlyCollection<string>(_sortedTransmitterIds.Values);

	public AutomatorRegistry(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public bool AnyTransmitters()
	{
		return _transmitters.Any();
	}

	public Automator FindTransmitterById(Guid entityId)
	{
		return _transmitters.FirstOrDefault((Automator automator) => automator.GetComponent<EntityComponent>().EntityId == entityId);
	}

	[OnEvent]
	public void OnEntityNameChangedEvent(EntityNameChangedEvent entityNameChangedEvent)
	{
		Automator component = entityNameChangedEvent.Entity.GetComponent<Automator>();
		if (component != null && component.IsTransmitter && _sortedTransmitterIds.ContainsValue(component.AutomatorId))
		{
			_sortedTransmitterIds.RemoveAt(_sortedTransmitterIds.IndexOfValue(component.AutomatorId));
			_sortedTransmitterIds.Add(component.SortingKey, component.AutomatorId);
		}
	}

	internal void Register(Automator automator)
	{
		_automators.Add(automator);
		if (automator.IsTransmitter)
		{
			_transmitters.Add(automator);
			_sortedTransmitterIds.Add(automator.SortingKey, automator.AutomatorId);
		}
	}

	internal void Unregister(Automator automator)
	{
		_automators.Remove(automator);
		if (automator.IsTransmitter)
		{
			_transmitters.Remove(automator);
			_sortedTransmitterIds.Remove(automator.SortingKey);
		}
	}
}
