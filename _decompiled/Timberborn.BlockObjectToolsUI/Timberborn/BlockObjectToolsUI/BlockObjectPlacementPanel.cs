using System;
using Timberborn.BlockObjectTools;
using Timberborn.BlockSystem;
using Timberborn.CoreUI;
using Timberborn.InputSystemUI;
using Timberborn.KeyBindingSystemUI;
using Timberborn.SingletonSystem;
using Timberborn.ToolPanelSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.BlockObjectToolsUI;

internal class BlockObjectPlacementPanel : IToolFragment
{
	private static readonly string RotateClockwiseKey = "RotateClockwise";

	private static readonly string RotateCounterclockwiseKey = "RotateCounterclockwise";

	private static readonly string FlipKey = "Flip";

	private static readonly string UnflippedIconClass = "block-object-placement-panel__button-image--unflipped";

	private static readonly string FlippedIconClass = "block-object-placement-panel__button-image--flipped";

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly EventBus _eventBus;

	private readonly KeyBindingShortcutService _keyBindingShortcutService;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PreviewPlacement _previewPlacement;

	private BlockObjectTool _tool;

	private VisualElement _root;

	private BindableButton _rotateClockwiseBindableButton;

	private BindableButton _rotateCounterclockwiseBindableButton;

	private BindableButton _flipBindableButton;

	private Button _flipButton;

	private VisualElement _flipIcon;

	public BlockObjectPlacementPanel(BindableButtonFactory bindableButtonFactory, EventBus eventBus, KeyBindingShortcutService keyBindingShortcutService, VisualElementLoader visualElementLoader, PreviewPlacement previewPlacement)
	{
		_bindableButtonFactory = bindableButtonFactory;
		_eventBus = eventBus;
		_keyBindingShortcutService = keyBindingShortcutService;
		_visualElementLoader = visualElementLoader;
		_previewPlacement = previewPlacement;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Common/ToolPanel/BlockObjectPlacementPanel";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_rotateClockwiseBindableButton = CreateBindableButton(_root.Q<Button>("RotateClockwise"), RotateClockwiseKey, RotateClockwise);
		_rotateCounterclockwiseBindableButton = CreateBindableButton(_root.Q<Button>("RotateCounterclockwise"), RotateCounterclockwiseKey, RotateCounterclockwise);
		_flipButton = _root.Q<Button>("Flip");
		_flipIcon = _root.Q<VisualElement>("FlipIcon");
		_flipBindableButton = CreateBindableButton(_flipButton, FlipKey, Flip);
		_eventBus.Register(this);
		Hide();
		return _root;
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		_tool = toolEnteredEvent.Tool as BlockObjectTool;
		_root.ToggleDisplayStyle(_tool != null);
		if (_tool != null)
		{
			_rotateClockwiseBindableButton.Bind();
			_rotateCounterclockwiseBindableButton.Bind();
			bool flippable = _tool.Template.GetSpec<BlockObjectSpec>().Flippable;
			if (flippable)
			{
				_previewPlacement.EnableFlipping();
				_flipBindableButton.Bind();
			}
			else
			{
				_previewPlacement.DisableFlipping();
			}
			_flipButton.ToggleDisplayStyle(flippable);
			UpdateFlipIcon();
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		Hide();
		_rotateClockwiseBindableButton.Unbind();
		_rotateCounterclockwiseBindableButton.Unbind();
		_flipBindableButton.Unbind();
	}

	private BindableButton CreateBindableButton(Button button, string key, Action action)
	{
		_keyBindingShortcutService.CreateAny(button.Q<Label>("Binding"), key);
		return _bindableButtonFactory.Create(button, key, action);
	}

	private void RotateClockwise()
	{
		_previewPlacement.RotateClockwise();
	}

	private void RotateCounterclockwise()
	{
		_previewPlacement.RotateCounterclockwise();
	}

	private void Flip()
	{
		_previewPlacement.Flip();
		UpdateFlipIcon();
	}

	private void Hide()
	{
		_root.ToggleDisplayStyle(visible: false);
	}

	private void UpdateFlipIcon()
	{
		_flipIcon.EnableInClassList(UnflippedIconClass, _previewPlacement.FlipMode.IsUnflipped);
		_flipIcon.EnableInClassList(FlippedIconClass, _previewPlacement.FlipMode.IsFlipped);
	}
}
