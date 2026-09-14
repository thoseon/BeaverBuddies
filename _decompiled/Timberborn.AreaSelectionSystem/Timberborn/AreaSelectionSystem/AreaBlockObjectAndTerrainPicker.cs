using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockObjectPickingSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.TerrainPhysics;
using UnityEngine;

namespace Timberborn.AreaSelectionSystem;

public class AreaBlockObjectAndTerrainPicker
{
	public delegate void Callback(IEnumerable<BlockObject> blockObjects, IEnumerable<Vector3Int> terrainBlocks, Vector3Int start, Vector3Int end, bool selectionStarted, bool selectingArea);

	private readonly struct PickingResult
	{
		public IEnumerable<BlockObject> BlockObjects { get; }

		public IEnumerable<Vector3Int> TerrainBlocks { get; }

		public Vector3Int Start { get; }

		public Vector3Int End { get; }

		public bool SelectingArea { get; }

		public PickingResult(IEnumerable<BlockObject> blockObjects, IEnumerable<Vector3Int> terrainBlocks, Vector3Int start, Vector3Int end, bool selectingArea)
		{
			BlockObjects = blockObjects;
			TerrainBlocks = terrainBlocks;
			Start = start;
			End = end;
			SelectingArea = selectingArea;
		}
	}

	private readonly AreaSelectionController _areaSelectionController;

	private readonly ITerrainPhysicsService _terrainPhysicsService;

	private readonly AreaSelector _areaSelector;

	private readonly BlockObjectPicker _blockObjectPicker;

	private readonly HashSet<BlockObject> _blockObjects = new HashSet<BlockObject>();

	private readonly HashSet<Vector3Int> _terrainBlocks = new HashSet<Vector3Int>();

	private SelectionStart? _selectionStart;

	internal AreaBlockObjectAndTerrainPicker(AreaSelectionController areaSelectionController, ITerrainPhysicsService physicsService, AreaSelector areaSelector, BlockObjectPicker blockObjectPicker)
	{
		_areaSelectionController = areaSelectionController;
		_terrainPhysicsService = physicsService;
		_areaSelector = areaSelector;
		_blockObjectPicker = blockObjectPicker;
	}

	public bool PickBlockObjectsAndTerrain<T>(Callback previewCallback, Callback actionCallback, Action showNoneCallback, Func<BlockObject, bool> blockObjectFilter = null)
	{
		return _areaSelectionController.ProcessInput(delegate(Ray startRay, Ray endRay, bool selectionStarted)
		{
			PickingResult pickingResult = PickBlockObjectsAndTerrain<T>(startRay, endRay, blockObjectFilter);
			previewCallback(pickingResult.BlockObjects, pickingResult.TerrainBlocks, pickingResult.Start, pickingResult.End, selectionStarted, pickingResult.SelectingArea);
			_terrainBlocks.Clear();
			_blockObjects.Clear();
		}, delegate(Ray startRay, Ray endRay, bool selectionStarted)
		{
			PickingResult pickingResult = PickBlockObjectsAndTerrain<T>(startRay, endRay, blockObjectFilter);
			actionCallback(pickingResult.BlockObjects, pickingResult.TerrainBlocks, pickingResult.Start, pickingResult.End, selectionStarted, pickingResult.SelectingArea);
			_terrainBlocks.Clear();
			_blockObjects.Clear();
		}, showNoneCallback);
	}

	public void Reset()
	{
		_areaSelectionController.Reset();
		_selectionStart = null;
	}

	private PickingResult PickBlockObjectsAndTerrain<T>(Ray startRay, Ray endRay, Func<BlockObject, bool> blockObjectFilter)
	{
		PickingResult blockObjects = GetBlockObjects<T>(startRay, endRay, blockObjectFilter);
		_blockObjects.AddRange(blockObjects.BlockObjects);
		if (!_blockObjects.IsEmpty())
		{
			_terrainPhysicsService.GetTerrainAndBlockObjectStack(_blockObjects, _terrainBlocks, _blockObjects);
			return new PickingResult(_blockObjects.AsReadOnlyEnumerable(), _terrainBlocks.AsReadOnlyEnumerable(), blockObjects.Start, blockObjects.End, blockObjects.SelectingArea);
		}
		return blockObjects;
	}

	private PickingResult GetBlockObjects<T>(Ray startRay, Ray endRay, Func<BlockObject, bool> blockObjectFilter)
	{
		bool flag = !startRay.Equals(endRay);
		SelectionStart? selectionStart = GetSelectionStart<BlockObject>(startRay, flag);
		if (selectionStart.HasValue)
		{
			SelectionStart valueOrDefault = selectionStart.GetValueOrDefault();
			Vector3Int vector3Int = (flag ? _areaSelector.GetSelectionEnd(valueOrDefault, endRay) : valueOrDefault.Coordinates);
			return new PickingResult(from blockObject in _blockObjectPicker.PickBlockObjects(valueOrDefault, vector3Int, BlockObjectPickingMode.InsideArea, blockObjectFilter, flag)
				where blockObject.GetComponent<T>() != null
				select blockObject, Enumerable.Empty<Vector3Int>(), valueOrDefault.Coordinates, vector3Int, flag);
		}
		return new PickingResult(Enumerable.Empty<BlockObject>(), Enumerable.Empty<Vector3Int>(), default(Vector3Int), default(Vector3Int), flag);
	}

	private SelectionStart? GetSelectionStart<T>(Ray startRay, bool selectingArea)
	{
		if (!selectingArea)
		{
			_selectionStart = _areaSelector.GetSelectionStart<T>(startRay);
		}
		return _selectionStart;
	}
}
