using System;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.Population;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.PopulationUI;

internal class PopulationDataRow : IPopulationRow
{
	private readonly ILoc _loc;

	private readonly PopulationService _populationService;

	private readonly Label _adultCount;

	private readonly Label _childCount;

	private readonly Label _botCount;

	private readonly Label _contaminatedCount;

	private readonly VisualElement _botIcon;

	private readonly VisualElement _contaminatedIcon;

	private readonly Func<PopulationData> _populationDataGetter;

	private readonly Phrase _adultPhrase = Phrase.New().FormatCompact();

	private readonly Phrase _childPhrase = Phrase.New().FormatCompact();

	private readonly Phrase _botPhrase = Phrase.New().FormatCompact();

	private readonly Phrase _contaminatedPhrase = Phrase.New().FormatCompact();

	public PopulationDataRow(ILoc loc, PopulationService populationService, Label adultCount, Label childCount, Label botCount, Label contaminatedCount, VisualElement botIcon, VisualElement contaminatedIcon, Func<PopulationData> populationDataGetter)
	{
		_loc = loc;
		_populationService = populationService;
		_adultCount = adultCount;
		_childCount = childCount;
		_botCount = botCount;
		_contaminatedCount = contaminatedCount;
		_botIcon = botIcon;
		_contaminatedIcon = contaminatedIcon;
		_populationDataGetter = populationDataGetter;
	}

	public void UpdateData()
	{
		PopulationData populationData = _populationDataGetter();
		UpdateBeaverCount(populationData);
		UpdateBotCount(populationData);
		UpdateContaminatedCount(populationData);
	}

	private void UpdateBeaverCount(PopulationData populationData)
	{
		_adultCount.text = _loc.T(_adultPhrase, populationData.NumberOfHealthyAdults);
		_childCount.text = _loc.T(_childPhrase, populationData.NumberOfHealthyChildren);
	}

	private void UpdateBotCount(PopulationData populationData)
	{
		bool botCreated = _populationService.BotCreated;
		_botIcon.ToggleDisplayStyle(botCreated);
		_botCount.ToggleDisplayStyle(botCreated);
		_botCount.text = _loc.T(_botPhrase, populationData.NumberOfBots);
	}

	private void UpdateContaminatedCount(PopulationData populationData)
	{
		int num = populationData.ContaminationData.ContaminatedAdults + populationData.ContaminationData.ContaminatedChildren;
		bool visible = num > 0;
		_contaminatedIcon.ToggleDisplayStyle(visible);
		_contaminatedCount.ToggleDisplayStyle(visible);
		_contaminatedCount.text = _loc.T(_contaminatedPhrase, num);
	}
}
