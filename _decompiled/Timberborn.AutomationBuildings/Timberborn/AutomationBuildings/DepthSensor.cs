using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WaterSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

public class DepthSensor : BaseComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity, IDuplicable<DepthSensor>, IDuplicable, ISamplingTransmitter, ITransmitter
{
	private static readonly ComponentKey DepthSensorKey = new ComponentKey("DepthSensor");

	private static readonly PropertyKey<float> ThresholdKey = new PropertyKey<float>("Threshold");

	private static readonly PropertyKey<NumericComparisonMode> ModeKey = new PropertyKey<NumericComparisonMode>("Mode");

	private static readonly float DefaultDepthOffset = 0.5f;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private Automator _automator;

	private BlockObject _blockObject;

	private DepthSensorSpec _depthSensorSpec;

	private float? _rawThreshold;

	private float _sampledWaterHeight;

	private int _sampledFloor;

	public Vector3Int SensorCoordinates { get; private set; }

	public NumericComparisonMode Mode { get; private set; }

	public int MinThreshold => _sampledFloor;

	public float MaxThreshold => (float)SensorCoordinates.z + _depthSensorSpec.SensorHeightOffset;

	public float Threshold => Mathf.Clamp(_rawThreshold ?? throw new Exception("_rawThreshold not initialzed."), MinThreshold, MaxThreshold);

	public float ThresholdFromFloor => Threshold - (float)_sampledFloor;

	public float Depth => Mathf.Clamp(_sampledWaterHeight, MinThreshold, MaxThreshold);

	public float DepthFromFloor => Mathf.Clamp(_sampledWaterHeight - (float)_sampledFloor, 0f, MaxThreshold - (float)MinThreshold);

	public DepthSensor(IThreadSafeWaterMap threadSafeWaterMap)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void Awake()
	{
		_automator = GetComponent<Automator>();
		_blockObject = GetComponent<BlockObject>();
		_depthSensorSpec = GetComponent<DepthSensorSpec>();
		DisableComponent();
	}

	public void InitializeEntity()
	{
		InitializeSensorCoordinates();
		float valueOrDefault = _rawThreshold.GetValueOrDefault();
		if (!_rawThreshold.HasValue)
		{
			valueOrDefault = (float)SensorCoordinates.z - DefaultDepthOffset;
			_rawThreshold = valueOrDefault;
		}
	}

	public void SetThreshold(float value)
	{
		if (!_rawThreshold.Equals(value))
		{
			_rawThreshold = value;
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
		IObjectSaver component = entitySaver.GetComponent(DepthSensorKey);
		if (_rawThreshold.HasValue)
		{
			component.Set(ThresholdKey, _rawThreshold.Value);
		}
		component.Set(ModeKey, Mode);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(DepthSensorKey);
		if (component.Has(ThresholdKey))
		{
			_rawThreshold = component.Get(ThresholdKey);
		}
		Mode = component.Get(ModeKey);
	}

	public void DuplicateFrom(DepthSensor source)
	{
		InitializeSensorCoordinates();
		_rawThreshold = (float)GetCurrentFloor() + source.ThresholdFromFloor;
		Mode = source.Mode;
		UpdateOutputState();
	}

	public void Sample()
	{
		_sampledWaterHeight = _threadSafeWaterMap.WaterHeightOrFloor(SensorCoordinates);
		_sampledFloor = GetCurrentFloor();
		UpdateOutputState();
	}

	private void InitializeSensorCoordinates()
	{
		SensorCoordinates = _blockObject.TransformCoordinates(_depthSensorSpec.SensorCoordinates);
	}

	private void UpdateOutputState()
	{
		_automator.SetState(Mode.Evaluate(_sampledWaterHeight, Threshold));
	}

	private int GetCurrentFloor()
	{
		if (!_threadSafeWaterMap.TryGetColumnFloor(SensorCoordinates, out var floor))
		{
			return SensorCoordinates.z;
		}
		return floor;
	}
}
