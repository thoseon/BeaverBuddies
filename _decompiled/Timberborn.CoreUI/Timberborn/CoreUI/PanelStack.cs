using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.InputSystem;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.UISound;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class PanelStack : IInputProcessor
{
	private static readonly string OverlayKey = "Core/Overlay";

	private readonly UISoundController _uiSoundController;

	private readonly InputService _inputService;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly EventBus _eventBus;

	private readonly RootVisualElementProvider _rootVisualElementProvider;

	private VisualElement _root;

	private VisualElement _container;

	private readonly Stack<StackedPanel> _stack = new Stack<StackedPanel>();

	private StackedPanel TopPanel => _stack.Peek();

	public PanelStack(UISoundController uiSoundController, InputService inputService, VisualElementLoader visualElementLoader, EventBus eventBus, RootVisualElementProvider rootVisualElementProvider)
	{
		_uiSoundController = uiSoundController;
		_inputService = inputService;
		_visualElementLoader = visualElementLoader;
		_eventBus = eventBus;
		_rootVisualElementProvider = rootVisualElementProvider;
	}

	public VisualElement Initialize(string visualTreeAssetName, string containerName)
	{
		UIDocument uIDocument = _rootVisualElementProvider.CreateEmpty("PanelStack", 1);
		VisualTreeAsset visualTreeAsset = _visualElementLoader.LoadVisualTreeAsset(visualTreeAssetName);
		uIDocument.visualTreeAsset = visualTreeAsset;
		PanelTextSettings textSettings = uIDocument.panelSettings.textSettings;
		uIDocument.panelSettings.textSettings = null;
		uIDocument.panelSettings.textSettings = textSettings;
		_root = uIDocument.rootVisualElement;
		_container = _root.Q<VisualElement>(containerName);
		_eventBus.Register(this);
		return _root;
	}

	[OnEvent]
	public void OnUIVisibilityChanged(UIVisibilityChangedEvent uiVisibilityChangedEvent)
	{
		_root.ToggleDisplayStyle(uiVisibilityChangedEvent.UIVisible);
	}

	public bool ProcessInput()
	{
		if (_inputService.UICancel)
		{
			return ProcessUICancel();
		}
		if (_inputService.UIConfirm)
		{
			return ProcessUIConfirm();
		}
		return TopPanel.IsOverlay;
	}

	public void Push(IPanelController panelController)
	{
		Push(panelController, hideTop: false, showOverlay: false);
	}

	public void PushOverlay(IPanelController panelController)
	{
		Push(panelController, hideTop: false, showOverlay: true);
	}

	public void PushDialog(IPanelController panelController)
	{
		Push(panelController, hideTop: false, showOverlay: true, isDialog: true);
	}

	public void HideAndPushDialog(IPanelController panelController)
	{
		Push(panelController, hideTop: true, showOverlay: true, isDialog: true);
	}

	public void HideAndPush(IPanelController panelController)
	{
		Push(panelController, hideTop: true, showOverlay: false);
	}

	public void HideAndPushWithoutPause(IPanelController panelController)
	{
		Push(panelController, hideTop: true, showOverlay: false, isDialog: false, lockSpeed: false);
	}

	public void HideAndPushOverlay(IPanelController panelController)
	{
		Push(panelController, hideTop: true, showOverlay: true);
	}

	public void Pop(IPanelController panelController)
	{
		StackedPanel panel = _stack.Pop();
		if (panelController != panel.PanelController)
		{
			throw new ArgumentException(string.Format("{0} {1} is not on top of the stack!", "IPanelController", panelController));
		}
		Hide(panel);
		if (panel.TopHidden && _stack.Any())
		{
			ShowTop();
		}
	}

	public bool ContainsPanelBlocker()
	{
		return _stack.Any((StackedPanel stack) => stack.PanelController is IPanelBlocker);
	}

	public bool IsPanelOnTop(IPanelController panelController)
	{
		if (!_stack.IsEmpty())
		{
			return TopPanel.PanelController == panelController;
		}
		return false;
	}

	private bool ProcessUICancel()
	{
		TopPanel.PanelController.OnUICancelled();
		_uiSoundController.PlayCancelSound();
		return true;
	}

	private bool ProcessUIConfirm()
	{
		if (TopPanel.PanelController.OnUIConfirmed())
		{
			_uiSoundController.PlayClickSound();
		}
		return true;
	}

	private VisualElement GetPanel(IPanelController panelController, bool showOverlay)
	{
		if (showOverlay)
		{
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(OverlayKey);
			visualElement.Add(panelController.GetPanel());
			return visualElement;
		}
		return panelController.GetPanel();
	}

	private void Push(IPanelController panelController, bool hideTop, bool showOverlay, bool isDialog = false, bool lockSpeed = true)
	{
		if (hideTop && _stack.Any())
		{
			Hide(TopPanel);
		}
		VisualElement panel = GetPanel(panelController, showOverlay);
		StackedPanel panel2 = new StackedPanel(panelController, panel, hideTop, showOverlay, isDialog, lockSpeed);
		Push(panel2);
	}

	private void Push(StackedPanel panel)
	{
		_stack.Push(panel);
		Show(panel);
	}

	private void Show(StackedPanel panel)
	{
		VisualElement visualElement = panel.VisualElement;
		_container.focusController.focusedElement?.Blur();
		_container.Add(visualElement);
		_inputService.FlushUIInput();
		_inputService.AddInputProcessor(this);
		_eventBus.Post(new PanelShownEvent(panel.IsDialog, panel.LockSpeed));
	}

	private void Hide(StackedPanel panel)
	{
		_container.focusController.focusedElement?.Blur();
		_container.Remove(panel.VisualElement);
		_inputService.RemoveInputProcessor(this);
		_eventBus.Post(new PanelHiddenEvent(_stack.Any(), _stack.All((StackedPanel stack) => !stack.LockSpeed), panel.IsDialog));
	}

	private void ShowTop()
	{
		StackedPanel stackedPanel = _stack.Pop();
		IPanelController panelController = stackedPanel.PanelController;
		VisualElement panel = GetPanel(panelController, stackedPanel.IsOverlay);
		StackedPanel panel2 = new StackedPanel(panelController, panel, stackedPanel.TopHidden, stackedPanel.IsOverlay, stackedPanel.IsDialog, stackedPanel.LockSpeed);
		Push(panel2);
	}
}
