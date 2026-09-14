using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.MechanicalSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.MechanicalSystemHighlighting;

internal class MechanicalGraphHighlightService : ILoadableSingleton, ILateUpdatableSingleton
{
	private readonly Highlighter _highlighter;

	private readonly EventBus _eventBus;

	private readonly ISpecService _specService;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly MechanicalGraphIterator _mechanicalGraphIterator;

	private readonly HashSet<MechanicalNode> _rootNodes = new HashSet<MechanicalNode>();

	private readonly HashSet<MechanicalNode> _graphNodes = new HashSet<MechanicalNode>();

	private Color _highlightColor;

	private bool _dirty;

	public MechanicalGraphHighlightService(Highlighter highlighter, EventBus eventBus, ISpecService specService, EntitySelectionService entitySelectionService, MechanicalGraphIterator mechanicalGraphIterator)
	{
		_highlighter = highlighter;
		_eventBus = eventBus;
		_specService = specService;
		_entitySelectionService = entitySelectionService;
		_mechanicalGraphIterator = mechanicalGraphIterator;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_highlightColor = _specService.GetSingleSpec<MechanicalNodeHighlighterSpec>().HighlightColor;
	}

	public void LateUpdateSingleton()
	{
		if (_dirty)
		{
			RefreshHighlight();
			_dirty = false;
		}
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		_dirty = true;
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		_dirty = true;
	}

	[OnEvent]
	public void OnMechanicalGraphCreated(MechanicalGraphCreatedEvent mechanicalGraphCreatedEvent)
	{
		_dirty = true;
	}

	[OnEvent]
	public void OnMechanicalGraphRemoved(MechanicalGraphRemovedEvent mechanicalGraphRemovedEvent)
	{
		_dirty = true;
	}

	[OnEvent]
	public void OnSelectableObjectSelected(SelectableObjectSelectedEvent selectableObjectSelectedEvent)
	{
		HighlightSelectedNode();
	}

	[OnEvent]
	public void OnSelectableObjectUnselectedEvent(SelectableObjectUnselectedEvent selectableObjectUnselectedEvent)
	{
		RemoveAllNodesFromHighlight();
	}

	public void AddNodeToHighlight(MechanicalNode mechanicalNode)
	{
		_rootNodes.Add(mechanicalNode);
		_dirty = true;
	}

	public void RemoveAllNodesFromHighlight()
	{
		_rootNodes.Clear();
		_dirty = true;
	}

	private void RefreshHighlight()
	{
		if (_rootNodes.Count > 0)
		{
			HighlightNetworkFromRoot();
		}
		else
		{
			_highlighter.UnhighlightAllSecondary();
		}
	}

	private void HighlightSelectedNode()
	{
		SelectableObject selectedObject = _entitySelectionService.SelectedObject;
		if ((bool)selectedObject)
		{
			MechanicalNode component = selectedObject.GetComponent<MechanicalNode>();
			if (component != null)
			{
				AddNodeToHighlight(component);
			}
		}
	}

	private void HighlightNetworkFromRoot()
	{
		_highlighter.UnhighlightAllSecondary();
		_mechanicalGraphIterator.Iterate(_rootNodes, _graphNodes, ShouldIncludeUnfinished());
		if (_graphNodes.Count <= 0)
		{
			return;
		}
		foreach (MechanicalNode graphNode in _graphNodes)
		{
			_highlighter.HighlightSecondary(graphNode, _highlightColor);
		}
		_graphNodes.Clear();
	}

	private bool ShouldIncludeUnfinished()
	{
		foreach (MechanicalNode rootNode in _rootNodes)
		{
			BlockObject component = rootNode.GetComponent<BlockObject>();
			if (component.IsUnfinished || component.IsPreview)
			{
				return true;
			}
		}
		return false;
	}
}
