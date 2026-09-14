using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.PrioritySystemUI;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using UnityEngine;

namespace Timberborn.BuilderPrioritySystemUI;

internal class BuilderPrioritizableHighlighter : IPostLoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly Highlighter _highlighter;

	private readonly PriorityColors _priorityColors;

	private readonly List<BuilderPrioritizable> _builderPrioritizables = new List<BuilderPrioritizable>();

	private bool _enabled;

	public BuilderPrioritizableHighlighter(EventBus eventBus, Highlighter highlighter, PriorityColors priorityColors)
	{
		_eventBus = eventBus;
		_highlighter = highlighter;
		_priorityColors = priorityColors;
	}

	public void PostLoad()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		if (_enabled)
		{
			HighlightAll();
		}
	}

	[OnEvent]
	public void OnToolGroupEntered(ToolGroupEnteredEvent toolGroupEnteredEvent)
	{
		ToolGroupSpec toolGroup = toolGroupEnteredEvent.ToolGroup;
		if ((object)toolGroup != null && toolGroup.HasSpec<BuilderPriorityToolGroupSpec>())
		{
			_enabled = true;
			HighlightAll();
		}
	}

	[OnEvent]
	public void OnToolGroupExited(ToolGroupExitedEvent toolGroupExitedEvent)
	{
		ToolGroupSpec toolGroup = toolGroupExitedEvent.ToolGroup;
		if ((object)toolGroup != null && toolGroup.HasSpec<BuilderPriorityToolGroupSpec>())
		{
			_enabled = false;
			_highlighter.UnhighlightAllSecondary();
		}
	}

	public void AddBuilderPrioritizable(BuilderPrioritizable builderPrioritizable)
	{
		_builderPrioritizables.Add(builderPrioritizable);
	}

	public void RemoveBuilderPrioritizable(BuilderPrioritizable builderPrioritizable)
	{
		_builderPrioritizables.Remove(builderPrioritizable);
	}

	public void HighlightIfEnabled(BuilderPrioritizable builderPrioritizable)
	{
		if (_enabled)
		{
			Highlight(builderPrioritizable);
		}
	}

	public void HighlightAll()
	{
		_highlighter.UnhighlightAllSecondary();
		foreach (BuilderPrioritizable builderPrioritizable in _builderPrioritizables)
		{
			Highlight(builderPrioritizable);
		}
	}

	private void Highlight(BuilderPrioritizable builderPrioritizable)
	{
		Color highlightColor = _priorityColors.GetHighlightColor(builderPrioritizable.Priority);
		_highlighter.HighlightSecondary(builderPrioritizable, highlightColor);
	}
}
