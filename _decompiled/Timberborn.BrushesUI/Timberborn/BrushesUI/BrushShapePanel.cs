using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

internal class BrushShapePanel : IToolFragment, IInputProcessor
{
	private static readonly string ToggleBrushShapeKey = "ToggleBrushShape";

	private readonly EventBus _eventBus;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly InputService _inputService;

	private readonly ILoc _loc;

	private readonly KeyBindingShortcutService _keyBindingShortcutService;

	private VisualElement _root;

	private VisualElement _togglesContainer;

	private IBrushWithShape _brushWithShape;

	private readonly Dictionary<BrushShape, Toggle> _toggles = new Dictionary<BrushShape, Toggle>();

	private ImmutableArray<BrushShape> _brushShapeValues;

	public BrushShapePanel(EventBus eventBus, VisualElementLoader visualElementLoader, InputService inputService, ILoc loc, KeyBindingShortcutService keyBindingShortcutService)
	{
		_eventBus = eventBus;
		_visualElementLoader = visualElementLoader;
		_inputService = inputService;
		_loc = loc;
		_keyBindingShortcutService = keyBindingShortcutService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("MapEditor/ToolPanel/BrushShapePanel");
		_togglesContainer = _root.Q<VisualElement>("Toggles");
		_keyBindingShortcutService.CreateAny(_root.Q<Label>("Binding"), ToggleBrushShapeKey);
		_root.ToggleDisplayStyle(visible: false);
		_eventBus.Register(this);
		_brushShapeValues = Enum.GetValues(typeof(BrushShape)).Cast<BrushShape>().ToImmutableArray();
		InitializeToggles();
		return _root;
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		_brushWithShape = toolEnteredEvent.Tool as IBrushWithShape;
		if (_brushWithShape != null)
		{
			_inputService.AddInputProcessor(this);
			UpdateValue();
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		if (_brushWithShape != null)
		{
			_inputService.RemoveInputProcessor(this);
			_root.ToggleDisplayStyle(visible: false);
			_brushWithShape = null;
		}
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(ToggleBrushShapeKey))
		{
			ToggleBrushShape();
			UpdateValue();
		}
		return false;
	}

	private void InitializeToggles()
	{
		foreach (BrushShape brushShapeValue in _brushShapeValues)
		{
			AddToggle(brushShapeValue);
		}
	}

	private void AddToggle(BrushShape brushShape)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("MapEditor/ToolPanel/ToolPanelToggle");
		Toggle toggle = visualElement.Q<Toggle>("ToolPanelToggle");
		toggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> evt)
		{
			OnValueChanged(evt, brushShape);
		});
		toggle.text = _loc.T(brushShape.GetLocKey());
		_toggles.Add(brushShape, toggle);
		_togglesContainer.Add(visualElement);
	}

	private void OnValueChanged(ChangeEvent<bool> changeEvent, BrushShape brushShape)
	{
		if (changeEvent.newValue)
		{
			_brushWithShape.BrushShape = brushShape;
		}
		UpdateValue();
	}

	private void UpdateValue()
	{
		foreach (BrushShape key in _toggles.Keys)
		{
			_toggles[key].SetValueWithoutNotify(_brushWithShape.BrushShape == key);
		}
	}

	private void ToggleBrushShape()
	{
		BrushShape brushShape = _brushWithShape.BrushShape;
		int index = (_brushShapeValues.IndexOf(brushShape) + 1) % _brushShapeValues.Length;
		BrushShape brushShape2 = _brushShapeValues[index];
		_brushWithShape.BrushShape = brushShape2;
	}
}
