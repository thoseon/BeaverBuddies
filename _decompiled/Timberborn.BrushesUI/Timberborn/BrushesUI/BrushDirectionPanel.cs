using Timberborn.Brushes;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.KeyBindingSystemUI;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.ToolPanelSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.BrushesUI;

internal class BrushDirectionPanel : IToolFragment, IInputProcessor
{
	private static readonly string IncreaseLocKey = "MapEditor.Brush.Direction.Raise";

	private static readonly string DecreaseLocKey = "MapEditor.Brush.Direction.Lower";

	private static readonly string InverseBrushDirectionKey = "InverseBrushDirection";

	private readonly EventBus _eventBus;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly InputService _inputService;

	private readonly ILoc _loc;

	private readonly KeyBindingShortcutService _keyBindingShortcutService;

	private VisualElement _root;

	private VisualElement _togglesContainer;

	private IBrushWithDirection _brushWithDirection;

	private Toggle _increaseToggle;

	private Toggle _decreaseToggle;

	public BrushDirectionPanel(EventBus eventBus, VisualElementLoader visualElementLoader, InputService inputService, ILoc loc, KeyBindingShortcutService keyBindingShortcutService)
	{
		_eventBus = eventBus;
		_visualElementLoader = visualElementLoader;
		_inputService = inputService;
		_loc = loc;
		_keyBindingShortcutService = keyBindingShortcutService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("MapEditor/ToolPanel/BrushDirectionPanel");
		_togglesContainer = _root.Q<VisualElement>("Toggles");
		_keyBindingShortcutService.CreateAny(_root.Q<Label>("Binding"), InverseBrushDirectionKey);
		_root.ToggleDisplayStyle(visible: false);
		_eventBus.Register(this);
		InitializeToggles();
		return _root;
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		_brushWithDirection = toolEnteredEvent.Tool as IBrushWithDirection;
		if (_brushWithDirection != null)
		{
			_inputService.AddInputProcessor(this);
			UpdateValue();
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		if (_brushWithDirection != null)
		{
			_inputService.RemoveInputProcessor(this);
			_root.ToggleDisplayStyle(visible: false);
			_brushWithDirection.Inverse = false;
			_brushWithDirection = null;
		}
	}

	public bool ProcessInput()
	{
		_brushWithDirection.Inverse = _inputService.IsKeyHeld(InverseBrushDirectionKey);
		UpdateValue();
		return false;
	}

	private void InitializeToggles()
	{
		_increaseToggle = AddToggle(increaseToggle: true);
		_decreaseToggle = AddToggle(increaseToggle: false);
	}

	private Toggle AddToggle(bool increaseToggle)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("MapEditor/ToolPanel/ToolPanelToggle");
		Toggle toggle = visualElement.Q<Toggle>("ToolPanelToggle");
		toggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> evt)
		{
			OnValueChanged(evt, increaseToggle);
		});
		toggle.text = _loc.T(increaseToggle ? IncreaseLocKey : DecreaseLocKey);
		_togglesContainer.Add(visualElement);
		return toggle;
	}

	private void OnValueChanged(ChangeEvent<bool> changeEvent, bool isIncreaseToggle)
	{
		if (changeEvent.newValue)
		{
			_brushWithDirection.Increase = isIncreaseToggle;
		}
		else
		{
			_brushWithDirection.Increase = !isIncreaseToggle;
		}
	}

	private void UpdateValue()
	{
		_increaseToggle.SetValueWithoutNotify(_brushWithDirection.IsIncreasing);
		_decreaseToggle.SetValueWithoutNotify(!_brushWithDirection.IsIncreasing);
	}
}
