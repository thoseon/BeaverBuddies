using System;
using System.Collections.Generic;
using Timberborn.BlockObjectPickingSystem;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Brushes;
using Timberborn.CameraSystem;
using Timberborn.Common;
using Timberborn.GridTraversing;
using Timberborn.InputSystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Localization;
using Timberborn.MapEditorConstructionGuidelinesUI;
using Timberborn.MapStateSystem;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using Timberborn.TerrainQueryingSystem;
using Timberborn.TerrainSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using Timberborn.UndoSystem;
using UnityEngine;

namespace Timberborn.MapEditorBrushesUI;

public class RelativeTerrainHeightBrushTool : ITool, IToolDescriptor, IInputProcessor, ILoadableSingleton, IBrushWithSize, IBrushWithShape, IBrushWithHeight, IBrushWithDirection, IBrushWithGuidelines
{
	private static readonly string TitleLocKey = "MapEditor.Brush.RelativeTerrainHeight";

	private static readonly float MarkerYOffset = 0.02f;

	private readonly InputService _inputService;

	private readonly ITerrainService _terrainService;

	private readonly BrushShapeIterator _brushShapeIterator;

	private readonly TerrainPicker _terrainPicker;

	private readonly CameraService _cameraService;

	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly TerrainIntegrityService _terrainIntegrityService;

	private readonly ILoc _loc;

	private readonly BlockObjectRaycaster _blockObjectRaycaster;

	private readonly StackableBlockService _stackableBlockService;

	private readonly IUndoRegistry _undoRegistry;

	private readonly MapSize _mapSize;

	private readonly ISpecService _specService;

	private BrushColorSpec _brushColorSpec;

	private readonly List<Vector3Int> _brushCoordinates = new List<Vector3Int>();

	private MeshDrawer _markerDrawer;

	private ToolDescription _toolDescription;

	private bool _isDrawing;

	private Vector3Int _drawingOrigin;

	public int BrushSize { get; set; } = 5;

	public int BrushHeight { get; set; } = 1;

	public BrushShape BrushShape { get; set; }

	public bool Increase { get; set; } = true;

	public bool Inverse { get; set; }

	public int MinimumBrushHeight => 1;

	public bool IsIncreasing
	{
		get
		{
			if (!Increase || Inverse)
			{
				if (!Increase)
				{
					return Inverse;
				}
				return false;
			}
			return true;
		}
	}

	public RelativeTerrainHeightBrushTool(InputService inputService, ITerrainService terrainService, BrushShapeIterator brushShapeIterator, TerrainPicker terrainPicker, CameraService cameraService, MarkerDrawerFactory markerDrawerFactory, ILevelVisibilityService levelVisibilityService, TerrainIntegrityService terrainIntegrityService, ILoc loc, BlockObjectRaycaster blockObjectRaycaster, StackableBlockService stackableBlockService, IUndoRegistry undoRegistry, MapSize mapSize, ISpecService specService)
	{
		_inputService = inputService;
		_terrainService = terrainService;
		_brushShapeIterator = brushShapeIterator;
		_terrainPicker = terrainPicker;
		_cameraService = cameraService;
		_markerDrawerFactory = markerDrawerFactory;
		_levelVisibilityService = levelVisibilityService;
		_terrainIntegrityService = terrainIntegrityService;
		_loc = loc;
		_blockObjectRaycaster = blockObjectRaycaster;
		_stackableBlockService = stackableBlockService;
		_undoRegistry = undoRegistry;
		_mapSize = mapSize;
		_specService = specService;
	}

	public void Load()
	{
		_brushColorSpec = _specService.GetSingleSpec<BrushColorSpec>();
		_toolDescription = new ToolDescription.Builder(_loc.T(TitleLocKey)).Build();
		_markerDrawer = _markerDrawerFactory.CreateTileDrawer();
	}

	public bool ProcessInput()
	{
		ProcessBrush();
		return false;
	}

	public void Enter()
	{
		_inputService.AddInputProcessor(this);
	}

	public void Exit()
	{
		_terrainIntegrityService.ClearHighlight();
		_inputService.RemoveInputProcessor(this);
		_undoRegistry.CommitStack();
	}

	public ToolDescription DescribeTool()
	{
		return _toolDescription;
	}

	private void ProcessBrush()
	{
		Ray ray = _cameraService.ScreenPointToRayInGridSpace(_inputService.MousePosition);
		bool justStartedDrawing = UpdateDrawingState(ray);
		if (_isDrawing)
		{
			ApplyBrush(ray, justStartedDrawing);
		}
		else
		{
			PreviewBrush(ray);
		}
	}

	private bool UpdateDrawingState(Ray ray)
	{
		if (!_isDrawing)
		{
			if (TryGetCursorCoordinates(ray, out var coordinates))
			{
				_drawingOrigin = coordinates;
				if (_inputService.MainMouseButtonHeld && !_inputService.MouseOverUI)
				{
					_isDrawing = true;
					return true;
				}
			}
		}
		else if (!_inputService.MainMouseButtonHeld)
		{
			_isDrawing = false;
			_undoRegistry.CommitStack();
		}
		return false;
	}

	private void ApplyBrush(Ray ray, bool justStartedDrawing)
	{
		if (!TryPickCoordinates(ray, justStartedDrawing, out var center))
		{
			return;
		}
		UpdateBrushCoordinates(center);
		_terrainIntegrityService.RemoveViolatingElements(GetCoordinatesToCleanupBlockObjects(), GetCoordinatesToValidateIntegrity());
		int num = (IsIncreasing ? BrushHeight : (-BrushHeight));
		foreach (Vector3Int brushCoordinate in _brushCoordinates)
		{
			if (brushCoordinate.z == _drawingOrigin.z && InMapRange(brushCoordinate.z))
			{
				Vector3Int vector3Int = new Vector3Int(brushCoordinate.x, brushCoordinate.y, brushCoordinate.z);
				if (num > 0)
				{
					int heightChange = Math.Min(brushCoordinate.z + num, _mapSize.MaxMapEditorTerrainHeight) - brushCoordinate.z;
					_terrainService.SetTerrain(vector3Int, heightChange);
				}
				else
				{
					_terrainService.UnsetTerrain(vector3Int.Below(), Math.Abs(num));
				}
			}
		}
		_brushCoordinates.Clear();
	}

	private void PreviewBrush(Ray ray)
	{
		_terrainIntegrityService.ClearHighlight();
		if (TryGetCursorCoordinates(ray, out var coordinates))
		{
			UpdateBrushCoordinates(coordinates);
			DrawTileMarkers();
			_terrainIntegrityService.HighlightViolatingElements(GetCoordinatesToCleanupBlockObjects(), GetCoordinatesToValidateIntegrity());
			_brushCoordinates.Clear();
		}
	}

	private bool TryPickCoordinates(Ray ray, bool justStartedDrawing, out Vector3Int center)
	{
		if (justStartedDrawing && TryGetCursorCoordinates(ray, out center))
		{
			return true;
		}
		if (TryGetCursorForStackableBlockObject(ray, out center) && center.z == _drawingOrigin.z)
		{
			return true;
		}
		TraversedCoordinates? traversedCoordinates = _terrainPicker.FindCoordinatesOnLevelInMap(ray, _drawingOrigin.z);
		if (traversedCoordinates.HasValue)
		{
			center = traversedCoordinates.GetValueOrDefault().Coordinates;
			return true;
		}
		center = default(Vector3Int);
		return false;
	}

	private void DrawTileMarkers()
	{
		foreach (Vector3Int brushCoordinate in _brushCoordinates)
		{
			Vector3Int coordinates = new Vector3Int(brushCoordinate.x, brushCoordinate.y, _drawingOrigin.z);
			if (TryGetRelativeHeight(coordinates, out var relativeHeight))
			{
				int z = coordinates.z + relativeHeight;
				Vector3Int coordinates2 = new Vector3Int(brushCoordinate.x, brushCoordinate.y, z);
				Color color = ((relativeHeight != 0 || !InMapRange(coordinates2.z)) ? _brushColorSpec.Neutral : (IsIncreasing ? _brushColorSpec.Positive : _brushColorSpec.Negative));
				_markerDrawer.DrawAtCoordinates(coordinates2, MarkerYOffset, color);
			}
		}
	}

	private bool TryGetCursorCoordinates(Ray ray, out Vector3Int coordinates)
	{
		bool flag = TryGetCursorForStackableBlockObject(ray, out coordinates);
		TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinates(ray);
		if (traversedCoordinates.HasValue)
		{
			TraversedCoordinates valueOrDefault = traversedCoordinates.GetValueOrDefault();
			if (!flag || valueOrDefault.CoordinatesWithFaceOffset.z > coordinates.z)
			{
				coordinates = valueOrDefault.CoordinatesWithFaceOffset;
			}
			return true;
		}
		return false;
	}

	private bool TryGetRelativeHeight(Vector3Int coordinates, out int relativeHeight)
	{
		if (!_terrainService.Underground(coordinates) && _stackableBlockService.IsFinishedStackableBlockAt(coordinates.Below()))
		{
			relativeHeight = 0;
			return true;
		}
		return _terrainService.TryGetRelativeHeight(coordinates, out relativeHeight);
	}

	private void UpdateBrushCoordinates(Vector3Int center)
	{
		foreach (Vector3Int item in _brushShapeIterator.IterateShape(center, BrushSize, BrushShape))
		{
			if (AreCoordinatesValid(item, out var height))
			{
				_brushCoordinates.Add(new Vector3Int(item.x, item.y, height));
			}
		}
	}

	private bool AreCoordinatesValid(Vector3Int coordinates, out int height)
	{
		height = GetHeight(coordinates);
		return height < _levelVisibilityService.MaxVisibleLevel + 1;
	}

	private int GetHeight(Vector3Int coordinates)
	{
		if (!_terrainService.Underground(coordinates) && _stackableBlockService.IsFinishedStackableBlockAt(coordinates.Below()))
		{
			return coordinates.z;
		}
		if (_terrainService.TryGetRelativeHeight(coordinates, out var relativeHeight))
		{
			return coordinates.z + relativeHeight;
		}
		return int.MaxValue;
	}

	private bool InMapRange(int height)
	{
		if (!IsIncreasing)
		{
			return height > 0;
		}
		return height < _mapSize.MaxMapEditorTerrainHeight;
	}

	private IEnumerable<Vector3Int> GetCoordinatesToCleanupBlockObjects()
	{
		foreach (Vector3Int coordinates in _brushCoordinates)
		{
			int z = coordinates.z;
			if (z != _drawingOrigin.z)
			{
				continue;
			}
			int num = (IsIncreasing ? z : (z - BrushHeight));
			int endHeight = (IsIncreasing ? (z + BrushHeight) : z);
			for (int i = num; i < endHeight; i++)
			{
				if (i >= 0 && i < _mapSize.MaxMapEditorTerrainHeight)
				{
					yield return new Vector3Int(coordinates.x, coordinates.y, i);
				}
			}
		}
	}

	private IEnumerable<Vector3Int> GetCoordinatesToValidateIntegrity()
	{
		if (IsIncreasing)
		{
			yield break;
		}
		foreach (Vector3Int coordinates in _brushCoordinates)
		{
			if (coordinates.z == _drawingOrigin.z)
			{
				for (int z = 0; z <= BrushHeight; z++)
				{
					yield return new Vector3Int(coordinates.x, coordinates.y, coordinates.z - z);
				}
			}
		}
	}

	private bool TryGetCursorForStackableBlockObject(Ray ray, out Vector3Int coordinates)
	{
		if (_blockObjectRaycaster.TryHitBlockObject<BlockObject>(ray, out var blockObjectHit) && blockObjectHit.HitBlock.Stackable == BlockStackable.BlockObject)
		{
			coordinates = blockObjectHit.HitBlock.Coordinates.Above();
			return true;
		}
		coordinates = default(Vector3Int);
		return false;
	}
}
