using System;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.GameFactionSystem;
using Timberborn.InputSystemUI;
using Timberborn.Localization;
using Timberborn.Population;
using Timberborn.ScienceSystem;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class BasicStatisticsPanelFactory : ILoadableSingleton
{
	private static readonly string WellbeingLocKey = "Wellbeing.DisplayName";

	private static readonly string BatchControlLocKey = "BatchControl.ShowInfo";

	private static readonly string BeaversPerishedLocKey = "Bot.BeaversPerished";

	private static readonly string AllPerishedLocKey = "Population.AllPerished";

	private static readonly string ScienceLocKey = "TopBar.Science";

	private static readonly string OpenWellbeingBoxKey = "OpenWellbeingBox";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PopulationWellbeingBox _populationWellbeingBox;

	private readonly FactionService _factionService;

	private readonly ScienceService _scienceService;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly IBatchControlBox _batchControlBox;

	private readonly PopulationService _populationService;

	private readonly ILoc _loc;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly EventBus _eventBus;

	private BindableButton _wellbeingButton;

	public BasicStatisticsPanelFactory(VisualElementLoader visualElementLoader, PopulationWellbeingBox populationWellbeingBox, FactionService factionService, ScienceService scienceService, ITooltipRegistrar tooltipRegistrar, IBatchControlBox batchControlBox, PopulationService populationService, ILoc loc, BindableButtonFactory bindableButtonFactory, EventBus eventBus)
	{
		_visualElementLoader = visualElementLoader;
		_populationWellbeingBox = populationWellbeingBox;
		_factionService = factionService;
		_scienceService = scienceService;
		_tooltipRegistrar = tooltipRegistrar;
		_batchControlBox = batchControlBox;
		_populationService = populationService;
		_loc = loc;
		_bindableButtonFactory = bindableButtonFactory;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public VisualElement Create()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/Population/BasicStatisticsPanel");
		_wellbeingButton = _bindableButtonFactory.Create(visualElement.Q<Button>("Wellbeing"), OpenWellbeingBoxKey, _populationWellbeingBox.Show);
		_tooltipRegistrar.RegisterLocalizable(visualElement.Q<Button>("Wellbeing"), GetWellbeingTooltip);
		Button button = visualElement.Q<Button>("BatchControl");
		button.RegisterCallback<ClickEvent>(delegate
		{
			_batchControlBox.OpenBatchControlBox();
		});
		_tooltipRegistrar.RegisterLocalizable(button, BatchControlLocKey);
		_tooltipRegistrar.Register(visualElement.Q<VisualElement>("ScienceCountHeader"), (Func<string>)GetScienceTooltip);
		visualElement.Q<Image>("FactionIcon").sprite = _factionService.Current.Logo.Asset;
		return visualElement;
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_wellbeingButton.Bind();
	}

	private string GetWellbeingTooltip()
	{
		if (_populationService.OnlyBotsAlive)
		{
			return BeaversPerishedLocKey;
		}
		if (!_populationService.AllDead)
		{
			return WellbeingLocKey;
		}
		return AllPerishedLocKey;
	}

	private string GetScienceTooltip()
	{
		return $"{_loc.T(ScienceLocKey)}: {_scienceService.SciencePoints}";
	}
}
