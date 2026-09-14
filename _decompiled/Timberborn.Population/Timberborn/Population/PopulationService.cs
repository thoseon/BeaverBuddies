using Timberborn.BeaverContaminationSystem;
using Timberborn.Beavers;
using Timberborn.Bots;
using Timberborn.Common;
using Timberborn.DwellingSystem;
using Timberborn.GameDistricts;
using Timberborn.Navigation;
using Timberborn.PopulationWorkStatistics;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;

namespace Timberborn.Population;

public class PopulationService : ITickableSingleton, ILoadableSingleton, IPostLoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly BeaverPopulation _beaverPopulation;

	private readonly BotPopulation _botPopulation;

	private readonly GlobalDwellingStatisticsProvider _globalDwellingStatisticsProvider;

	private readonly GlobalEmploymentStatisticsProvider _globalEmploymentStatisticsProvider;

	private readonly GlobalWorkRefusingStatisticsProvider _globalWorkRefusingStatisticsProvider;

	private readonly GlobalBeaverContaminationStatisticsProvider _globalBeaverContaminationStatisticsProvider;

	private readonly PopulationDataCollector _populationDataCollector;

	private DistrictCenter _districtCenter;

	public PopulationData GlobalPopulationData { get; } = new PopulationData();

	public PopulationData DistrictPopulationData { get; } = new PopulationData();

	public bool BotCreated => _botPopulation.BotCreated;

	public bool IsAnyoneContaminated
	{
		get
		{
			if (GlobalPopulationData.ContaminationData.ContaminatedAdults <= 0)
			{
				return GlobalPopulationData.ContaminationData.ContaminatedChildren > 0;
			}
			return true;
		}
	}

	public bool OnlyBotsAlive
	{
		get
		{
			if (_beaverPopulation.NumberOfBeavers == 0)
			{
				return _botPopulation.NumberOfBots > 0;
			}
			return false;
		}
	}

	public bool AllDead
	{
		get
		{
			if (_beaverPopulation.NumberOfBeavers == 0)
			{
				return _botPopulation.NumberOfBots == 0;
			}
			return false;
		}
	}

	public PopulationService(EventBus eventBus, BeaverPopulation beaverPopulation, BotPopulation botPopulation, GlobalDwellingStatisticsProvider globalDwellingStatisticsProvider, GlobalEmploymentStatisticsProvider globalEmploymentStatisticsProvider, GlobalWorkRefusingStatisticsProvider globalWorkRefusingStatisticsProvider, GlobalBeaverContaminationStatisticsProvider globalBeaverContaminationStatisticsProvider, PopulationDataCollector populationDataCollector, [Ordering] INavigationPhase navigationPhase)
	{
		_eventBus = eventBus;
		_beaverPopulation = beaverPopulation;
		_botPopulation = botPopulation;
		_globalDwellingStatisticsProvider = globalDwellingStatisticsProvider;
		_globalEmploymentStatisticsProvider = globalEmploymentStatisticsProvider;
		_globalWorkRefusingStatisticsProvider = globalWorkRefusingStatisticsProvider;
		_globalBeaverContaminationStatisticsProvider = globalBeaverContaminationStatisticsProvider;
		_populationDataCollector = populationDataCollector;
	}

	public void Tick()
	{
		UpdateData(forceEvent: false);
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void PostLoad()
	{
		UpdateData(forceEvent: true);
	}

	public void SwitchDistrict(DistrictCenter districtCenter)
	{
		_districtCenter = districtCenter;
		UpdateData(forceEvent: true);
	}

	[OnEvent]
	public void OnMigration(MigrationEvent migrationEvent)
	{
		UpdateData(forceEvent: false);
	}

	[OnEvent]
	public void OnNewGameInitialized(NewGameInitializedEvent newGameInitializedEvent)
	{
		UpdateData(forceEvent: false);
	}

	private void UpdateData(bool forceEvent)
	{
		bool num = UpdateGlobalData();
		bool flag = UpdateDistrictData();
		if (num | flag | forceEvent)
		{
			_eventBus.Post(new PopulationChangedEvent());
		}
	}

	private bool UpdateGlobalData()
	{
		return _populationDataCollector.CollectData(_beaverPopulation.NumberOfAdults, _beaverPopulation.NumberOfChildren, _botPopulation.NumberOfBots, _globalWorkRefusingStatisticsProvider, _globalDwellingStatisticsProvider, _globalEmploymentStatisticsProvider, _globalBeaverContaminationStatisticsProvider, GlobalPopulationData);
	}

	private bool UpdateDistrictData()
	{
		if ((bool)_districtCenter)
		{
			return _populationDataCollector.CollectData(_districtCenter, DistrictPopulationData);
		}
		return false;
	}
}
