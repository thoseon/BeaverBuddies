using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.MechanicalSystem;

internal class MechanicalGraphFactory
{
	private readonly EventBus _eventBus;

	private readonly MechanicalGraphRegistry _mechanicalGraphRegistry;

	private readonly IDayNightCycle _dayNightCycle;

	public MechanicalGraphFactory(EventBus eventBus, MechanicalGraphRegistry mechanicalGraphRegistry, IDayNightCycle dayNightCycle)
	{
		_eventBus = eventBus;
		_mechanicalGraphRegistry = mechanicalGraphRegistry;
		_dayNightCycle = dayNightCycle;
	}

	public MechanicalGraph Create()
	{
		return new MechanicalGraph(_eventBus, _mechanicalGraphRegistry, _dayNightCycle);
	}

	public void Join(IEnumerable<MechanicalGraph> graphs)
	{
		MechanicalGraph mechanicalGraph = Create();
		foreach (MechanicalGraph graph in graphs)
		{
			foreach (MechanicalNode item in graph.Nodes.ToImmutableArray())
			{
				mechanicalGraph.AddNode(item);
			}
		}
	}
}
