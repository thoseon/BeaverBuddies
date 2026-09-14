using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WaterBuildings;

public class StreamGauge : TickableComponent, IAwakableComponent, IFinishedPostLoadStateListener, IPersistentEntity
{
	private static readonly ComponentKey StreamGaugeKey = new ComponentKey("StreamGauge");

	private static readonly PropertyKey<float> HighestWaterLevelKey = new PropertyKey<float>("HighestWaterLevel");

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private BlockObject _blockObject;

	private StreamGaugeAnimationController _streamGaugeAnimationController;

	private StreamGaugeSpec _streamGaugeSpec;

	private Vector3Int _coordinates;

	public float WaterLevel { get; private set; }

	public float HighestWaterLevel { get; private set; }

	public float WaterCurrent { get; private set; }

	public float ContaminationLevel { get; private set; }

	public StreamGauge(IThreadSafeWaterMap threadSafeWaterMap)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_streamGaugeAnimationController = GetComponent<StreamGaugeAnimationController>();
		_streamGaugeSpec = GetComponent<StreamGaugeSpec>();
		DisableComponent();
	}

	public override void Tick()
	{
		Update();
		UpdateHighestWaterLevel();
	}

	public void ResetHighestWaterLevel()
	{
		HighestWaterLevel = 0f;
		UpdateMarkerHeight();
	}

	public void OnEnterFinishedPostLoadState()
	{
		SetCoordinates();
		Update();
		UpdateHighestWaterLevel();
		UpdateMarkerHeight();
		EnableComponent();
	}

	public void OnExitFinishedPostLoadState()
	{
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(StreamGaugeKey).Set(HighestWaterLevelKey, HighestWaterLevel);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(StreamGaugeKey);
		HighestWaterLevel = component.Get(HighestWaterLevelKey);
	}

	private void Update()
	{
		if (_threadSafeWaterMap.WaterHeightOrFloor(_coordinates) > (float)_blockObject.CoordinatesAtBaseZ.z)
		{
			SetValues();
		}
		else
		{
			ResetValues();
		}
	}

	private void SetValues()
	{
		float value = _threadSafeWaterMap.WaterHeightOrFloor(_coordinates) - (float)_blockObject.CoordinatesAtBaseZ.z;
		WaterLevel = Mathf.Clamp(value, 0f, _streamGaugeSpec.MaxWaterLevel);
		Vector2 vector = _threadSafeWaterMap.WaterFlowDirection(_coordinates);
		WaterCurrent = Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
		ContaminationLevel = _threadSafeWaterMap.ColumnContamination(_coordinates);
	}

	private void ResetValues()
	{
		WaterLevel = 0f;
		WaterCurrent = 0f;
		ContaminationLevel = 0f;
	}

	private void UpdateHighestWaterLevel()
	{
		if (WaterLevel > HighestWaterLevel)
		{
			HighestWaterLevel = WaterLevel;
			UpdateMarkerHeight();
		}
	}

	private void UpdateMarkerHeight()
	{
		_streamGaugeAnimationController.SetHeight(HighestWaterLevel);
	}

	private void SetCoordinates()
	{
		_coordinates = _blockObject.PositionedBlocks.GetOccupiedCoordinates().First();
	}
}
