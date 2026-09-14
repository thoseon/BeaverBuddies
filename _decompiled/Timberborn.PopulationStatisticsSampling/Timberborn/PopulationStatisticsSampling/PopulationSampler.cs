using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.GameCycleSystem;
using Timberborn.GameDistricts;
using Timberborn.Population;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.Wellbeing;

namespace Timberborn.PopulationStatisticsSampling;

public class PopulationSampler : ILoadableSingleton, ITickableSingleton
{
	private readonly GlobalPopulationSamplesRegistry _globalPopulationSamplesRegistry;

	private readonly PopulationDataCollector _populationDataCollector;

	private readonly EventBus _eventBus;

	private readonly WellbeingService _wellbeingService;

	private readonly GameCycleService _gameCycleService;

	private readonly List<DistrictPopulationSamplesRegistry> _districtRegistries = new List<DistrictPopulationSamplesRegistry>();

	private PopulationSample _globalSample;

	private int _performInitialSample;

	public PopulationSampler(GlobalPopulationSamplesRegistry globalPopulationSamplesRegistry, PopulationDataCollector populationDataCollector, EventBus eventBus, WellbeingService wellbeingService, GameCycleService gameCycleService)
	{
		_globalPopulationSamplesRegistry = globalPopulationSamplesRegistry;
		_populationDataCollector = populationDataCollector;
		_eventBus = eventBus;
		_wellbeingService = wellbeingService;
		_gameCycleService = gameCycleService;
	}

	public void AddDistrictRegistry(DistrictPopulationSamplesRegistry districtPopulationSamplesRegistry)
	{
		_districtRegistries.Add(districtPopulationSamplesRegistry);
	}

	public void RemoveDistrictRegistry(DistrictPopulationSamplesRegistry districtPopulationSamplesRegistry)
	{
		_districtRegistries.Remove(districtPopulationSamplesRegistry);
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void Tick()
	{
		if (--_performInitialSample == 0)
		{
			CollectPopulationSamples();
		}
	}

	[OnEvent]
	public void OnDaytimeStart(DaytimeStartEvent daytimeStartEvent)
	{
		CollectPopulationSamples();
	}

	[OnEvent]
	public void OnNewGameInitializedEvent(NewGameInitializedEvent newGameInitializedEvent)
	{
		_performInitialSample = 2;
	}

	private void CollectPopulationSamples()
	{
		_globalSample = default(PopulationSample);
		CollectDistrictsSamples();
		_globalPopulationSamplesRegistry.PopulationSampleHistory.AddSample(_globalSample);
		_eventBus.Post(new PopulationSampledEvent());
	}

	private void CollectDistrictsSamples()
	{
		foreach (DistrictPopulationSamplesRegistry districtRegistry in _districtRegistries)
		{
			DistrictCenter districtCenter = districtRegistry.DistrictCenter;
			PopulationData populationData = new PopulationData();
			_populationDataCollector.CollectData(districtCenter, populationData);
			DistrictPopulationBalance component = districtCenter.GetComponent<DistrictPopulationBalance>();
			PopulationSample populationSample = new PopulationSample(_gameCycleService.CycleDay, _gameCycleService.Cycle, populationData.NumberOfAdults, populationData.NumberOfChildren, populationData.ContaminationData.ContaminatedTotal, populationData.NumberOfBots, _wellbeingService.GetAverageDistrictWellbeing(districtCenter), component.Births, component.Deaths, component.BotCreations, component.BotDestructions, populationData.BedData.OccupiedBeds, populationData.BedData.FreeBeds, populationData.BedData.Homeless, populationData.BeaverWorkplaceData.OccupiedWorkslots, populationData.BeaverWorkplaceData.FreeWorkslots, populationData.BeaverWorkplaceData.Unemployed, populationData.BeaverWorkforceData.Unemployable, populationData.BotWorkplaceData.OccupiedWorkslots, populationData.BotWorkplaceData.FreeWorkslots, populationData.BotWorkplaceData.Unemployed, populationData.BotWorkforceData.Unemployable);
			districtRegistry.PopulationSampleHistory.AddSample(populationSample);
			_globalSample += populationSample;
			component.Clear();
		}
		_globalSample.SetWellbeing(_wellbeingService.AverageGlobalWellbeing);
	}
}
