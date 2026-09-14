using Timberborn.Automation;
using Timberborn.AutomationBuildings;
using Timberborn.AutomationUI;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class MemoryFragment : IEntityPanelFragment
{
	private static readonly string RelayModeLocKeyPrefix = "Building.Memory.Mode.";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly EnumDropdownProviderFactory _enumDropdownProviderFactory;

	private readonly TransmitterSelectorInitializer _transmitterSelectorInitializer;

	private readonly MemoryModeDescriptions _memoryModeDescriptions;

	private EnumDropdownProvider<MemoryMode> _modeDropdownProvider;

	private TransmitterSelector _inputASelector;

	private TransmitterSelector _inputBSelector;

	private TransmitterSelector _resetInputSelector;

	private VisualElement _root;

	private Dropdown _modeDropdown;

	private Label _modeDescription;

	private Memory _memory;

	private bool _showInputB;

	private Automator _lastInputB;

	public MemoryFragment(VisualElementLoader visualElementLoader, DropdownItemsSetter dropdownItemsSetter, EnumDropdownProviderFactory enumDropdownProviderFactory, TransmitterSelectorInitializer transmitterSelectorInitializer, MemoryModeDescriptions memoryModeDescriptions)
	{
		_visualElementLoader = visualElementLoader;
		_dropdownItemsSetter = dropdownItemsSetter;
		_enumDropdownProviderFactory = enumDropdownProviderFactory;
		_transmitterSelectorInitializer = transmitterSelectorInitializer;
		_memoryModeDescriptions = memoryModeDescriptions;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/MemoryFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_modeDropdown = _root.Q<Dropdown>("Mode");
		_modeDropdownProvider = _enumDropdownProviderFactory.CreateLocalized(() => _memory.Mode, delegate(MemoryMode relayMode)
		{
			_memory.SetMode(relayMode);
		}, RelayModeLocKeyPrefix);
		_inputASelector = _root.Q<TransmitterSelector>("InputA");
		_transmitterSelectorInitializer.Initialize(_inputASelector, () => _memory.InputA, delegate(Automator automator)
		{
			_memory.SetInputA(automator);
		});
		_inputBSelector = _root.Q<TransmitterSelector>("InputB");
		_transmitterSelectorInitializer.Initialize(_inputBSelector, () => _memory.InputB, delegate(Automator automator)
		{
			_memory.SetInputB(automator);
		});
		_resetInputSelector = _root.Q<TransmitterSelector>("ResetInput");
		_transmitterSelectorInitializer.InitializeOptional(_resetInputSelector, () => _memory.ResetInput, delegate(Automator automator)
		{
			_memory.SetResetInput(automator);
		});
		_modeDescription = _root.Q<Label>("ModeDescription");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		if (entity.TryGetComponent<Memory>(out _memory))
		{
			_root.ToggleDisplayStyle(visible: true);
			_dropdownItemsSetter.SetItems(_modeDropdown, _modeDropdownProvider);
			_inputASelector.Show(_memory);
			_inputBSelector.Show(_memory);
			_resetInputSelector.Show(_memory);
			_showInputB = _memory.UsesInputB;
		}
	}

	public void UpdateFragment()
	{
		if (!_memory)
		{
			return;
		}
		_inputASelector.UpdateStateIcon();
		if (_showInputB != _memory.UsesInputB)
		{
			_showInputB = _memory.UsesInputB;
			if (_showInputB)
			{
				if ((bool)_lastInputB)
				{
					_memory.SetInputB(_lastInputB);
				}
				_inputBSelector.UpdateSelectedValue();
			}
		}
		if (_showInputB)
		{
			_inputBSelector.ToggleDisplayStyle(visible: true);
			_inputBSelector.UpdateStateIcon();
			_lastInputB = _memory.InputB;
		}
		else
		{
			_inputBSelector.ToggleDisplayStyle(visible: false);
		}
		_resetInputSelector.UpdateStateIcon();
		_modeDescription.text = _memoryModeDescriptions.GetDescription(_memory.Mode);
	}

	public void ClearFragment()
	{
		_memory = null;
		_lastInputB = null;
		_modeDropdown.ClearItems();
		_inputASelector.ClearItems();
		_inputBSelector.ClearItems();
		_resetInputSelector.ClearItems();
		_root.ToggleDisplayStyle(visible: false);
	}
}
