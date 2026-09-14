using System;
using Timberborn.ApplicationLifetime;
using Timberborn.CoreUI;
using Timberborn.KeyBindingSystemUI;
using Timberborn.MainMenuSceneLoading;
using Timberborn.MapEditorPersistenceUI;
using Timberborn.Options;
using Timberborn.SettingsSystemUI;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorUI;

internal class MapEditorOptionsBox : IOptionsBox, IPanelController, ILoadableSingleton
{
	private static readonly string ExitChangesLostPromptLocKey = "Menu.ExitChangesLostPrompt";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly MainMenuSceneLoader _mainMenuSceneLoader;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly ISettingsController _settingsController;

	private readonly KeyBindingsBox _keyBindingsBox;

	private readonly PanelStack _panelStack;

	private readonly MapSaverLoader _mapSaverLoader;

	private VisualElement _root;

	public MapEditorOptionsBox(VisualElementLoader visualElementLoader, MainMenuSceneLoader mainMenuSceneLoader, DialogBoxShower dialogBoxShower, ISettingsController settingsController, KeyBindingsBox keyBindingsBox, PanelStack panelStack, MapSaverLoader mapSaverLoader)
	{
		_visualElementLoader = visualElementLoader;
		_mainMenuSceneLoader = mainMenuSceneLoader;
		_dialogBoxShower = dialogBoxShower;
		_settingsController = settingsController;
		_keyBindingsBox = keyBindingsBox;
		_panelStack = panelStack;
		_mapSaverLoader = mapSaverLoader;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("MapEditor/MapEditorOptionsBox");
		_root.Q<Button>("Resume").RegisterCallback<ClickEvent>(OnResumeClicked);
		_root.Q<Button>("Save").RegisterCallback<ClickEvent>(OnSaveClicked);
		_root.Q<Button>("SaveAs").RegisterCallback<ClickEvent>(OnSaveAsClicked);
		_root.Q<Button>("Load").RegisterCallback<ClickEvent>(OnLoadClicked);
		_root.Q<Button>("NewMap").RegisterCallback<ClickEvent>(OnNewMapClicked);
		_root.Q<Button>("Bindings").RegisterCallback<ClickEvent>(OnBindingsClicked);
		_root.Q<Button>("Settings").RegisterCallback<ClickEvent>(OnSettingsClicked);
		_root.Q<Button>("ExitToMenu").RegisterCallback<ClickEvent>(OnExitToMenuClicked);
		_root.Q<Button>("ExitToDesktop").RegisterCallback<ClickEvent>(OnExitToDesktopClicked);
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public void Show()
	{
		_panelStack.PushOverlay(this);
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
		Close();
	}

	private void OnResumeClicked(ClickEvent evt)
	{
		Close();
	}

	private void OnSaveClicked(ClickEvent evt)
	{
		_mapSaverLoader.Save();
	}

	private void OnSaveAsClicked(ClickEvent evt)
	{
		_mapSaverLoader.SaveAs();
	}

	private void OnLoadClicked(ClickEvent evt)
	{
		_mapSaverLoader.LoadMap();
	}

	private void OnNewMapClicked(ClickEvent evt)
	{
		_mapSaverLoader.NewMap();
	}

	private void OnBindingsClicked(ClickEvent evt)
	{
		_panelStack.HideAndPushOverlay(_keyBindingsBox);
	}

	private void OnSettingsClicked(ClickEvent evt)
	{
		_panelStack.HideAndPushOverlay(_settingsController);
	}

	private void OnExitToMenuClicked(ClickEvent evt)
	{
		ShowExitDialog(_mainMenuSceneLoader.SaveAndOpenMainMenu);
	}

	private void OnExitToDesktopClicked(ClickEvent evt)
	{
		ShowExitDialog(GameQuitter.Quit);
	}

	private void Close()
	{
		_panelStack.Pop(this);
	}

	private void ShowExitDialog(Action exitAction)
	{
		_dialogBoxShower.Create().SetLocalizedMessage(ExitChangesLostPromptLocKey).SetConfirmButton(exitAction)
			.SetDefaultCancelButton()
			.Show();
	}
}
