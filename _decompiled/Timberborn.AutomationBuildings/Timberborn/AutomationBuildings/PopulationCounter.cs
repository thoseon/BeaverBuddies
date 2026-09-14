using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BuildingsReachability;
using Timberborn.DuplicationSystem;
using Timberborn.GameDistricts;
using Timberborn.Persistence;
using Timberborn.Population;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class PopulationCounter : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<PopulationCounter>, IDuplicable, ISamplingTransmitter, ITransmitter, IUnconnectedBuildingBlocker
{
	private static readonly ComponentKey ComponentKey = new ComponentKey("PopulationCounter");

	private static readonly PropertyKey<PopulationCounterMode> ModeKey = new PropertyKey<PopulationCounterMode>("Mode");

	private static readonly PropertyKey<NumericComparisonMode> ComparisonModeKey = new PropertyKey<NumericComparisonMode>("ComparisonMode");

	private static readonly PropertyKey<bool> GlobalModeKey = new PropertyKey<bool>("GlobalMode");

	private static readonly PropertyKey<bool> CountBeaversKey = new PropertyKey<bool>("CountBeavers");

	private static readonly PropertyKey<bool> CountBotsKey = new PropertyKey<bool>("CountBots");

	private static readonly PropertyKey<int> ThresholdKey = new PropertyKey<int>("Threshold");

	private readonly SamplingPopulationService _samplingPopulationService;

	private Automator _automator;

	private DistrictBuilding _districtBuilding;

	private PopulationData _sampledPopulationData;

	private readonly PopulationData _emptyPopulationData = new PopulationData();

	public PopulationCounterMode Mode { get; private set; }

	public NumericComparisonMode ComparisonMode { get; private set; }

	public bool GlobalMode { get; private set; }

	public bool CountBeavers { get; private set; } = true;

	public bool CountBots { get; private set; } = true;

	public int Threshold { get; private set; }

	public bool UsesWorkerType => Mode switch
	{
		PopulationCounterMode.TotalPopulation => false, 
		PopulationCounterMode.TotalBeavers => false, 
		PopulationCounterMode.Adults => false, 
		PopulationCounterMode.Children => false, 
		PopulationCounterMode.Bots => false, 
		PopulationCounterMode.OccupiedBeds => false, 
		PopulationCounterMode.FreeBeds => false, 
		PopulationCounterMode.Homeless => false, 
		PopulationCounterMode.Jobs => true, 
		PopulationCounterMode.Employed => true, 
		PopulationCounterMode.Unemployed => true, 
		PopulationCounterMode.Vacancies => true, 
		PopulationCounterMode.TotalWorkers => true, 
		PopulationCounterMode.HealthyWorkers => true, 
		PopulationCounterMode.UnhealthyWorkers => true, 
		PopulationCounterMode.ContaminatedTotal => false, 
		PopulationCounterMode.ContaminatedAdults => false, 
		PopulationCounterMode.ContaminatedChildren => false, 
		_ => throw new ArgumentOutOfRangeException(), 
	};

	public bool IsUnconnectedBlocked => GlobalMode;

	public event EventHandler IsUnconnectedBlockedChanged;

	internal PopulationCounter(SamplingPopulationService samplingPopulationService)
	{
		_samplingPopulationService = samplingPopulationService;
	}

	public void Awake()
	{
		_automator = GetComponent<Automator>();
		_districtBuilding = GetComponent<DistrictBuilding>();
		_districtBuilding.ReassignedInstantDistrict += OnReassignedDistrict;
		_districtBuilding.ReassignedConstructionDistrict += OnReassignedDistrict;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ComponentKey);
		component.Set(ModeKey, Mode);
		component.Set(ComparisonModeKey, ComparisonMode);
		component.Set(GlobalModeKey, GlobalMode);
		component.Set(CountBeaversKey, CountBeavers);
		component.Set(CountBotsKey, CountBots);
		component.Set(ThresholdKey, Threshold);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ComponentKey);
		Mode = component.Get(ModeKey);
		ComparisonMode = component.Get(ComparisonModeKey);
		GlobalMode = component.Has(GlobalModeKey) && component.Get(GlobalModeKey);
		CountBeavers = component.Has(CountBeaversKey) && component.Get(CountBeaversKey);
		CountBots = component.Has(CountBotsKey) && component.Get(CountBotsKey);
		Threshold = component.Get(ThresholdKey);
	}

	public void DuplicateFrom(PopulationCounter source)
	{
		Mode = source.Mode;
		ComparisonMode = source.ComparisonMode;
		SetGlobalModeInternal(source.GlobalMode, sample: false);
		CountBeavers = source.CountBeavers;
		CountBots = source.CountBots;
		Threshold = source.Threshold;
		Sample();
	}

	public void SetThreshold(int threshold)
	{
		if (Threshold != threshold)
		{
			Threshold = threshold;
			Sample();
		}
	}

	public void SetMode(PopulationCounterMode mode)
	{
		if (Mode != mode)
		{
			Mode = mode;
			Sample();
		}
	}

	public void SetGlobalMode(bool value)
	{
		SetGlobalModeInternal(value, sample: true);
	}

	public void SetCountBeavers(bool value)
	{
		if (CountBeavers != value)
		{
			CountBeavers = value;
			Sample();
		}
	}

	public void SetCountBots(bool value)
	{
		if (CountBots != value)
		{
			CountBots = value;
			Sample();
		}
	}

	public void SetComparisonMode(NumericComparisonMode comparisionMode)
	{
		ComparisonMode = comparisionMode;
		Sample();
	}

	public void Sample()
	{
		if (GlobalMode)
		{
			_sampledPopulationData = _samplingPopulationService.GlobalPopulationData;
		}
		else
		{
			DistrictCenter instantOrConstructionDistrict = _districtBuilding.GetInstantOrConstructionDistrict();
			_sampledPopulationData = (instantOrConstructionDistrict ? _samplingPopulationService.GetDistrictData(instantOrConstructionDistrict) : _emptyPopulationData);
		}
		UpdateOutputState();
	}

	public int GetMeasurement()
	{
		return Mode switch
		{
			PopulationCounterMode.TotalPopulation => _sampledPopulationData.TotalPopulation, 
			PopulationCounterMode.TotalBeavers => _sampledPopulationData.NumberOfBeavers, 
			PopulationCounterMode.Adults => _sampledPopulationData.NumberOfAdults, 
			PopulationCounterMode.Children => _sampledPopulationData.NumberOfChildren, 
			PopulationCounterMode.Bots => _sampledPopulationData.NumberOfBots, 
			PopulationCounterMode.OccupiedBeds => _sampledPopulationData.BedData.OccupiedBeds, 
			PopulationCounterMode.FreeBeds => _sampledPopulationData.BedData.FreeBeds, 
			PopulationCounterMode.Homeless => _sampledPopulationData.BedData.Homeless, 
			PopulationCounterMode.Jobs => (CountBeavers ? _sampledPopulationData.BeaverWorkplaceData.TotalWorkslots : 0) + (CountBots ? _sampledPopulationData.BotWorkplaceData.TotalWorkslots : 0), 
			PopulationCounterMode.Employed => (CountBeavers ? _sampledPopulationData.BeaverWorkplaceData.OccupiedWorkslots : 0) + (CountBots ? _sampledPopulationData.BotWorkplaceData.OccupiedWorkslots : 0), 
			PopulationCounterMode.Unemployed => (CountBeavers ? _sampledPopulationData.BeaverWorkplaceData.Unemployed : 0) + (CountBots ? _sampledPopulationData.BotWorkplaceData.Unemployed : 0), 
			PopulationCounterMode.Vacancies => (CountBeavers ? _sampledPopulationData.BeaverWorkplaceData.FreeWorkslots : 0) + (CountBots ? _sampledPopulationData.BotWorkplaceData.FreeWorkslots : 0), 
			PopulationCounterMode.TotalWorkers => (CountBeavers ? _sampledPopulationData.BeaverWorkforceData.Total : 0) + (CountBots ? _sampledPopulationData.BotWorkforceData.Total : 0), 
			PopulationCounterMode.HealthyWorkers => (CountBeavers ? _sampledPopulationData.BeaverWorkforceData.Employable : 0) + (CountBots ? _sampledPopulationData.BotWorkforceData.Employable : 0), 
			PopulationCounterMode.UnhealthyWorkers => (CountBeavers ? _sampledPopulationData.BeaverWorkforceData.Unemployable : 0) + (CountBots ? _sampledPopulationData.BotWorkforceData.Unemployable : 0), 
			PopulationCounterMode.ContaminatedTotal => _sampledPopulationData.ContaminationData.ContaminatedTotal, 
			PopulationCounterMode.ContaminatedAdults => _sampledPopulationData.ContaminationData.ContaminatedAdults, 
			PopulationCounterMode.ContaminatedChildren => _sampledPopulationData.ContaminationData.ContaminatedChildren, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private void OnReassignedDistrict(object sender, EventArgs e)
	{
		Sample();
	}

	private void UpdateOutputState()
	{
		_automator.SetState(ComparisonMode.Evaluate(GetMeasurement(), Threshold));
	}

	private void SetGlobalModeInternal(bool value, bool sample)
	{
		if (GlobalMode != value)
		{
			GlobalMode = value;
			IsUnconnectedBlockedChanged?.Invoke(this, EventArgs.Empty);
			if (sample)
			{
				Sample();
			}
		}
	}
}
