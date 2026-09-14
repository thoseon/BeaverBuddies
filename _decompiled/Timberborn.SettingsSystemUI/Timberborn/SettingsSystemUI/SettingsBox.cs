using Timberborn.CoreUI;
using Timberborn.IntroSettingsSystem;
using Timberborn.KeyBindingSystemUI;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class SettingsBox : ISettingsController, IPanelController, ILoadableSingleton
{
	private readonly DevModeSettingsController _devModeSettingsController;

	private readonly GraphicsSettingsController _graphicsSettingsController;

	private readonly ScreenSettingsController _screenSettingsController;

	private readonly UISettingsController _uiSettingsController;

	private readonly TutorialSettingsController _tutorialSettingsController;

	private readonly AccessibilitySettingsController _accessibilitySettingsController;

	private readonly InputSettingsController _inputSettingsController;

	private readonly SoundSettingsController _soundSettingsController;

	private readonly GameSavingSettingsController _gameSavingSettingsController;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly LanguageSettingsController _languageSettingsController;

	private readonly KeyBindingsBox _keyBindingsBox;

	private readonly AnalyticsSettingsController _analyticsSettingsController;

	private readonly IntroSettingsController _introSettingsController;

	private readonly CameraSettingsController _cameraSettingsController;

	private VisualElement _root;

	private ScrollView _content;

	public SettingsBox(DevModeSettingsController devModeSettingsController, GraphicsSettingsController graphicsSettingsController, ScreenSettingsController screenSettingsController, UISettingsController uiSettingsController, TutorialSettingsController tutorialSettingsController, AccessibilitySettingsController accessibilitySettingsController, InputSettingsController inputSettingsController, SoundSettingsController soundSettingsController, GameSavingSettingsController gameSavingSettingsController, VisualElementLoader visualElementLoader, PanelStack panelStack, LanguageSettingsController languageSettingsController, KeyBindingsBox keyBindingsBox, AnalyticsSettingsController analyticsSettingsController, IntroSettingsController introSettingsController, CameraSettingsController cameraSettingsController)
	{
		_devModeSettingsController = devModeSettingsController;
		_graphicsSettingsController = graphicsSettingsController;
		_screenSettingsController = screenSettingsController;
		_uiSettingsController = uiSettingsController;
		_tutorialSettingsController = tutorialSettingsController;
		_accessibilitySettingsController = accessibilitySettingsController;
		_inputSettingsController = inputSettingsController;
		_soundSettingsController = soundSettingsController;
		_gameSavingSettingsController = gameSavingSettingsController;
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_languageSettingsController = languageSettingsController;
		_keyBindingsBox = keyBindingsBox;
		_analyticsSettingsController = analyticsSettingsController;
		_introSettingsController = introSettingsController;
		_cameraSettingsController = cameraSettingsController;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Options/SettingsBox");
		_content = _root.Q<ScrollView>("Content");
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		_root.Q<Button>("BindingsButton").RegisterCallback<ClickEvent>(OpenKeyBindingsBox);
		_devModeSettingsController.Initialize(_root, OnUICancelled);
		_graphicsSettingsController.Initialize(_root);
		_screenSettingsController.Initialize(_root);
		_uiSettingsController.Initialize(_root);
		_tutorialSettingsController.Initialize(_root);
		_accessibilitySettingsController.Initialize(_root);
		_inputSettingsController.Initialize(_root);
		_soundSettingsController.Initialize(_root);
		_gameSavingSettingsController.Initialize(_root);
		_languageSettingsController.Initialize(_root);
		_analyticsSettingsController.Initialize(_root);
		_introSettingsController.Initialize(_root);
		_cameraSettingsController.Initialize(_root);
	}

	public VisualElement GetPanel()
	{
		_devModeSettingsController.Update();
		_screenSettingsController.Update();
		_uiSettingsController.Update();
		_tutorialSettingsController.Update();
		_accessibilitySettingsController.Update();
		_inputSettingsController.Update();
		_soundSettingsController.Update();
		_gameSavingSettingsController.Update();
		_analyticsSettingsController.Update();
		_introSettingsController.Update();
		_cameraSettingsController.Update();
		return _root;
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
		_content.scrollOffset = Vector2.zero;
		_screenSettingsController.Clear();
		_panelStack.Pop(this);
	}

	private void OpenKeyBindingsBox(ClickEvent evt)
	{
		_panelStack.HideAndPushOverlay(_keyBindingsBox);
	}
}
