using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Cutting;
using Timberborn.EntitySystem;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.WorldPersistence;
using Timberborn.YielderFinding;
using Timberborn.Yielding;
using UnityEngine;

namespace Timberborn.Forestry;

public class TreeCuttingArea : ISaveableSingleton, ILoadableSingleton, IPostLoadableSingleton
{
	private static readonly SingletonKey TreeCuttingAreaKey = new SingletonKey("TreeCuttingArea");

	private static readonly ListKey<Vector3Int> CuttingAreaKey = new ListKey<Vector3Int>("CuttingArea");

	private readonly ISingletonLoader _singletonLoader;

	private readonly IBlockService _blockService;

	private readonly EventBus _eventBus;

	private readonly ITerrainService _terrainService;

	private readonly MapEditorMode _mapEditorMode;

	private readonly HashSet<Vector3Int> _cuttingArea = new HashSet<Vector3Int>();

	private readonly Dictionary<Vector3Int, Yielder> _yieldersInArea = new Dictionary<Vector3Int, Yielder>();

	public IEnumerable<Yielder> YieldersInArea => _yieldersInArea.Values;

	public IEnumerable<Vector3Int> CuttingArea => _cuttingArea.AsReadOnlyEnumerable();

	public bool AnyYielderSelected => !_yieldersInArea.IsEmpty();

	public TreeCuttingArea(ISingletonLoader singletonLoader, IBlockService blockService, EventBus eventBus, ITerrainService terrainService, MapEditorMode mapEditorMode)
	{
		_singletonLoader = singletonLoader;
		_blockService = blockService;
		_eventBus = eventBus;
		_terrainService = terrainService;
		_mapEditorMode = mapEditorMode;
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(TreeCuttingAreaKey, out var objectLoader))
		{
			_cuttingArea.AddRange(objectLoader.Get(CuttingAreaKey));
		}
		_eventBus.Register(this);
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
	}

	public void PostLoad()
	{
		foreach (Vector3Int item in _cuttingArea)
		{
			AddTree(item);
		}
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			singletonSaver.GetSingleton(TreeCuttingAreaKey).Set(CuttingAreaKey, _cuttingArea);
		}
	}

	public bool IsInCuttingArea(Vector3Int coordinates)
	{
		return _cuttingArea.Contains(coordinates);
	}

	public bool HasYielder(Vector3Int coordinates)
	{
		if (IsInCuttingArea(coordinates) && _yieldersInArea.TryGetValue(coordinates, out var value))
		{
			return value.IsYieldingOrAlive();
		}
		return false;
	}

	public void AddCoordinates(IEnumerable<Vector3Int> coordinates)
	{
		foreach (Vector3Int coordinate in coordinates)
		{
			_cuttingArea.Add(coordinate);
			AddTree(coordinate);
		}
		_eventBus.Post(new TreeCuttingAreaChangedEvent(coordinatesAdded: true));
	}

	public void RemoveCoordinates(IEnumerable<Vector3Int> coordinates)
	{
		foreach (Vector3Int coordinate in coordinates)
		{
			_cuttingArea.Remove(coordinate);
			TreeComponent bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<TreeComponent>(coordinate);
			if (bottomObjectComponentAt != null)
			{
				RemoveYielder(bottomObjectComponentAt);
			}
		}
		_eventBus.Post(new TreeCuttingAreaChangedEvent());
	}

	[OnEvent]
	public void OnEntityInitialized(EntityInitializedEvent entityInitializedEvent)
	{
		TreeComponent component = entityInitializedEvent.Entity.GetComponent<TreeComponent>();
		if (component != null)
		{
			BlockObject component2 = component.GetComponent<BlockObject>();
			if (_cuttingArea.Contains(component2.Coordinates))
			{
				AddYielder(component);
				_eventBus.Post(new TreeAddedToCuttingAreaEvent(component));
			}
		}
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		TreeComponent component = entityDeletedEvent.Entity.GetComponent<TreeComponent>();
		if (component != null)
		{
			RemoveYielder(component);
		}
	}

	[OnEvent]
	public void OnCuttableCut(CuttableCutEvent cuttableCutEvent)
	{
		TreeComponent component = cuttableCutEvent.Cuttable.GetComponent<TreeComponent>();
		if (component != null)
		{
			RemoveYielder(component);
		}
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		if (_cuttingArea.RemoveWhere((Vector3Int coordinates) => CoordinatesInsideChanged(coordinates, terrainHeightChangeEventArgs)) > 0)
		{
			_eventBus.Post(new TreeCuttingAreaChangedEvent());
		}
	}

	private void AddTree(Vector3Int coordinates)
	{
		TreeComponent bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<TreeComponent>(coordinates);
		if (bottomObjectComponentAt != null)
		{
			AddYielder(bottomObjectComponentAt);
		}
	}

	private void AddYielder(TreeComponent treeComponent)
	{
		Cuttable component = treeComponent.GetComponent<Cuttable>();
		BlockObject component2 = treeComponent.GetComponent<BlockObject>();
		_yieldersInArea[component2.Coordinates] = component.Yielder;
	}

	private void RemoveYielder(TreeComponent treeComponent)
	{
		BlockObject component = treeComponent.GetComponent<BlockObject>();
		_yieldersInArea.Remove(component.Coordinates);
	}

	private bool CoordinatesInsideChanged(Vector3Int coordinates, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		TerrainHeightChange change = terrainHeightChangeEventArgs.Change;
		if (coordinates.XY() == change.Coordinates && coordinates.z <= change.To + 1)
		{
			return coordinates.z >= change.From + 1;
		}
		return false;
	}
}
