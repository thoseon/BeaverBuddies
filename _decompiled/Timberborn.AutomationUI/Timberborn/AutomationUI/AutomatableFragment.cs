using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationUI;

internal class AutomatableFragment : IEntityPanelFragment
{
	private static readonly string TransmitterSelectorNoneClass = "transmitter-selector--automatable-none";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly TransmitterSelectorInitializer _transmitterSelectorInitializer;

	private VisualElement _root;

	private TransmitterSelector _inputSelector;

	private Automatable _automatable;

	public AutomatableFragment(VisualElementLoader visualElementLoader, TransmitterSelectorInitializer transmitterSelectorInitializer)
	{
		_visualElementLoader = visualElementLoader;
		_transmitterSelectorInitializer = transmitterSelectorInitializer;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/AutomatableFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_inputSelector = _root.Q<TransmitterSelector>("Input");
		_transmitterSelectorInitializer.InitializeStandalone(_inputSelector, GetInput, SetInput);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		if (entity.TryGetComponent<Automatable>(out _automatable))
		{
			_inputSelector.Show(_automatable);
			_automatable.InputReconnected += OnInputReconnected;
		}
	}

	public void UpdateFragment()
	{
		if (_automatable != null)
		{
			if (_automatable.IsNeeded())
			{
				_root.ToggleDisplayStyle(visible: true);
				_inputSelector.UpdateStateIcon();
				_inputSelector.EnableInClassList(TransmitterSelectorNoneClass, !_automatable.IsAutomated);
			}
			else
			{
				_root.ToggleDisplayStyle(visible: false);
			}
		}
	}

	public void ClearFragment()
	{
		if (_automatable != null)
		{
			_automatable.InputReconnected -= OnInputReconnected;
			_automatable = null;
		}
		_inputSelector.ClearItems();
		_root.ToggleDisplayStyle(visible: false);
	}

	private Automator GetInput()
	{
		return _automatable.Input;
	}

	private void SetInput(Automator automator)
	{
		_automatable.SetInput(automator);
	}

	private void OnInputReconnected(object sender, EventArgs e)
	{
		_inputSelector.UpdateSelectedValue();
	}
}
