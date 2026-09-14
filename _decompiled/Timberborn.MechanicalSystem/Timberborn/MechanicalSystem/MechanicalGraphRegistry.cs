using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.MechanicalSystem;

public class MechanicalGraphRegistry : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly List<MechanicalGraph> _mechanicalGraphs = new List<MechanicalGraph>();

	public ReadOnlyList<MechanicalGraph> MechanicalGraphs => _mechanicalGraphs.AsReadOnlyList();

	public MechanicalGraphRegistry(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void AddGraph(MechanicalGraph mechanicalGraph)
	{
		if (mechanicalGraph.Nodes.Count() == 1)
		{
			_mechanicalGraphs.Add(mechanicalGraph);
			_eventBus.Post(new MechanicalGraphCreatedEvent());
		}
	}

	public void RemoveGraph(MechanicalGraph mechanicalGraph)
	{
		if (!mechanicalGraph.Nodes.Any())
		{
			_mechanicalGraphs.Remove(mechanicalGraph);
			_eventBus.Post(new MechanicalGraphRemovedEvent());
		}
	}
}
