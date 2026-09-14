using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.GameFactionSystem;
using Timberborn.GameOver;
using Timberborn.MainMenuSceneLoading;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameOverUI;

public class GameOverBox : IPanelController, ILoadableSingleton, IPanelBlocker
{
	private readonly PanelStack _panelStack;

	private readonly EventBus _eventBus;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly MainMenuSceneLoader _mainMenuSceneLoader;

	private readonly DevModeManager _devModeManager;

	private readonly FactionService _factionService;

	private VisualElement _root;

	public GameOverBox(PanelStack panelStack, EventBus eventBus, VisualElementLoader visualElementLoader, MainMenuSceneLoader mainMenuSceneLoader, DevModeManager devModeManager, FactionService factionService)
	{
		_panelStack = panelStack;
		_eventBus = eventBus;
		_visualElementLoader = visualElementLoader;
		_mainMenuSceneLoader = mainMenuSceneLoader;
		_devModeManager = devModeManager;
		_factionService = factionService;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/GameOverBox");
		_root.Q<Label>("Flavor").text = _factionService.Current.GameOverFlavor.Value;
		_root.Q<Label>("Info").text = _factionService.Current.GameOverMessage.Value;
		_root.Q<Button>("CloseButton").ToggleDisplayStyle(visible: false);
		_root.Q<Button>("ContinueButton").RegisterCallback<ClickEvent>(delegate
		{
			_panelStack.Pop(this);
		});
		_root.Q<Button>("ExitButton").RegisterCallback<ClickEvent>(delegate
		{
			_mainMenuSceneLoader.SaveAndOpenMainMenu();
		});
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnGameOverEvent(GameOverEvent gameOverEvent)
	{
		if (!_devModeManager.Enabled)
		{
			_panelStack.PushOverlay(this);
		}
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
	}
}
