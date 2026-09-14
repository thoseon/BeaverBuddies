using System.Collections.Generic;
using Timberborn.BatchControl;
using Timberborn.Bots;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.PopulationStatisticsSampling;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.PopulationStatisticsBatchControl;

internal class PopulationStatisticsGraphFactory
{
	private static readonly Color Red = new Color(0.9f, 0f, 0f);

	private static readonly Color Green = new Color(0f, 0.9f, 0f);

	private static readonly Color LightBlue = new Color(0.16f, 0.59f, 0.8f);

	private static readonly Color Amber = new Color(1f, 0.8f, 0f);

	private static readonly Color Peach = new Color(1f, 0.9f, 0.7f);

	private static readonly Color RedOrange = new Color(1f, 0.4f, 0.1f);

	private readonly VisualElementLoader _visualElementLoader;

	private readonly EventBus _eventBus;

	private readonly BatchControlBoxTimeRangeController _batchControlBoxTimeRangeController;

	private readonly BotPopulation _botPopulation;

	private readonly PopulationGraphState _populationGraphState;

	private readonly ILoc _loc;

	private readonly PopulationStatisticsTooltipRegistrar _populationStatisticsTooltipRegistrar;

	public PopulationStatisticsGraphFactory(VisualElementLoader visualElementLoader, EventBus eventBus, BatchControlBoxTimeRangeController batchControlBoxTimeRangeController, BotPopulation botPopulation, PopulationGraphState populationGraphState, ILoc loc, PopulationStatisticsTooltipRegistrar populationStatisticsTooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_eventBus = eventBus;
		_batchControlBoxTimeRangeController = batchControlBoxTimeRangeController;
		_botPopulation = botPopulation;
		_populationGraphState = populationGraphState;
		_loc = loc;
		_populationStatisticsTooltipRegistrar = populationStatisticsTooltipRegistrar;
	}

	public IEnumerable<IBatchControlRowItem> Create(PopulationSampleHistory populationSampleHistory)
	{
		yield return Create(populationSampleHistory, PopulationGraph(), "BatchControl.Population");
		yield return Create(populationSampleHistory, WellbeingGraph(), "Wellbeing.DisplayName");
		yield return Create(populationSampleHistory, PopulationTurnoverGraph(), "BatchControl.PopulationStatistics.PopulationTurnover");
		yield return Create(populationSampleHistory, HomesGraph(), "BatchControl.Housing");
		yield return Create(populationSampleHistory, BeaverEmploymentGraph(), "BatchControl.PopulationStatistics.BeaverEmployment");
		if (_botPopulation.BotCreated)
		{
			yield return Create(populationSampleHistory, BotEmploymentGraph(), "BatchControl.PopulationStatistics.BotEmployment");
		}
	}

	private PopulationStatisticsGraph Create(PopulationSampleHistory populationSampleHistory, List<PopulationGraphDefinition> definitions, string titleLocKey)
	{
		string elementName = "Game/BatchControl/PopulationStatisticsGraph";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		visualElement.Q<Label>("Title").text = _loc.T(titleLocKey);
		return new PopulationStatisticsGraph(_eventBus, _batchControlBoxTimeRangeController, _visualElementLoader, _populationGraphState, _populationStatisticsTooltipRegistrar, visualElement, populationSampleHistory, definitions);
	}

	private List<PopulationGraphDefinition> PopulationGraph()
	{
		List<PopulationGraphDefinition> list = new List<PopulationGraphDefinition>
		{
			new PopulationGraphDefinition("TotalPopulation", _loc.T("Building.PopulationCounter.Mode.TotalPopulation"), Green),
			new PopulationGraphDefinition("Adults", _loc.T("Building.PopulationCounter.Mode.Adults"), Amber),
			new PopulationGraphDefinition("Children", _loc.T("Building.PopulationCounter.Mode.Children"), Peach),
			new PopulationGraphDefinition("Contaminated", _loc.T("Beaver.Population.Contaminated"), Red, isVisible: false)
		};
		if (_botPopulation.BotCreated)
		{
			list.Add(new PopulationGraphDefinition("Bots", _loc.T("Building.PopulationCounter.Mode.Bots"), LightBlue));
		}
		return list;
	}

	private List<PopulationGraphDefinition> WellbeingGraph()
	{
		return new List<PopulationGraphDefinition>
		{
			new PopulationGraphDefinition("Wellbeing", _loc.T("BatchControl.PopulationStatistics.PopulationAverage"), Green)
		};
	}

	private List<PopulationGraphDefinition> PopulationTurnoverGraph()
	{
		List<PopulationGraphDefinition> list = new List<PopulationGraphDefinition>
		{
			new PopulationGraphDefinition("Births", _loc.T("BatchControl.PopulationStatistics.Births"), Green),
			new PopulationGraphDefinition("Deaths", _loc.T("BatchControl.PopulationStatistics.Deaths"), Red)
		};
		if (_botPopulation.BotCreated)
		{
			list.Add(new PopulationGraphDefinition("BotCreations", _loc.T("BatchControl.PopulationStatistics.BotCreations"), LightBlue));
			list.Add(new PopulationGraphDefinition("BotDestructions", _loc.T("BatchControl.PopulationStatistics.BotDestructions"), RedOrange));
		}
		return list;
	}

	private List<PopulationGraphDefinition> HomesGraph()
	{
		return new List<PopulationGraphDefinition>
		{
			new PopulationGraphDefinition("TotalBeds", _loc.T("Building.PopulationCounter.Mode.TotalBeds"), Green),
			new PopulationGraphDefinition("OccupiedBeds", _loc.T("Building.PopulationCounter.Mode.OccupiedBeds"), Amber),
			new PopulationGraphDefinition("FreeBeds", _loc.T("Building.PopulationCounter.Mode.FreeBeds"), Peach, isVisible: false),
			new PopulationGraphDefinition("Homeless", _loc.T("Building.PopulationCounter.Mode.Homeless"), Red)
		};
	}

	private List<PopulationGraphDefinition> BeaverEmploymentGraph()
	{
		return new List<PopulationGraphDefinition>
		{
			new PopulationGraphDefinition("EmployedBeavers", _loc.T("Building.PopulationCounter.Mode.Employed"), Green),
			new PopulationGraphDefinition("FreeWorkSlotsBeavers", _loc.T("Building.PopulationCounter.Mode.Vacancies"), Amber),
			new PopulationGraphDefinition("UnemployedBeavers", _loc.T("Building.PopulationCounter.Mode.Unemployed"), RedOrange),
			new PopulationGraphDefinition("UnemployableBeavers", _loc.T("Work.Incapacitated"), Red, isVisible: false)
		};
	}

	private List<PopulationGraphDefinition> BotEmploymentGraph()
	{
		return new List<PopulationGraphDefinition>
		{
			new PopulationGraphDefinition("EmployedBots", _loc.T("Building.PopulationCounter.Mode.Employed"), Green),
			new PopulationGraphDefinition("FreeWorkSlotsBots", _loc.T("Building.PopulationCounter.Mode.Vacancies"), Amber),
			new PopulationGraphDefinition("UnemployedBots", _loc.T("Building.PopulationCounter.Mode.Unemployed"), RedOrange),
			new PopulationGraphDefinition("UnemployableBots", _loc.T("Work.Incapacitated"), Red, isVisible: false)
		};
	}
}
