using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Timberborn.ToolSystemUI;

internal class DescriptionPanel
{
	private readonly List<Action> _updateCallbacks = new List<Action>();

	public VisualElement Root { get; }

	public DescriptionPanel(VisualElement root)
	{
		Root = root;
	}

	public void AddUpdateCallback(Action callback)
	{
		_updateCallbacks.Add(callback);
	}

	public void Update()
	{
		foreach (Action updateCallback in _updateCallbacks)
		{
			updateCallback();
		}
	}
}
