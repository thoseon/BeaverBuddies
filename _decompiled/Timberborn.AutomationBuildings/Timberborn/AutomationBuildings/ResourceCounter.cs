using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.Persistence;
using Timberborn.ResourceCountingSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class ResourceCounter : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<ResourceCounter>, IDuplicable, ISamplingTransmitter, ITransmitter
{
	private static readonly ComponentKey ComponentKey = new ComponentKey("ResourceCounter");

	private static readonly PropertyKey<int> ThresholdKey = new PropertyKey<int>("Threshold");

	private static readonly PropertyKey<float> FillRateThresholdKey = new PropertyKey<float>("FillRateThreshold");

	private static readonly PropertyKey<string> GoodIdKey = new PropertyKey<string>("GoodId");

	private static readonly PropertyKey<ResourceCounterMode> ModeKey = new PropertyKey<ResourceCounterMode>("Mode");

	private static readonly PropertyKey<NumericComparisonMode> ComparisonModeKey = new PropertyKey<NumericComparisonMode>("ComparisonMode");

	private static readonly PropertyKey<bool> IncludeInputsKey = new PropertyKey<bool>("IncludeInputs");

	private readonly IGoodService _goodService;

	private readonly SamplingResourcesService _samplingResourcesService;

	private Automator _automator;

	private DistrictBuilding _districtBuilding;

	public int SampledResourceCount { get; private set; }

	public float SampledFillRate { get; private set; }

	public int Threshold { get; private set; }

	public string GoodId { get; private set; }

	public float FillRateThreshold { get; private set; }

	public NumericComparisonMode ComparisonMode { get; private set; }

	public ResourceCounterMode Mode { get; private set; }

	public bool IncludeInputs { get; private set; }

	public event EventHandler<string> GoodChanged;

	internal ResourceCounter(IGoodService goodService, SamplingResourcesService samplingResourcesService)
	{
		_goodService = goodService;
		_samplingResourcesService = samplingResourcesService;
	}

	public void Awake()
	{
		GoodId = _goodService.Goods[0];
		_automator = GetComponent<Automator>();
		_districtBuilding = GetComponent<DistrictBuilding>();
		_districtBuilding.ReassignedInstantDistrict += OnReassignedDistrict;
		_districtBuilding.ReassignedConstructionDistrict += OnReassignedDistrict;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ComponentKey);
		component.Set(ThresholdKey, Threshold);
		component.Set(FillRateThresholdKey, FillRateThreshold);
		component.Set(GoodIdKey, GoodId);
		component.Set(ModeKey, Mode);
		component.Set(ComparisonModeKey, ComparisonMode);
		component.Set(IncludeInputsKey, IncludeInputs);
	}

	[BackwardCompatible(2026, 2, 24, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ComponentKey);
		Threshold = component.Get(ThresholdKey);
		FillRateThreshold = component.Get(FillRateThresholdKey);
		GoodId = component.Get(GoodIdKey);
		Mode = component.Get(ModeKey);
		ComparisonMode = component.Get(ComparisonModeKey);
		if (component.Has(IncludeInputsKey))
		{
			IncludeInputs = component.Get(IncludeInputsKey);
		}
	}

	public void DuplicateFrom(ResourceCounter source)
	{
		Mode = source.Mode;
		GoodId = source.GoodId;
		InvokeGoodChangeEvent(source.GoodId);
		Threshold = source.Threshold;
		FillRateThreshold = source.FillRateThreshold;
		ComparisonMode = source.ComparisonMode;
		IncludeInputs = source.IncludeInputs;
		Sample();
	}

	public void SetGoodId(string goodId)
	{
		GoodId = goodId;
		InvokeGoodChangeEvent(goodId);
		Sample();
	}

	public void SetThreshold(int threshold)
	{
		Threshold = threshold;
		UpdateOutputState();
	}

	public void SetFillRateThreshold(float fillRateThreshold)
	{
		FillRateThreshold = fillRateThreshold;
		UpdateOutputState();
	}

	public void SetMode(ResourceCounterMode mode)
	{
		Mode = mode;
		Sample();
	}

	public void SetComparisonMode(NumericComparisonMode comparisionMode)
	{
		ComparisonMode = comparisionMode;
		UpdateOutputState();
	}

	public void SetIncludeInputs(bool includeInputs)
	{
		IncludeInputs = includeInputs;
		Sample();
	}

	public void Sample()
	{
		DistrictCenter instantOrConstructionDistrict = _districtBuilding.GetInstantOrConstructionDistrict();
		ResourceCount sampledResourceCount = _samplingResourcesService.GetSampledResourceCount(instantOrConstructionDistrict, GoodId);
		switch (Mode)
		{
		case ResourceCounterMode.FillRate:
			SampledFillRate = sampledResourceCount.FillRate;
			break;
		case ResourceCounterMode.StockLevel:
			SampledResourceCount = (IncludeInputs ? sampledResourceCount.AllStock : sampledResourceCount.AvailableStock);
			break;
		}
		UpdateOutputState();
	}

	private void OnReassignedDistrict(object sender, EventArgs e)
	{
		Sample();
	}

	private void UpdateOutputState()
	{
		if (_districtBuilding.GetInstantOrConstructionDistrict() == null)
		{
			_automator.SetState(state: false);
			return;
		}
		Automator automator = _automator;
		automator.SetState(Mode switch
		{
			ResourceCounterMode.StockLevel => ComparisonMode.Evaluate(SampledResourceCount, Threshold), 
			ResourceCounterMode.FillRate => ComparisonMode.Evaluate(SampledFillRate, FillRateThreshold), 
			_ => throw new ArgumentOutOfRangeException(), 
		});
	}

	private void InvokeGoodChangeEvent(string goodId)
	{
		GoodChanged?.Invoke(this, goodId);
	}
}
