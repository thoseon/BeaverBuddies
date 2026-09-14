using System;
using Timberborn.BlueprintSystem;
using Timberborn.Brushes;
using Timberborn.CoreUI;
using Timberborn.InputSystemUI;
using Timberborn.KeyBindingSystemUI;
using Timberborn.SingletonSystem;
using Timberborn.ToolPanelSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.BrushesUI;

internal class BrushSizePanel : IToolFragment
{
	private static readonly string IncreaseBrushSizeKey = "IncreaseBrushSize";

	private static readonly string DecreaseBrushSizeKey = "DecreaseBrushSize";

	private readonly EventBus _eventBus;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly KeyBindingShortcutService _keyBindingShortcutService;

	private readonly ISpecService _specService;

	private VisualElement _root;

	private Label _brushHeightValue;

	private IBrushWithSize _brushWithSize;

	private BindableButton _increaseButton;

	private BindableButton _decreaseButton;

	private int _maxBrushSize;

	public BrushSizePanel(EventBus eventBus, VisualElementLoader visualElementLoader, BindableButtonFactory bindableButtonFactory, KeyBindingShortcutService keyBindingShortcutService, ISpecService specService)
	{
		_eventBus = eventBus;
		_visualElementLoader = visualElementLoader;
		_bindableButtonFactory = bindableButtonFactory;
		_keyBindingShortcutService = keyBindingShortcutService;
		_specService = specService;
	}

	public VisualElement InitializeFragment()
	{
		BrushesSpec singleSpec = _specService.GetSingleSpec<BrushesSpec>();
		_maxBrushSize = singleSpec.MaxBrushSize;
		_root = _visualElementLoader.LoadVisualElement("MapEditor/ToolPanel/BrushSizePanel");
		_brushHeightValue = _root.Q<Label>("Height");
		_increaseButton = _bindableButtonFactory.Create(_root.Q<Button>("Plus"), IncreaseBrushSizeKey, IncreaseBrushSize, blockInput: false);
		_decreaseButton = _bindableButtonFactory.Create(_root.Q<Button>("Minus"), DecreaseBrushSizeKey, DecreaseBrushSize, blockInput: false);
		_keyBindingShortcutService.CreateAny(_root.Q<Label>("Increase"), IncreaseBrushSizeKey);
		_keyBindingShortcutService.CreateAny(_root.Q<Label>("Decrease"), DecreaseBrushSizeKey);
		_root.ToggleDisplayStyle(visible: false);
		_eventBus.Register(this);
		return _root;
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		_brushWithSize = toolEnteredEvent.Tool as IBrushWithSize;
		if (_brushWithSize != null)
		{
			_root.ToggleDisplayStyle(visible: true);
			_increaseButton.Bind();
			_decreaseButton.Bind();
			UpdateBrushSizeValue();
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		_root.ToggleDisplayStyle(visible: false);
		_increaseButton.Unbind();
		_decreaseButton.Unbind();
	}

	private void IncreaseBrushSize()
	{
		_brushWithSize.BrushSize = Math.Min(_brushWithSize.BrushSize + 1, _maxBrushSize);
		UpdateBrushSizeValue();
	}

	private void DecreaseBrushSize()
	{
		_brushWithSize.BrushSize = Math.Max(_brushWithSize.BrushSize - 1, 1);
		UpdateBrushSizeValue();
	}

	private void UpdateBrushSizeValue()
	{
		_brushHeightValue.text = _brushWithSize.BrushSize.ToString();
	}
}
