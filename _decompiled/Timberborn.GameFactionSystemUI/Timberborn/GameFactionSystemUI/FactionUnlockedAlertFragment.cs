using Timberborn.AlertPanelSystem;
using Timberborn.CoreUI;
using Timberborn.FactionSystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.WellbeingUI;
using UnityEngine.UIElements;

namespace Timberborn.GameFactionSystemUI;

internal class FactionUnlockedAlertFragment : IAlertFragment
{
	private static readonly string NewFactionUnlockedLocKey = "FactionSelection.NewFactionUnlocked";

	private readonly AlertPanelRowFactory _alertPanelRowFactory;

	private readonly EventBus _eventBus;

	private readonly ILoc _loc;

	private readonly PopulationWellbeingBox _populationWellbeingBox;

	private VisualElement _root;

	private FactionSpec _unlockedFaction;

	public FactionUnlockedAlertFragment(AlertPanelRowFactory alertPanelRowFactory, EventBus eventBus, ILoc loc, PopulationWellbeingBox populationWellbeingBox)
	{
		_alertPanelRowFactory = alertPanelRowFactory;
		_eventBus = eventBus;
		_loc = loc;
		_populationWellbeingBox = populationWellbeingBox;
	}

	public void InitializeAlertFragment(VisualElement root)
	{
		_root = _alertPanelRowFactory.CreateClosable("NewFactionUnlocked");
		_root.Q<Button>("Button").RegisterCallback<ClickEvent>(OnClicked);
		_eventBus.Register(this);
		root.Add(_root);
	}

	public void UpdateAlertFragment()
	{
	}

	[OnEvent]
	public void OnFactionUnlocked(FactionUnlockedEvent factionUnlockedEvent)
	{
		string value = factionUnlockedEvent.Faction.DisplayName.Value;
		_root.Q<Button>("Button").text = _loc.T(NewFactionUnlockedLocKey) + " " + value + "!";
		_root.ToggleDisplayStyle(visible: true);
		_unlockedFaction = factionUnlockedEvent.Faction;
	}

	private void OnClicked(ClickEvent evt)
	{
		_root.ToggleDisplayStyle(visible: false);
		_populationWellbeingBox.ShowUnlockedFaction(_unlockedFaction);
	}
}
