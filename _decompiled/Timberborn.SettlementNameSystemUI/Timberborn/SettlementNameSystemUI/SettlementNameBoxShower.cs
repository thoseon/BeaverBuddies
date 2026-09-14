using Timberborn.CoreUI;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.GameStartup;
using Timberborn.GameWonderCompletion;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.SettlementNameSystemUI;

public class SettlementNameBoxShower : ISettlementNamePromptShower
{
	private static readonly int CharacterLimit = 50;

	private readonly PanelStack _panelStack;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly GameSaveRepository _gameSaveRepository;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly GameWonderCompletionService _gameWonderCompletionService;

	private readonly EventBus _eventBus;

	private readonly InputService _inputService;

	private string _initialSettlementName;

	public SettlementNameBoxShower(PanelStack panelStack, VisualElementLoader visualElementLoader, GameSaveRepository gameSaveRepository, DialogBoxShower dialogBoxShower, GameWonderCompletionService gameWonderCompletionService, EventBus eventBus, InputService inputService)
	{
		_panelStack = panelStack;
		_visualElementLoader = visualElementLoader;
		_gameSaveRepository = gameSaveRepository;
		_dialogBoxShower = dialogBoxShower;
		_gameWonderCompletionService = gameWonderCompletionService;
		_eventBus = eventBus;
		_inputService = inputService;
	}

	public void PromptDisallowingCancelling(bool includeResetStartLocationLink)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/SettlementNameBox");
		SettlementNameBox settlementNameBox = new SettlementNameBox(_panelStack, _gameSaveRepository, _dialogBoxShower, delegate(string settlementName)
		{
			_eventBus.Post(new SettlementNameChangedEvent(settlementName));
		}, visualElement, _initialSettlementName);
		TextField textField = visualElement.Q<TextField>("Input");
		textField.maxLength = CharacterLimit;
		textField.Q<TextElement>().RegisterCallback<FocusOutEvent>(delegate
		{
			if (_inputService.WasConfirmPressedLastFrame)
			{
				settlementNameBox.OnUIConfirmed();
			}
		});
		visualElement.Q<Button>("ConfirmButton").RegisterCallback<ClickEvent>(delegate
		{
			settlementNameBox.OnUIConfirmed();
		});
		Button button = visualElement.Q<Button>("RelocateButton");
		button.ToggleDisplayStyle(_gameWonderCompletionService.IsWonderCompletedWithAnyFaction());
		button.RegisterCallback<ClickEvent>(delegate
		{
			OnRelocateClicked(settlementNameBox);
		});
		Button button2 = visualElement.Q<Button>("ResetStartLocation");
		button2.ToggleDisplayStyle(includeResetStartLocationLink);
		button2.RegisterCallback<ClickEvent>(delegate
		{
			OnResetLocationClicked(settlementNameBox);
		});
		_panelStack.PushOverlay(settlementNameBox);
		textField.Focus();
	}

	private void OnResetLocationClicked(IPanelController settlementNameBox)
	{
		_panelStack.Pop(settlementNameBox);
		_eventBus.Post(new ResetStartingLocationEvent());
	}

	private void OnRelocateClicked(SettlementNameBox settlementNameBox)
	{
		_initialSettlementName = settlementNameBox.SettlementName;
		_panelStack.Pop(settlementNameBox);
		_eventBus.Post(new RelocateSettlementEvent());
	}
}
