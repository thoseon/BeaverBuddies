using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.AutomationUI;

internal class TransmitterFragment : IEntityPanelFragment
{
	private static readonly string StateOffLocKey = "Automation.State.Off";

	private static readonly string StateOnLocKey = "Automation.State.On";

	private static readonly string StateErrorLocKey = "Automation.State.Error";

	private static readonly string StateProcessingLocKey = "Automation.State.Processing";

	private static readonly string UsagesLocKey = "Automation.Usages";

	private static readonly string StateLabelUnfinishedClass = "transmitter-fragment__state-label--unfinished";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly AutomationStateIconBuilder _automationStateIconBuilder;

	private readonly ILoc _loc;

	private VisualElement _root;

	private AutomationStateIcon _automationStateIcon;

	private Label _stateLabel;

	private Label _usagesLabel;

	private Automator _automator;

	public TransmitterFragment(VisualElementLoader visualElementLoader, AutomationStateIconBuilder automationStateIconBuilder, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_automationStateIconBuilder = automationStateIconBuilder;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/TransmitterFragment");
		_root.ToggleDisplayStyle(visible: false);
		_automationStateIcon = _automationStateIconBuilder.Create(_root.Q<Image>("StateIcon"), () => _automator).Build();
		_stateLabel = _root.Q<Label>("StateLabel");
		_usagesLabel = _root.Q<Label>("Usages");
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_automator = entity.GetComponent<Automator>();
	}

	public void ClearFragment()
	{
		_automator = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_automator && _automator.IsTransmitter)
		{
			_root.ToggleDisplayStyle(visible: true);
			_stateLabel.text = GetStateText();
			_stateLabel.EnableInClassList(StateLabelUnfinishedClass, !_automator.Enabled);
			_automationStateIcon.Update();
			_usagesLabel.text = _loc.T(UsagesLocKey, _automator.Usages);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private string GetStateText()
	{
		if (_automator.IsProcessingNewInput)
		{
			return _loc.T(StateProcessingLocKey);
		}
		return _automator.UnfinishedState switch
		{
			AutomatorState.Off => _loc.T(StateOffLocKey), 
			AutomatorState.On => _loc.T(StateOnLocKey), 
			AutomatorState.Error => _loc.T(StateErrorLocKey), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
