using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class LeverFragment : IEntityPanelFragment, IInputProcessor
{
	private static readonly string SwitchOnLocKey = "Building.Lever.SwitchOn";

	private static readonly string SwitchOffLocKey = "Building.Lever.SwitchOff";

	private static readonly string UniqueBuildingActionKey = "UniqueBuildingAction";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly InputService _inputService;

	private readonly ILoc _loc;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private VisualElement _root;

	private Button _button;

	private Lever _lever;

	private Toggle _springReturnToggle;

	private Toggle _pinnedToggle;

	public LeverFragment(VisualElementLoader visualElementLoader, InputService inputService, ILoc loc, ITooltipRegistrar tooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_inputService = inputService;
		_loc = loc;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/LeverFragment");
		_root.ToggleDisplayStyle(visible: false);
		_button = _root.Q<Button>("Button");
		_button.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
		_button.RegisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
		_tooltipRegistrar.RegisterWithKeyBinding(_button, GetButtonText(), UniqueBuildingActionKey);
		_springReturnToggle = _root.Q<Toggle>("SpringReturn");
		_springReturnToggle.RegisterValueChangedCallback(OnSpringReturnChanged);
		_pinnedToggle = _root.Q<Toggle>("Pinned");
		_pinnedToggle.RegisterValueChangedCallback(OnPinnedChanged);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_lever = entity.GetComponent<Lever>();
		if ((bool)_lever)
		{
			_inputService.AddInputProcessor(this);
			UpdateButtonTooltip();
		}
	}

	public void ClearFragment()
	{
		_lever = null;
		_root.ToggleDisplayStyle(visible: false);
		_inputService.RemoveInputProcessor(this);
	}

	public void UpdateFragment()
	{
		if ((bool)_lever)
		{
			_button.text = GetButtonText();
			_springReturnToggle.SetValueWithoutNotify(_lever.IsSpringReturn);
			_pinnedToggle.SetValueWithoutNotify(_lever.IsPinned);
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(UniqueBuildingActionKey))
		{
			if (_lever.IsSpringReturn)
			{
				_lever.Press();
				return true;
			}
			_lever.Press();
			_lever.Release();
			return true;
		}
		if (_inputService.IsKeyUp(UniqueBuildingActionKey) && _lever.IsSpringReturn)
		{
			_lever.Release();
			return true;
		}
		return false;
	}

	private void UpdateButtonTooltip()
	{
		_tooltipRegistrar.RegisterWithKeyBinding(_button, GetButtonText(), UniqueBuildingActionKey);
	}

	private string GetButtonText()
	{
		Lever lever = _lever;
		if (lever == null || !lever.IsOn)
		{
			return _loc.T(SwitchOnLocKey);
		}
		return _loc.T(SwitchOffLocKey);
	}

	private void OnPointerDown(PointerDownEvent pointerDownEvent)
	{
		_lever.Press();
		UpdateButtonTooltip();
	}

	private void OnPointerUp(PointerUpEvent pointerUpEvent)
	{
		_lever.Release();
		UpdateButtonTooltip();
	}

	private void OnSpringReturnChanged(ChangeEvent<bool> evt)
	{
		_lever.SetSpringReturn(evt.newValue);
	}

	private void OnPinnedChanged(ChangeEvent<bool> evt)
	{
		_lever.SetPinned(evt.newValue);
	}
}
