using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WaterSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

public class FlowSensor : BaseComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity, IDuplicable<FlowSensor>, IDuplicable, ISamplingTransmitter, ITransmitter
{
	public static readonly float Precision = 0.01f;

	private static readonly ComponentKey ContaminationSensorKey = new ComponentKey("FlowSensor");

	private static readonly PropertyKey<float> ThresholdKey = new PropertyKey<float>("Threshold");

	private static readonly PropertyKey<NumericComparisonMode> ModeKey = new PropertyKey<NumericComparisonMode>("Mode");

	private static readonly float DefaultThreshold = 0f;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private FlowSensorSpec _flowSensorSpec;

	private Automator _automator;

	private BlockObject _blockObject;

	private Vector3Int _sensorCoordinates;

	public float Threshold { get; private set; }

	public float? SampledFlow { get; private set; }

	public NumericComparisonMode Mode { get; private set; }

	public float MaxThreshold => _flowSensorSpec.MaxThreshold;

	public FlowSensor(IThreadSafeWaterMap threadSafeWaterMap)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void Awake()
	{
		_flowSensorSpec = GetComponent<FlowSensorSpec>();
		_automator = GetComponent<Automator>();
		_blockObject = GetComponent<BlockObject>();
		Threshold = DefaultThreshold;
	}

	public void InitializeEntity()
	{
		_sensorCoordinates = _blockObject.TransformCoordinates(_flowSensorSpec.SensorCoordinates);
	}

	public void SetThreshold(float value)
	{
		if (!Threshold.Equals(value))
		{
			Threshold = value;
			UpdateOutputState();
		}
	}

	public void SetMode(NumericComparisonMode mode)
	{
		if (Mode != mode)
		{
			Mode = mode;
			UpdateOutputState();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ContaminationSensorKey);
		component.Set(ThresholdKey, Threshold);
		component.Set(ModeKey, Mode);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ContaminationSensorKey);
		Threshold = component.Get(ThresholdKey);
		Mode = component.Get(ModeKey);
	}

	public void DuplicateFrom(FlowSensor source)
	{
		Threshold = source.Threshold;
		Mode = source.Mode;
		UpdateOutputState();
	}

	public void Sample()
	{
		SampledFlow = (HasWaterBelow(out var floor) ? new float?(GetFlow(floor)) : ((float?)null));
		UpdateOutputState();
	}

	private float GetFlow(int floor)
	{
		return Numbers.RoundToPrecision(_threadSafeWaterMap.WaterFlowDirection(new Vector3Int(_sensorCoordinates.x, _sensorCoordinates.y, floor)).magnitude, Precision);
	}

	private bool HasWaterBelow(out int floor)
	{
		return _threadSafeWaterMap.TryGetColumnFloor(_sensorCoordinates, out floor);
	}

	private void UpdateOutputState()
	{
		_automator.SetState(SampledFlow.HasValue && Mode.Evaluate(SampledFlow.Value, Numbers.RoundToPrecision(Threshold, Precision)));
	}
}
