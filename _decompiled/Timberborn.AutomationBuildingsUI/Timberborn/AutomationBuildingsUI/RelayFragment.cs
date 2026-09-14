using System.Collections.Generic;
using Timberborn.Automation;
using Timberborn.AutomationBuildings;
using Timberborn.AutomationUI;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class RelayFragment : IEntityPanelFragment
{
	private static readonly string RelayModeLocKeyPrefix = "Building.Relay.Mode.";

	private static readonly string AvailableInputs = "ABCDEFGH";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly EnumDropdownProviderFactory _enumDropdownProviderFactory;

	private readonly TransmitterSelectorInitializer _transmitterSelectorInitializer;

	private readonly RelayModeDescriptions _relayModeDescriptions;

	private EnumDropdownProvider<RelayMode> _modeDropdownProvider;

	private readonly List<TransmitterSelector> _inputSelectors = new List<TransmitterSelector>(AvailableInputs.Length);

	private VisualElement _root;

	private Dropdown _modeDropdown;

	private Label _modeDescription;

	private Button _addInputButton;

	private Relay _relay;

	private readonly List<Automator> _lastInputs = new List<Automator>(AvailableInputs.Length - 1);

	private bool _supportsMultipleInputs;

	private int _visibleMultipleInputs;

	public RelayFragment(VisualElementLoader visualElementLoader, DropdownItemsSetter dropdownItemsSetter, EnumDropdownProviderFactory enumDropdownProviderFactory, TransmitterSelectorInitializer transmitterSelectorInitializer, RelayModeDescriptions relayModeDescriptions)
	{
		_visualElementLoader = visualElementLoader;
		_dropdownItemsSetter = dropdownItemsSetter;
		_enumDropdownProviderFactory = enumDropdownProviderFactory;
		_transmitterSelectorInitializer = transmitterSelectorInitializer;
		_relayModeDescriptions = relayModeDescriptions;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/RelayFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_modeDropdown = _root.Q<Dropdown>("Mode");
		_addInputButton = _root.Q<Button>("AddInput");
		_addInputButton.RegisterCallback<ClickEvent>(delegate
		{
			IncreaseVisibleInputs();
		});
		_modeDropdownProvider = _enumDropdownProviderFactory.CreateLocalized(() => _relay.Mode, delegate(RelayMode relayMode)
		{
			_relay.SetMode(relayMode);
		}, RelayModeLocKeyPrefix);
		for (int num = 0; num < AvailableInputs.Length; num++)
		{
			InitializeInput(num);
		}
		_modeDescription = _root.Q<Label>("ModeDescription");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		if (!entity.TryGetComponent<Relay>(out _relay))
		{
			return;
		}
		_root.ToggleDisplayStyle(visible: true);
		_dropdownItemsSetter.SetItems(_modeDropdown, _modeDropdownProvider);
		foreach (TransmitterSelector inputSelector in _inputSelectors)
		{
			inputSelector.Show(_relay);
		}
		_supportsMultipleInputs = _relay.SupportsMultipleInputs;
		_visibleMultipleInputs = _relay.Inputs.Count;
	}

	public void UpdateFragment()
	{
		if (!_relay)
		{
			return;
		}
		_inputSelectors[0].UpdateStateIcon();
		if (_supportsMultipleInputs != _relay.SupportsMultipleInputs)
		{
			UpdateSelectedValues();
		}
		if (_supportsMultipleInputs)
		{
			UpdateMultipleInputs();
		}
		else
		{
			for (int i = 1; i < _inputSelectors.Count; i++)
			{
				_inputSelectors[i].ToggleDisplayStyle(visible: false);
			}
		}
		_modeDescription.text = _relayModeDescriptions.GetDescription(_relay.Mode);
		UpdateAddInputVisibility();
		UpdateCloseIconVisibility();
	}

	public void ClearFragment()
	{
		_relay = null;
		_lastInputs.Clear();
		_modeDropdown.ClearItems();
		foreach (TransmitterSelector inputSelector in _inputSelectors)
		{
			inputSelector.ClearItems();
		}
		_root.ToggleDisplayStyle(visible: false);
	}

	private void IncreaseVisibleInputs()
	{
		_visibleMultipleInputs++;
		_relay.IncreaseInputs();
	}

	private void InitializeInput(int index)
	{
		TransmitterSelector transmitterSelector = _root.Q<TransmitterSelector>($"Input{AvailableInputs[index]}");
		_inputSelectors.Add(transmitterSelector);
		_transmitterSelectorInitializer.Initialize(transmitterSelector, () => _relay.GetInput(index), delegate(Automator automator)
		{
			_relay.SetInput(automator, index);
		}, delegate
		{
			RemoveRow(index);
		});
	}

	private void RemoveRow(int index)
	{
		_relay.RemoveInput(index);
		for (int i = index; i < _visibleMultipleInputs; i++)
		{
			_inputSelectors[i].UpdateSelectedValue();
		}
		_visibleMultipleInputs--;
	}

	private void UpdateSelectedValues()
	{
		_supportsMultipleInputs = _relay.SupportsMultipleInputs;
		if (_supportsMultipleInputs)
		{
			for (int i = 0; i < _lastInputs.Count; i++)
			{
				Automator automator = _lastInputs[i];
				_relay.SetInput(automator, i + 1);
			}
			for (int j = 1; j < _inputSelectors.Count; j++)
			{
				_inputSelectors[j].UpdateSelectedValue();
			}
		}
	}

	private void UpdateMultipleInputs()
	{
		int num = _visibleMultipleInputs - 1;
		for (int i = 1; i < _inputSelectors.Count; i++)
		{
			if (i <= num)
			{
				TransmitterSelector transmitterSelector = _inputSelectors[i];
				transmitterSelector.ToggleDisplayStyle(visible: true);
				transmitterSelector.UpdateStateIcon();
			}
			else
			{
				_inputSelectors[i].ToggleDisplayStyle(visible: false);
			}
		}
		_lastInputs.Clear();
		for (int j = 1; j < _relay.Inputs.Count; j++)
		{
			_lastInputs.Add(_relay.Inputs[j].Transmitter);
		}
	}

	private void UpdateAddInputVisibility()
	{
		bool visible = _relay.SupportsMultipleInputs && _visibleMultipleInputs < AvailableInputs.Length;
		_addInputButton.ToggleDisplayStyle(visible);
	}

	private void UpdateCloseIconVisibility()
	{
		bool flag = _relay.SupportsMultipleInputs && _visibleMultipleInputs > 2;
		foreach (TransmitterSelector inputSelector in _inputSelectors)
		{
			if (flag)
			{
				inputSelector.ShowCloseIcon();
			}
			else
			{
				inputSelector.HideCloseIcon();
			}
		}
	}
}
