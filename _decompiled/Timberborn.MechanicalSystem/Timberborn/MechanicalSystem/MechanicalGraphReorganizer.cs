using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;

namespace Timberborn.MechanicalSystem;

internal class MechanicalGraphReorganizer
{
	private readonly MechanicalGraphFactory _mechanicalGraphFactory;

	private readonly HashSet<MechanicalNode> _oldGraphNodes = new HashSet<MechanicalNode>();

	private readonly HashSet<MechanicalNode> _currentGraphNodes = new HashSet<MechanicalNode>();

	public MechanicalGraphReorganizer(MechanicalGraphFactory mechanicalGraphFactory)
	{
		_mechanicalGraphFactory = mechanicalGraphFactory;
	}

	public void Reorganize(MechanicalGraph mechanicalGraph)
	{
		_oldGraphNodes.AddRange(mechanicalGraph.Nodes);
		while (!_oldGraphNodes.IsEmpty())
		{
			MechanicalGraph graph = _mechanicalGraphFactory.Create();
			_currentGraphNodes.Add(_oldGraphNodes.First());
			while (!_currentGraphNodes.IsEmpty())
			{
				ProcessNodeAndVisitConnected(graph, _currentGraphNodes.First());
			}
		}
	}

	private void ProcessNodeAndVisitConnected(MechanicalGraph graph, MechanicalNode node)
	{
		_oldGraphNodes.Remove(node);
		_currentGraphNodes.Remove(node);
		graph.AddNode(node);
		VisitConnectedNodes(node);
	}

	private void VisitConnectedNodes(MechanicalNode node)
	{
		foreach (MechanicalNode item in node.Transputs.Select((Transput transput) => transput.ConnectedNode).Intersect(_oldGraphNodes))
		{
			if (_oldGraphNodes.Contains(item))
			{
				_currentGraphNodes.Add(item);
			}
		}
	}
}
