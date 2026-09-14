using System.Linq;
using Timberborn.CoreUI;
using Timberborn.GameFactionSystem;
using Timberborn.GameSound;
using Timberborn.GameWonderCompletion;
using Timberborn.Localization;
using Timberborn.MapItemsUI;
using Timberborn.MapRepositorySystem;
using Timberborn.MapThumbnail;
using Timberborn.SettlementStatistics;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.GameWonderCompletionUI;

public class WonderCompletionPanel : IPanelController, ILoadableSingleton, IPanelBlocker
{
	private static readonly string WonderCompletedLocKey = "WonderCompletion.WonderCompleted";

	private readonly MapNameService _mapNameService;

	private readonly MapThumbnailCache _mapThumbnailCache;

	private readonly FactionService _factionService;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly EventBus _eventBus;

	private readonly StatisticItemFactory _statisticItemFactory;

	private readonly MapItemProvider _mapItemProvider;

	private readonly GameWonderCompletionService _gameWonderCompletionService;

	private readonly ILoc _loc;

	private readonly DelayedButtonEnabler _delayedButtonEnabler;

	private readonly GameUISoundController _gameUISoundController;

	private VisualElement _root;

	private VisualElement _mapPanel;

	private VisualElement _flexibleStartRoot;

	private VisualElement _thumbnail;

	private Label _mapMasteryLabel;

	private VisualElement _mapMasteryFactionIcon;

	private VisualElement _statisticsContainer;

	private Button _resumeButton;

	public WonderCompletionPanel(MapNameService mapNameService, MapThumbnailCache mapThumbnailCache, FactionService factionService, VisualElementLoader visualElementLoader, PanelStack panelStack, EventBus eventBus, StatisticItemFactory statisticItemFactory, IncrementalStatisticCollector incrementalStatisticCollector, MapItemProvider mapItemProvider, GameWonderCompletionService gameWonderCompletionService, ILoc loc, DelayedButtonEnabler delayedButtonEnabler, GameUISoundController gameUISoundController)
	{
		_mapNameService = mapNameService;
		_mapThumbnailCache = mapThumbnailCache;
		_factionService = factionService;
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_eventBus = eventBus;
		_statisticItemFactory = statisticItemFactory;
		_mapItemProvider = mapItemProvider;
		_gameWonderCompletionService = gameWonderCompletionService;
		_loc = loc;
		_delayedButtonEnabler = delayedButtonEnabler;
		_gameUISoundController = gameUISoundController;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/WonderCompletion/WonderCompletionPanel");
		_root.Q<Button>("ResumeButton").RegisterCallback<ClickEvent>(delegate
		{
			_panelStack.Pop(this);
		});
		_root.Q<Button>("CloseButton").ToggleDisplayStyle(visible: false);
		_mapMasteryLabel = _root.Q<Label>("MapMasteryInfo");
		_mapPanel = _root.Q<VisualElement>("MapPanel");
		_thumbnail = _root.Q<VisualElement>("ThumbnailImage");
		_flexibleStartRoot = _root.Q<VisualElement>("FlexibleStartRoot");
		_mapMasteryFactionIcon = _root.Q<VisualElement>("MapMasteryFactionIcon");
		_statisticsContainer = _root.Q<VisualElement>("StatisticsContainer");
		_resumeButton = _root.Q<Button>("ResumeButton");
		_eventBus.Register(this);
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	[OnEvent]
	public void OnWonderCompleted(WonderCompletedEvent wonderCompletedEvent)
	{
		Show();
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
	}

	private void Show()
	{
		ShowMainSection();
		ShowMapPanel();
		ShowStatistics();
		_panelStack.PushOverlay(this);
		_delayedButtonEnabler.Add(_resumeButton);
		_gameUISoundController.PlayWonderCongratulationSound();
	}

	private void ShowMainSection()
	{
		FactionWonderSpec spec = _factionService.Current.GetSpec<FactionWonderSpec>();
		_root.Q<Label>("Flavor").text = spec.WonderCompletionFlavor.Value;
		_root.Q<Label>("Congratulations").text = spec.WonderCompletionMessage.Value;
		Sprite asset = spec.WonderCompletionImage.Asset;
		_root.Q<Image>("WonderCompletionImage").style.backgroundImage = new StyleBackground(asset);
	}

	private void ShowMapPanel()
	{
		bool wasCompletedFirstTimeForMap = _gameWonderCompletionService.WasCompletedFirstTimeForMap;
		bool wasCompletedFirstTimeForFaction = _gameWonderCompletionService.WasCompletedFirstTimeForFaction;
		if (wasCompletedFirstTimeForMap | wasCompletedFirstTimeForFaction)
		{
			_mapPanel.ToggleDisplayStyle(visible: true);
			_root.Q<Label>("MapName").text = _mapNameService.Name;
			_thumbnail.style.backgroundImage = GetMapThumbnail();
			_flexibleStartRoot.ToggleDisplayStyle(wasCompletedFirstTimeForMap);
			Sprite asset = _factionService.Current.Logo.Asset;
			_mapMasteryFactionIcon.style.backgroundImage = new StyleBackground(asset);
			_mapMasteryLabel.text = _loc.T(WonderCompletedLocKey, _factionService.Current.DisplayName.Value);
		}
		else
		{
			_mapPanel.ToggleDisplayStyle(visible: false);
		}
	}

	private void ShowStatistics()
	{
		_statisticsContainer.Add(_statisticItemFactory.Create(StatisticIds.DaysPassed));
		_statisticsContainer.Add(_statisticItemFactory.Create(StatisticIds.BeaversBorn));
		_statisticsContainer.Add(_statisticItemFactory.Create(StatisticIds.WaterConsumed));
		_statisticsContainer.Add(_statisticItemFactory.Create(StatisticIds.TailsPainted));
		_statisticsContainer.Add(_statisticItemFactory.Create(StatisticIds.TreesCut));
		_statisticsContainer.Add(_statisticItemFactory.Create(StatisticIds.ChippedTeeth));
		_statisticsContainer.Add(_statisticItemFactory.Create(StatisticIds.BotsManufactured));
		_statisticsContainer.Add(_statisticItemFactory.Create(StatisticIds.DynamiteDetonated));
		_statisticsContainer.Add(_statisticItemFactory.CreateIfHasValue(StatisticIds.BeaversExploded));
	}

	private Texture2D GetMapThumbnail()
	{
		MapFileReference? mapFileReference = (_mapNameService.IsResource ? new MapFileReference?(MapFileReference.FromResource(_mapNameService.Name)) : GetCustom());
		if (!mapFileReference.HasValue)
		{
			return null;
		}
		return _mapThumbnailCache.GetThumbnail(mapFileReference.Value);
	}

	private MapFileReference? GetCustom()
	{
		return _mapItemProvider.GetCustomMaps().LastOrDefault((MapItem mapItem) => mapItem.MapFileReference.Name == _mapNameService.Name)?.MapFileReference;
	}
}
