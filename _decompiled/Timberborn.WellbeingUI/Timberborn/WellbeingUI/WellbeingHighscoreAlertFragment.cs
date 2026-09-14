using Timberborn.AlertPanelSystem;
using Timberborn.CoreUI;
using Timberborn.GameSound;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.Wellbeing;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

internal class WellbeingHighscoreAlertFragment : IAlertFragment
{
	private static readonly string HighscoreReachedLocKey = "Wellbeing.HighscoreReached.Short";

	private readonly AlertPanelRowFactory _alertPanelRowFactory;

	private readonly EventBus _eventBus;

	private readonly GameUISoundController _gameUISoundController;

	private readonly PopulationWellbeingBox _populationWellbeingBox;

	private readonly ILoc _loc;

	private VisualElement _root;

	public WellbeingHighscoreAlertFragment(AlertPanelRowFactory alertPanelRowFactory, EventBus eventBus, GameUISoundController gameUISoundController, PopulationWellbeingBox populationWellbeingBox, ILoc loc)
	{
		_alertPanelRowFactory = alertPanelRowFactory;
		_eventBus = eventBus;
		_gameUISoundController = gameUISoundController;
		_populationWellbeingBox = populationWellbeingBox;
		_loc = loc;
	}

	public void InitializeAlertFragment(VisualElement root)
	{
		_root = _alertPanelRowFactory.CreateClosable("WellbeingHighscore");
		_root.Q<Button>("Button").RegisterCallback<ClickEvent>(OnClicked);
		Button button = _root.Q<Button>("Close");
		button.RegisterCallback<ClickEvent>(delegate
		{
			Hide();
		});
		button.ToggleDisplayStyle(visible: true);
		_eventBus.Register(this);
		Hide();
		root.Add(_root);
	}

	public void UpdateAlertFragment()
	{
	}

	[OnEvent]
	public void OnNewWellbeingHighscore(NewWellbeingHighscoreEvent newWellbeingHighscoreEvent)
	{
		int wellbeingHighscore = newWellbeingHighscoreEvent.WellbeingHighscore;
		_root.Q<Button>("Button").text = $"{_loc.T(HighscoreReachedLocKey)} {wellbeingHighscore}";
		_root.ToggleDisplayStyle(visible: true);
		_gameUISoundController.PlayWellbeingHighscoreSound();
	}

	private void OnClicked(ClickEvent evt)
	{
		Hide();
		_populationWellbeingBox.ShowWellbeingHighscore();
	}

	private void Hide()
	{
		_root.ToggleDisplayStyle(visible: false);
	}
}
