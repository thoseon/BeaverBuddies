using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.MechanicalSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.MechanicalSystemUI;

internal class MechanicalGraphModelUpdater : ILoadableSingleton, IUpdatableSingleton
{
	private readonly EventBus _eventBus;

	private readonly HashSet<MechanicalNode> _untouchedShafts = new HashSet<MechanicalNode>();

	private readonly HashSet<MechanicalNode> _nodesToUpdate = new HashSet<MechanicalNode>();

	private readonly Queue<MechanicalNode> _intersections = new Queue<MechanicalNode>();

	private readonly HashSet<MechanicalGraph> _dirtyGraphs = new HashSet<MechanicalGraph>();

	private readonly List<Transput> _transputCache = new List<Transput>();

	public MechanicalGraphModelUpdater(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void UpdateSingleton()
	{
		if (_dirtyGraphs.Count <= 0)
		{
			return;
		}
		foreach (MechanicalGraph dirtyGraph in _dirtyGraphs)
		{
			if (dirtyGraph.Valid)
			{
				UpdateModels(dirtyGraph);
			}
		}
		_dirtyGraphs.Clear();
	}

	[OnEvent]
	public void OnMechanicalGraphGeneratorAdded(MechanicalGraphGeneratorAddedEvent mechanicalGraphGeneratorAddedEvent)
	{
		_dirtyGraphs.Add(mechanicalGraphGeneratorAddedEvent.MechanicalGraph);
	}

	[OnEvent]
	public void OnMechanicalGraphGeneratorUpdated(MechanicalGraphGeneratorUpdatedEvent mechanicalGraphGeneratorUpdatedEvent)
	{
		_dirtyGraphs.Add(mechanicalGraphGeneratorUpdatedEvent.MechanicalGraph);
	}

	private void UpdateModels(MechanicalGraph mechanicalGraph)
	{
		_untouchedShafts.Clear();
		_untouchedShafts.AddRange(mechanicalGraph.Nodes.Where((MechanicalNode node) => node.IsShaft));
		foreach (MechanicalNode untouchedShaft in _untouchedShafts)
		{
			untouchedShaft.ResetAllTransputRotations();
		}
		TraverseFromGenerators(mechanicalGraph);
		TraverseRemainingShafts();
		UpdateModels();
	}

	private void TraverseFromGenerators(MechanicalGraph mechanicalGraph)
	{
		foreach (MechanicalNode item in mechanicalGraph.Nodes.Where((MechanicalNode node) => node.IsGenerator && !node.IgnoreRotation))
		{
			TraverseToNextIntersection(item);
		}
		TraverseRemainingIntersections();
	}

	private void TraverseRemainingShafts()
	{
		while (!_untouchedShafts.IsEmpty())
		{
			TraverseToNextIntersection(_untouchedShafts.First());
			TraverseRemainingIntersections();
		}
	}

	private void TraverseRemainingIntersections()
	{
		while (!_intersections.IsEmpty())
		{
			TraverseToNextIntersection(_intersections.Dequeue());
		}
	}

	private void TraverseToNextIntersection(MechanicalNode node)
	{
		_untouchedShafts.Remove(node);
		foreach (Transput item in node.TransputsWithConnections())
		{
			TraverseToNextIntersection(item.ConnectedTransput);
		}
	}

	private void TraverseToNextIntersection(Transput transput)
	{
		while (transput != null && transput.ParentNode.IsShaft)
		{
			MechanicalNode parentNode = transput.ParentNode;
			_nodesToUpdate.Add(parentNode);
			if (transput.ConnectedTransput.ReversedRotation == transput.ReversedRotation)
			{
				transput.ReverseRotation();
			}
			Transput nextSingleTransput = GetNextSingleTransput(transput);
			if (nextSingleTransput != null)
			{
				if (nextSingleTransput.RotationMatches(transput) && nextSingleTransput.ConnectedNode.IsShaft)
				{
					nextSingleTransput.ReverseRotation();
				}
				transput = nextSingleTransput.ConnectedTransput;
				continue;
			}
			if (_untouchedShafts.Contains(parentNode))
			{
				_untouchedShafts.Remove(parentNode);
				_intersections.Enqueue(parentNode);
			}
			break;
		}
	}

	private void UpdateModels()
	{
		foreach (MechanicalNode item in _nodesToUpdate)
		{
			item.GetComponent<MechanicalModel>().UpdateModel();
		}
		_nodesToUpdate.Clear();
	}

	private Transput GetNextSingleTransput(Transput input)
	{
		foreach (Transput item in input.ParentNode.TransputsWithConnections())
		{
			if (item != input)
			{
				_transputCache.Add(item);
			}
		}
		if (_transputCache.Count == 1)
		{
			Transput result = _transputCache[0];
			_transputCache.Clear();
			return result;
		}
		_transputCache.Clear();
		return null;
	}
}
