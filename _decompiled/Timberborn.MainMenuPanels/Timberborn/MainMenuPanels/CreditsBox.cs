using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuPanels;

public class CreditsBox : IPanelController, ILoadableSingleton, IUpdatableSingleton
{
	private static readonly float BaseSpeed = 50f;

	private static readonly float FastSpeed = 500f;

	private static readonly string ForwardCreditsKey = "ForwardCredits";

	private static readonly string RewindCreditsKey = "RewindCredits";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly MainMenuSoundController _mainMenuSoundController;

	private readonly InputService _inputService;

	private VisualElement _root;

	private VisualElement _creditsContent;

	private VisualElement _scrollViewWrapper;

	private bool _initializedOffset;

	private bool _isVisible;

	private float ViewHeight => _scrollViewWrapper.resolvedStyle.height;

	public CreditsBox(VisualElementLoader visualElementLoader, PanelStack panelStack, MainMenuSoundController mainMenuSoundController, InputService inputService)
	{
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_mainMenuSoundController = mainMenuSoundController;
		_inputService = inputService;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Options/CreditsBox");
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		_creditsContent = _root.Q("CreditsContent");
		_scrollViewWrapper = _root.Q("ScrollViewWrapper");
	}

	public VisualElement GetPanel()
	{
		_creditsContent.ToggleDisplayStyle(visible: false);
		_initializedOffset = false;
		_isVisible = true;
		_mainMenuSoundController.PlayCreditsMusic();
		return _root;
	}

	public bool OnUIConfirmed()
	{
		OnUICancelled();
		return false;
	}

	public void OnUICancelled()
	{
		_creditsContent.style.translate = Vector3.zero;
		_isVisible = false;
		_panelStack.Pop(this);
		_mainMenuSoundController.PlayThemeMusic();
	}

	public void UpdateSingleton()
	{
		if (_isVisible)
		{
			if (!_initializedOffset)
			{
				InitializeOffset();
			}
			else
			{
				ScrollCredits();
			}
		}
	}

	private void InitializeOffset()
	{
		if (!float.IsNaN(ViewHeight))
		{
			ScrollCredits(0f - ViewHeight);
			_creditsContent.ToggleDisplayStyle(visible: true);
			_initializedOffset = true;
		}
	}

	private void ScrollCredits()
	{
		float scrollSpeed = GetScrollSpeed();
		float y = _creditsContent.resolvedStyle.translate.y;
		bool flag = y >= 0f - _creditsContent.resolvedStyle.height + ViewHeight / 2f;
		bool flag2 = y < ViewHeight;
		if (((scrollSpeed > 0f) & flag) || ((scrollSpeed < 0f) & flag2))
		{
			ScrollCredits(Time.deltaTime * scrollSpeed);
		}
	}

	private void ScrollCredits(float delta)
	{
		_creditsContent.style.translate = _creditsContent.resolvedStyle.translate - new Vector3(0f, delta, 0f);
	}

	private float GetScrollSpeed()
	{
		if (_inputService.IsKeyHeld(ForwardCreditsKey))
		{
			return FastSpeed;
		}
		if (_inputService.IsKeyHeld(RewindCreditsKey))
		{
			return 0f - FastSpeed;
		}
		return BaseSpeed;
	}
}
