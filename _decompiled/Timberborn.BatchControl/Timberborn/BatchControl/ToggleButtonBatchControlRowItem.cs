using System;
using UnityEngine.UIElements;

namespace Timberborn.BatchControl;

internal class ToggleButtonBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private static readonly string ActiveClass = "toggle-active";

	private readonly Button _button;

	private readonly Func<bool> _stateGetter;

	public VisualElement Root { get; }

	public ToggleButtonBatchControlRowItem(VisualElement root, Button button, Func<bool> stateGetter)
	{
		Root = root;
		_button = button;
		_stateGetter = stateGetter;
	}

	public void UpdateRowItem()
	{
		_button.EnableInClassList(ActiveClass, _stateGetter());
	}
}
