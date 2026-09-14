using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.BlockObstacles;

public class LayeredBlockObstacle : BaseComponent, IAwakableComponent, IFinishedStateListener, IPersistentEntity
{
	private static readonly ComponentKey LayeredVerticalBlockObstacleKey = new ComponentKey("LayeredVerticalBlockObstacle");

	private static readonly PropertyKey<float> OccupancyRangeKey = new PropertyKey<float>("OccupancyRange");

	private readonly EventBus _eventBus;

	private readonly BlockOccupationLayerFactory _blockOccupationLayerFactory;

	private readonly ITerrainService _terrainService;

	private LayeredBlockObstacleSpec _layeredBlockObstacleSpec;

	private float _anchorWorldHeight;

	private float _maxOccupancyRange;

	private readonly List<BlockOccupationLayer> _blockOccupationLayers = new List<BlockOccupationLayer>();

	public float OccupancyRange { get; private set; }

	public float MaxOccupancyRange => _maxOccupancyRange;

	public event EventHandler MaxOccupancyRangeChanged;

	public LayeredBlockObstacle(EventBus eventBus, BlockOccupationLayerFactory blockOccupationLayerFactory, ITerrainService terrainService)
	{
		_eventBus = eventBus;
		_blockOccupationLayerFactory = blockOccupationLayerFactory;
		_terrainService = terrainService;
	}

	public void Awake()
	{
		_layeredBlockObstacleSpec = GetComponent<LayeredBlockObstacleSpec>();
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(LayeredVerticalBlockObstacleKey).Set(OccupancyRangeKey, OccupancyRange);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(LayeredVerticalBlockObstacleKey);
		OccupancyRange = component.Get(OccupancyRangeKey);
	}

	public void OnEnterFinishedState()
	{
		_eventBus.Register(this);
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
		_anchorWorldHeight = base.Transform.TransformPoint(_layeredBlockObstacleSpec.AnchorPosition).y;
		CreateBlockOccupationLayers();
		EnableComponent();
		if (TryUpdateBlockOccupationLayers(float.MinValue, OccupancyRange))
		{
			UpdateMaxOccupancyRange();
		}
	}

	public void OnExitFinishedState()
	{
		_eventBus.Unregister(this);
		_terrainService.TerrainHeightChanged -= OnTerrainHeightChanged;
		RemoveBlockOccupationLayers();
		DisableComponent();
	}

	[OnEvent]
	public void OnBlockObjectSet(BlockObjectSetEvent blockObjectSetEvent)
	{
		if (base.Enabled)
		{
			IEnumerable<Vector3Int> allCoordinates = blockObjectSetEvent.BlockObject.PositionedBlocks.GetAllCoordinates();
			UpdateMaxOccupancyRangeIfCoordinatesMatch(allCoordinates);
		}
	}

	[OnEvent]
	public void OnBlockObjectUnset(BlockObjectUnsetEvent blockObjectUnsetEvent)
	{
		if (base.Enabled)
		{
			IEnumerable<Vector3Int> allCoordinates = blockObjectUnsetEvent.BlockObject.PositionedBlocks.GetAllCoordinates();
			UpdateMaxOccupancyRangeIfCoordinatesMatch(allCoordinates);
		}
	}

	public void ModifyOccupancyRange(float occupancyRangeDelta)
	{
		SetOccupancyRange(OccupancyRange + occupancyRangeDelta);
	}

	private void SetOccupancyRange(float occupancyRange)
	{
		float occupancyRange2 = OccupancyRange;
		OccupancyRange = Mathf.Clamp(occupancyRange, 0f, MaxOccupancyRange);
		if (TryUpdateBlockOccupationLayers(occupancyRange2, OccupancyRange))
		{
			UpdateMaxOccupancyRange();
		}
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		if (base.Enabled)
		{
			UpdateMaxOccupancyRangeIfCoordinatesMatch(terrainHeightChangeEventArgs.Change.Coordinates);
		}
	}

	private void CreateBlockOccupationLayers()
	{
		for (int num = GetStartingGridHeight(); num >= 0; num--)
		{
			BlockOccupationLayer item = _blockOccupationLayerFactory.Create(base.Transform, _layeredBlockObstacleSpec.AnchorPosition, num, _layeredBlockObstacleSpec.LayerSize);
			_blockOccupationLayers.Add(item);
		}
	}

	private int GetStartingGridHeight()
	{
		return Mathf.FloorToInt(_anchorWorldHeight) - _layeredBlockObstacleSpec.BlockCreationOffset;
	}

	private void RemoveBlockOccupationLayers()
	{
		foreach (BlockOccupationLayer blockOccupationLayer in _blockOccupationLayers)
		{
			blockOccupationLayer.Remove();
		}
		_blockOccupationLayers.Clear();
	}

	private bool TryUpdateBlockOccupationLayers(float oldOccupancyRange, float newOccupancyRange)
	{
		int num = Mathf.FloorToInt(_anchorWorldHeight - oldOccupancyRange);
		int num2 = Mathf.FloorToInt(_anchorWorldHeight - newOccupancyRange);
		if (num != num2)
		{
			UpdateBlockOccupationLayers(num2);
			return true;
		}
		return false;
	}

	private void UpdateBlockOccupationLayers(int minimumGridHeight)
	{
		bool flag = false;
		foreach (BlockOccupationLayer blockOccupationLayer in _blockOccupationLayers)
		{
			if (LayerIsValidToOccupy(blockOccupationLayer, minimumGridHeight) && !flag)
			{
				blockOccupationLayer.AddToServices();
				continue;
			}
			blockOccupationLayer.RemoveFromServices();
			flag = true;
		}
	}

	private static bool LayerIsValidToOccupy(BlockOccupationLayer blockOccupationLayer, int minimumGridHeight)
	{
		bool num = blockOccupationLayer.CanBeAddedToServices();
		bool flag = blockOccupationLayer.GridHeight >= minimumGridHeight;
		return num & flag;
	}

	private void UpdateMaxOccupancyRangeIfCoordinatesMatch(IEnumerable<Vector3Int> coordinates)
	{
		foreach (Vector3Int coordinate in coordinates)
		{
			if (UpdateMaxOccupancyRangeIfCoordinatesMatch(coordinate.XY()))
			{
				break;
			}
		}
	}

	private bool UpdateMaxOccupancyRangeIfCoordinatesMatch(Vector2Int coordinates)
	{
		if (_blockOccupationLayers.First().Contains(coordinates))
		{
			UpdateMaxOccupancyRange();
			return true;
		}
		return false;
	}

	private void UpdateMaxOccupancyRange()
	{
		float maxOccupancyRange = _maxOccupancyRange;
		_maxOccupancyRange = GetMaxPotentialOccupancyRange();
		DecreaseOccupancyRangeForEachInvalidLayer(ref _maxOccupancyRange);
		OccupancyRange = Mathf.Clamp(OccupancyRange, 0f, MaxOccupancyRange);
		if (!maxOccupancyRange.Equals(_maxOccupancyRange))
		{
			MaxOccupancyRangeChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private float GetMaxPotentialOccupancyRange()
	{
		int gridHeight = _blockOccupationLayers.Last().GridHeight;
		return _anchorWorldHeight - (float)gridHeight;
	}

	private void DecreaseOccupancyRangeForEachInvalidLayer(ref float occupancyRange)
	{
		for (int i = 0; i < _blockOccupationLayers.Count; i++)
		{
			if (!_blockOccupationLayers[i].CanBeAddedToServices())
			{
				int num = _blockOccupationLayers.Count - i;
				occupancyRange -= num;
				break;
			}
		}
	}
}
