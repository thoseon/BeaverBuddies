using System;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.Population;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.PopulationUI;

internal class PopulationDataRowFactory
{
	private static readonly string AdultsLocKey = "Beaver.Population.Adults";

	private static readonly string ChildrenLocKey = "Beaver.Population.Children";

	private static readonly string BotsLocKey = "Bot.PluralDisplayName";

	private static readonly string ContaminatedAdultsLocKey = "Beaver.Population.ContaminatedAdults";

	private static readonly string ContaminatedChildrenLocKey = "Beaver.Population.ContaminatedChildren";

	private readonly ILoc _loc;

	private readonly PopulationService _populationService;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	public PopulationDataRowFactory(ILoc loc, PopulationService populationService, ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader)
	{
		_loc = loc;
		_populationService = populationService;
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
	}

	public PopulationDataRow Create(VisualElement root, Func<PopulationData> populationDataGetter)
	{
		string elementName = "Game/Population/PopulationDataRow";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		root.Add(visualElement);
		_tooltipRegistrar.Register(visualElement, () => GetPopulationTooltip(populationDataGetter));
		Label adultCount = visualElement.Q<Label>("AdultCount");
		Label childCount = visualElement.Q<Label>("ChildCount");
		Label botCount = visualElement.Q<Label>("BotCount");
		Label contaminatedCount = visualElement.Q<Label>("ContaminatedCount");
		VisualElement botIcon = visualElement.Q<VisualElement>("BotIcon");
		VisualElement contaminatedIcon = visualElement.Q<VisualElement>("ContaminatedIcon");
		return new PopulationDataRow(_loc, _populationService, adultCount, childCount, botCount, contaminatedCount, botIcon, contaminatedIcon, populationDataGetter);
	}

	private string GetPopulationTooltip(Func<PopulationData> populationDataGetter)
	{
		PopulationData populationData = populationDataGetter();
		ContaminationData contaminationData = populationData.ContaminationData;
		string text = (_populationService.BotCreated ? $"\n{_loc.T(BotsLocKey)}: {populationData.NumberOfBots}" : "");
		string text2 = ((contaminationData.ContaminatedAdults > 0) ? $"\n{_loc.T(ContaminatedAdultsLocKey)}: {contaminationData.ContaminatedAdults}" : "");
		string text3 = ((contaminationData.ContaminatedChildren > 0) ? $"\n{_loc.T(ContaminatedChildrenLocKey)}: {contaminationData.ContaminatedChildren}" : "");
		return $"{_loc.T(AdultsLocKey)}: {populationData.NumberOfHealthyAdults}" + $"\n{_loc.T(ChildrenLocKey)}: {populationData.NumberOfHealthyChildren}" + text + text2 + text3;
	}
}
