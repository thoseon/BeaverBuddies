using System;
using Timberborn.Brushes;
using Timberborn.CoreUI;
using Timberborn.InputSystemUI;
using Timberborn.KeyBindingSystemUI;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolPanelSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.BrushesUI;

internal class BrushHeightPanel : IToolFragment
{
	private static readonly string IncreaseBrushHeightKey = "IncreaseBrushHeight";

	private static readonly string DecreaseBrushHeightKey = "DecreaseBrushHeight";

	private readonly EventBus _eventBus;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly KeyBindingShortcutService _keyBindingShortcutService;

	private readonly MapSize _mapSize;

	private VisualElement _root;

	private Label _brushHeightValue;

	private IBrushWithHeight _brushWithHeight;

	private BindableButton _increaseButton;

	private BindableButton _decreaseButton;

	public BrushHeightPanel(EventBus eventBus, VisualElementLoader visualElementLoader, BindableButtonFactory bindableButtonFactory, KeyBindingShortcutService keyBindingShortcutService, MapSize mapSize)
	{
		_eventBus = eventBus;
		_visualElementLoader = visualElementLoader;
		_bindableButtonFactory = bindableButtonFactory;
		_keyBindingShortcutService = keyBindingShortcutService;
		_mapSize = mapSize;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("MapEditor/ToolPanel/BrushHeightPanel");
		_brushHeightValue = _root.Q<Label>("Height");
		_increaseButton = _bindableButtonFactory.Create(_root.Q<Button>("Plus"), IncreaseBrushHeightKey, IncreaseBrushHeight, blockInput: false);
		_decreaseButton = _bindableButtonFactory.Create(_root.Q<Button>("Minus"), DecreaseBrushHeightKey, DecreaseBrushHeight, blockInput: false);
		_keyBindingShortcutService.CreateAny(_root.Q<Label>("Increase"), IncreaseBrushHeightKey);
		_keyBindingShortcutService.CreateAny(_root.Q<Label>("Decrease"), DecreaseBrushHeightKey);
		_root.ToggleDisplayStyle(visible: false);
		_eventBus.Register(this);
		return _root;
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		_brushWithHeight = toolEnteredEvent.Tool as IBrushWithHeight;
		if (_brushWithHeight != null)
		{
			_root.ToggleDisplayStyle(visible: true);
			_increaseButton.Bind();
			_decreaseButton.Bind();
			UpdateBrushHeightValue();
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		_root.ToggleDisplayStyle(visible: false);
		_increaseButton.Unbind();
		_decreaseButton.Unbind();
	}

	private void IncreaseBrushHeight()
	{
		_brushWithHeight.BrushHeight = Math.Min(_brushWithHeight.BrushHeight + 1, _mapSize.MaxMapEditorTerrainHeight);
		UpdateBrushHeightValue();
	}

	private void DecreaseBrushHeight()
	{
		_brushWithHeight.BrushHeight = Math.Max(_brushWithHeight.BrushHeight - 1, _brushWithHeight.MinimumBrushHeight);
		UpdateBrushHeightValue();
	}

	private void UpdateBrushHeightValue()
	{
		_brushHeightValue.text = _brushWithHeight.BrushHeight.ToString();
	}
}
