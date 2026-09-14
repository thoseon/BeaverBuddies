using System;
using Timberborn.Automation;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.AutomationUI;

internal class AutomatableBatchControlRowItem : IBatchControlRowItem, IInitializableBatchControlItem, IClearableBatchControlRowItem
{
	private readonly Automatable _automatable;

	private readonly AutomationStateIcon _automationStateIcon;

	public VisualElement Root { get; }

	public AutomatableBatchControlRowItem(VisualElement root, Automatable automatable, AutomationStateIcon automationStateIcon)
	{
		Root = root;
		_automatable = automatable;
		_automationStateIcon = automationStateIcon;
	}

	public void InitializeRowItem()
	{
		_automatable.InputStateChanged += OnAutomatableInputStateChanged;
		UpdateItemState();
	}

	public void ClearRowItem()
	{
		_automatable.InputStateChanged -= OnAutomatableInputStateChanged;
	}

	private void OnAutomatableInputStateChanged(object sender, EventArgs e)
	{
		UpdateItemState();
	}

	private void UpdateItemState()
	{
		if (_automatable.IsAutomated)
		{
			Root.ToggleDisplayStyle(visible: true);
			_automationStateIcon.Update();
		}
		else
		{
			Root.ToggleDisplayStyle(visible: false);
		}
	}
}
