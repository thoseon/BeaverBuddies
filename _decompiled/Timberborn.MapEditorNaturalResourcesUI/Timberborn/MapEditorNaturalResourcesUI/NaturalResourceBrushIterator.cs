using System.Collections.Generic;
using Timberborn.Brushes;
using Timberborn.CameraSystem;
using Timberborn.GridTraversing;
using Timberborn.InputSystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.TerrainQueryingSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.MapEditorNaturalResourcesUI;

public class NaturalResourceBrushIterator
{
	private readonly InputService _inputService;

	private readonly BrushShapeIterator _brushShapeIterator;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly TerrainPicker _terrainPicker;

	private readonly CameraService _cameraService;

	private readonly ITerrainService _terrainService;

	private int? _originHeight;

	private bool _isDrawing;

	public NaturalResourceBrushIterator(InputService inputService, BrushShapeIterator brushShapeIterator, ILevelVisibilityService levelVisibilityService, TerrainPicker terrainPicker, CameraService cameraService, ITerrainService terrainService)
	{
		_inputService = inputService;
		_brushShapeIterator = brushShapeIterator;
		_levelVisibilityService = levelVisibilityService;
		_terrainPicker = terrainPicker;
		_cameraService = cameraService;
		_terrainService = terrainService;
	}

	public IEnumerable<Vector3Int> Iterate(int size, BrushShape shape)
	{
		bool wasDrawing = _isDrawing;
		_isDrawing = _inputService.MainMouseButtonHeld;
		if (!_isDrawing)
		{
			_originHeight = null;
		}
		bool originSet = false;
		foreach (Vector3Int item in IterateTerrain(size, shape))
		{
			if (!originSet && _isDrawing && !wasDrawing)
			{
				_originHeight = item.z;
				originSet = true;
			}
			if (item.z < _levelVisibilityService.MaxVisibleLevel + 1)
			{
				yield return item;
			}
		}
	}

	public void Reset()
	{
		_isDrawing = false;
		_originHeight = null;
	}

	private IEnumerable<Vector3Int> IterateTerrain(int size, BrushShape brushShape)
	{
		Ray ray = _cameraService.ScreenPointToRayInGridSpace(_inputService.MousePosition);
		TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinates(ray);
		if (!traversedCoordinates.HasValue)
		{
			yield break;
		}
		TraversedCoordinates valueOrDefault = traversedCoordinates.GetValueOrDefault();
		if (valueOrDefault.Face.z != 1)
		{
			yield break;
		}
		Vector3Int center = valueOrDefault.Coordinates + valueOrDefault.Face;
		foreach (Vector3Int item in _brushShapeIterator.IterateShape(center, size, brushShape))
		{
			if (_terrainService.TryGetRelativeHeight(item, out var relativeHeight))
			{
				int num = item.z + relativeHeight;
				int num2 = _originHeight ?? center.z;
				if (num == num2)
				{
					yield return new Vector3Int(item.x, item.y, num);
				}
			}
		}
	}
}
