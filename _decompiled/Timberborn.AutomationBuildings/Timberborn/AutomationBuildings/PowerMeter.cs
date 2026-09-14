using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.DuplicationSystem;
using Timberborn.MechanicalSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class PowerMeter : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<PowerMeter>, IDuplicable, ISamplingTransmitter, ITransmitter
{
	private static readonly ComponentKey ComponentKey = new ComponentKey("PowerMeter");

	private static readonly PropertyKey<PowerMeterMode> ModeKey = new PropertyKey<PowerMeterMode>("Mode");

	private static readonly PropertyKey<NumericComparisonMode> ComparisonModeKey = new PropertyKey<NumericComparisonMode>("ComparisonMode");

	private static readonly PropertyKey<int> IntThresholdKey = new PropertyKey<int>("IntThreshold");

	private static readonly PropertyKey<float> PercentThresholdKey = new PropertyKey<float>("PercentThreshold");

	private readonly TransputMap _transputMap;

	private Automator _automator;

	private MechanicalNode _mechanicalNode;

	private int _sampledPowerSupply;

	private int _sampledPowerDemand;

	private float _sampledBatteryChargeLevel;

	public PowerMeterMode Mode { get; private set; }

	public NumericComparisonMode ComparisonMode { get; private set; }

	public int IntThreshold { get; private set; }

	public float PercentThreshold { get; private set; } = 0.5f;

	public int IntMeasurement { get; private set; }

	public float PercentMeasurement { get; private set; }

	public bool IsPercentThreshold => Mode == PowerMeterMode.BatteryChargeLevel;

	public PowerMeter(TransputMap transputMap)
	{
		_transputMap = transputMap;
	}

	public void Awake()
	{
		_automator = GetComponent<Automator>();
		_mechanicalNode = GetComponent<MechanicalNode>();
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ComponentKey);
		component.Set(ModeKey, Mode);
		component.Set(ComparisonModeKey, ComparisonMode);
		component.Set(IntThresholdKey, IntThreshold);
		component.Set(PercentThresholdKey, PercentThreshold);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ComponentKey);
		Mode = component.Get(ModeKey);
		ComparisonMode = component.Get(ComparisonModeKey);
		IntThreshold = component.Get(IntThresholdKey);
		PercentThreshold = component.Get(PercentThresholdKey);
	}

	public void DuplicateFrom(PowerMeter source)
	{
		Mode = source.Mode;
		ComparisonMode = source.ComparisonMode;
		IntThreshold = source.IntThreshold;
		PercentThreshold = source.PercentThreshold;
		UpdateState();
	}

	public void SetMode(PowerMeterMode mode)
	{
		if (Mode != mode)
		{
			Mode = mode;
			UpdateState();
		}
	}

	public void SetComparisonMode(NumericComparisonMode comparisionMode)
	{
		if (ComparisonMode != comparisionMode)
		{
			ComparisonMode = comparisionMode;
			UpdateState();
		}
	}

	public void SetIntThreshold(int value)
	{
		if (IntThreshold != value)
		{
			IntThreshold = value;
			UpdateState();
		}
	}

	public void SetPercentThreshold(float value)
	{
		if (!PercentThreshold.Equals(value))
		{
			PercentThreshold = value;
			UpdateState();
		}
	}

	public void Sample()
	{
		MechanicalGraph graph = GetGraph();
		_sampledPowerSupply = graph?.PowerSupply ?? 0;
		_sampledPowerDemand = graph?.PowerDemand ?? 0;
		_sampledBatteryChargeLevel = graph?.BatteryChargeLevel ?? 0f;
		UpdateState();
	}

	private MechanicalGraph GetGraph()
	{
		return _mechanicalNode.Graph ?? GetGraphWhenUnfinished();
	}

	private MechanicalGraph GetGraphWhenUnfinished()
	{
		return _transputMap.GetFacingTransput(_mechanicalNode.Transputs[0])?.ParentNode.Graph;
	}

	private void UpdateState()
	{
		UpdateMeasurement();
		Automator automator = _automator;
		automator.SetState(Mode switch
		{
			PowerMeterMode.Supply => ComparisonMode.Evaluate(IntMeasurement, IntThreshold), 
			PowerMeterMode.Demand => ComparisonMode.Evaluate(IntMeasurement, IntThreshold), 
			PowerMeterMode.Surplus => ComparisonMode.Evaluate(IntMeasurement, IntThreshold), 
			PowerMeterMode.BatteryChargeLevel => ComparisonMode.Evaluate(PercentMeasurement, PercentThreshold), 
			_ => throw new ArgumentOutOfRangeException(Mode.ToString()), 
		});
	}

	private void UpdateMeasurement()
	{
		switch (Mode)
		{
		case PowerMeterMode.Supply:
			IntMeasurement = _sampledPowerSupply;
			break;
		case PowerMeterMode.Demand:
			IntMeasurement = _sampledPowerDemand;
			break;
		case PowerMeterMode.Surplus:
			IntMeasurement = _sampledPowerSupply - _sampledPowerDemand;
			break;
		case PowerMeterMode.BatteryChargeLevel:
			PercentMeasurement = _sampledBatteryChargeLevel;
			break;
		default:
			throw new ArgumentOutOfRangeException(Mode.ToString());
		}
	}
}
