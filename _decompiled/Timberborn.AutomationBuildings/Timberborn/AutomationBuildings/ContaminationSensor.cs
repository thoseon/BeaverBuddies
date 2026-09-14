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

public class ContaminationSensor : BaseComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity, IDuplicable<ContaminationSensor>, IDuplicable, ISamplingTransmitter, ITransmitter
{
	public static readonly float Precision = 0.01f;

	private static readonly float DefaultThreshold = 0.05f;

	private static readonly ComponentKey ContaminationSensorKey = new ComponentKey("ContaminationSensor");

	private static readonly PropertyKey<float> ThresholdKey = new PropertyKey<float>("Threshold");

	private static readonly PropertyKey<NumericComparisonMode> ModeKey = new PropertyKey<NumericComparisonMode>("Mode");

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private Automator _automator;

	private BlockObject _blockObject;

	private Vector3Int _sensorCoordinates;

	public float Threshold { get; private set; }

	public float? SampledContamination { get; private set; }

	public NumericComparisonMode Mode { get; private set; }

	public ContaminationSensor(IThreadSafeWaterMap threadSafeWaterMap)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void Awake()
	{
		_automator = GetComponent<Automator>();
		_blockObject = GetComponent<BlockObject>();
		Threshold = DefaultThreshold;
	}

	public void InitializeEntity()
	{
		_sensorCoordinates = _blockObject.TransformCoordinates(GetComponent<ContaminationSensorSpec>().SensorCoordinates);
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
		if (entityLoader.TryGetComponent(ContaminationSensorKey, out var objectLoader) || entityLoader.TryGetComponent(new ComponentKey("WaterContaminationSensor"), out objectLoader))
		{
			Threshold = objectLoader.Get(ThresholdKey);
			if (objectLoader.Has(ModeKey))
			{
				Mode = objectLoader.Get(ModeKey);
			}
		}
	}

	public void DuplicateFrom(ContaminationSensor source)
	{
		Threshold = source.Threshold;
		Mode = source.Mode;
		UpdateOutputState();
	}

	public void Sample()
	{
		SampledContamination = (HasWaterBelow(out var floor) ? new float?(GetContamination(floor)) : ((float?)null));
		UpdateOutputState();
	}

	private float GetContamination(int floor)
	{
		return Numbers.RoundToPrecision(_threadSafeWaterMap.ColumnContamination(new Vector3Int(_sensorCoordinates.x, _sensorCoordinates.y, floor)), Precision);
	}

	private bool HasWaterBelow(out int floor)
	{
		return _threadSafeWaterMap.TryGetColumnFloor(_sensorCoordinates, out floor);
	}

	private void UpdateOutputState()
	{
		_automator.SetState(SampledContamination.HasValue && Mode.Evaluate(SampledContamination.Value, Numbers.RoundToPrecision(Threshold, Precision)));
	}
}
