using System.Collections.Generic;
using System.Linq;
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

public class AbsoluteTerrainHeightBrushTool : ITool, IToolDescriptor, IInputProcessor, ILoadableSingleton, IBrushWithSize, IBrushWithShape, IBrushWithHeight, IBrushWithGuidelines
{
	private static readonly string TitleLocKey = "MapEditor.Brush.AbsoluteTerrainHeight";

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

	private readonly IUndoRegistry _undoRegistry;

	private readonly MapSize _mapSize;

	private readonly ISpecService _specService;

	private BrushColorSpec _brushColorSpec;

	private readonly List<Vector3Int> _brushCoordinates = new List<Vector3Int>();

	private MeshDrawer _markerDrawer;

	private ToolDescription _toolDescription;

	private bool _isDrawing;

	private Vector3Int _drawingOrigin;

	private bool _isRegisteringUndo;

	public int BrushSize { get; set; } = 3;

	public int BrushHeight { get; set; } = 1;

	public BrushShape BrushShape { get; set; }

	public int MinimumBrushHeight => 0;

	public AbsoluteTerrainHeightBrushTool(InputService inputService, ITerrainService terrainService, BrushShapeIterator brushShapeIterator, TerrainPicker terrainPicker, CameraService cameraService, MarkerDrawerFactory markerDrawerFactory, ILevelVisibilityService levelVisibilityService, TerrainIntegrityService terrainIntegrityService, ILoc loc, IUndoRegistry undoRegistry, MapSize mapSize, ISpecService specService)
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
		UpdateDrawingState(ray);
		if (_isDrawing)
		{
			ApplyBrush(ray);
		}
		else
		{
			PreviewBrush(ray);
		}
	}

	private void UpdateDrawingState(Ray ray)
	{
		if (!_isDrawing)
		{
			TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinates(ray);
			if (traversedCoordinates.HasValue)
			{
				TraversedCoordinates valueOrDefault = traversedCoordinates.GetValueOrDefault();
				_drawingOrigin = valueOrDefault.Coordinates + valueOrDefault.Face;
				if (_inputService.MainMouseButtonDown && !_inputService.MouseOverUI)
				{
					_isDrawing = true;
				}
			}
		}
		else if (!_inputService.MainMouseButtonHeld)
		{
			_isDrawing = false;
			_undoRegistry.CommitStack();
		}
	}

	private void ApplyBrush(Ray ray)
	{
		TraversedCoordinates? traversedCoordinates = _terrainPicker.FindCoordinatesOnLevelInMap(ray, _drawingOrigin.z);
		if (!traversedCoordinates.HasValue)
		{
			return;
		}
		UpdateBrushCoordinates(traversedCoordinates.GetValueOrDefault().Coordinates);
		_terrainIntegrityService.RemoveViolatingElements(GetCoordinatesToCleanupBlockObjects(), GetCoordinatesToValidateIntegrity());
		foreach (Vector3Int brushCoordinate in _brushCoordinates)
		{
			int terrainHeight = _terrainService.GetTerrainHeight(brushCoordinate);
			Vector3Int vector3Int = new Vector3Int(brushCoordinate.x, brushCoordinate.y, terrainHeight);
			if (terrainHeight > BrushHeight)
			{
				_terrainService.UnsetTerrain(vector3Int.Below(), terrainHeight - BrushHeight);
			}
			else if (terrainHeight < BrushHeight)
			{
				_terrainService.SetTerrain(vector3Int, BrushHeight - terrainHeight);
			}
		}
		DrawTileMarkers();
		_brushCoordinates.Clear();
	}

	private void PreviewBrush(Ray ray)
	{
		_terrainIntegrityService.ClearHighlight();
		TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinates(ray);
		if (traversedCoordinates.HasValue)
		{
			TraversedCoordinates valueOrDefault = traversedCoordinates.GetValueOrDefault();
			Vector3Int center = valueOrDefault.Coordinates + valueOrDefault.Face;
			UpdateBrushCoordinates(center);
			DrawTileMarkers();
			_terrainIntegrityService.HighlightViolatingElements(GetCoordinatesToCleanupBlockObjects(), GetCoordinatesToValidateIntegrity());
			_brushCoordinates.Clear();
		}
	}

	private void DrawTileMarkers()
	{
		foreach (Vector3Int brushCoordinate in _brushCoordinates)
		{
			int terrainHeight = _terrainService.GetTerrainHeight(brushCoordinate);
			Vector3Int coordinates = new Vector3Int(brushCoordinate.x, brushCoordinate.y, terrainHeight);
			int num = BrushHeight - terrainHeight;
			Color color = ((num == 0) ? _brushColorSpec.Neutral : ((num > 0) ? _brushColorSpec.Positive : _brushColorSpec.Negative));
			_markerDrawer.DrawAtCoordinates(coordinates, MarkerYOffset, color);
		}
	}

	private void UpdateBrushCoordinates(Vector3Int center)
	{
		_brushCoordinates.AddRange(_brushShapeIterator.IterateShape(center, BrushSize, BrushShape).Where(AreCoordinatesValid));
	}

	private bool AreCoordinatesValid(Vector3Int coordinates)
	{
		if (_terrainService.TryGetRelativeHeight(coordinates, out var relativeHeight))
		{
			return coordinates.z + relativeHeight < _levelVisibilityService.MaxVisibleLevel + 1;
		}
		return false;
	}

	private IEnumerable<Vector3Int> GetCoordinatesToCleanupBlockObjects()
	{
		foreach (Vector3Int coordinates in _brushCoordinates)
		{
			if (!_terrainService.TryGetRelativeHeight(coordinates, out var relativeHeight))
			{
				continue;
			}
			int num = coordinates.z + relativeHeight;
			int num2 = ((BrushHeight > num) ? num : BrushHeight);
			int endHeight = ((BrushHeight > num) ? BrushHeight : num);
			for (int z = num2; z < endHeight; z++)
			{
				if (z >= 0 && z < _mapSize.MaxMapEditorTerrainHeight)
				{
					yield return new Vector3Int(coordinates.x, coordinates.y, z);
				}
			}
		}
	}

	private IEnumerable<Vector3Int> GetCoordinatesToValidateIntegrity()
	{
		foreach (Vector3Int coordinates in _brushCoordinates)
		{
			if (_terrainService.TryGetRelativeHeight(coordinates, out var relativeHeight))
			{
				int terrainHeight = coordinates.z + relativeHeight;
				for (int i = BrushHeight; i <= terrainHeight; i++)
				{
					yield return new Vector3Int(coordinates.x, coordinates.y, i);
				}
			}
		}
	}
}
