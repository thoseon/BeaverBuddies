using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.CursorToolSystem;
using Timberborn.InputSystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.MapStateSystem;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.ToolSystem;
using UnityEngine;

namespace Timberborn.ConstructionGuidelines;

public class ConstructionGuidelinesRenderingService : ILoadableSingleton, IInputProcessor, ILateUpdatableSingleton
{
	private static readonly string ShowGuidelinesKey = "ShowGuidelines";

	private static readonly float MarkerYOffset = 0.022f;

	private static readonly int GuidelinesCenterCoordinatesKey = Shader.PropertyToID("_GuidelinesCenterCoordinates");

	private readonly MapSize _mapSize;

	private readonly TileDrawerFactory _tileDrawerFactory;

	private readonly ITerrainService _terrainService;

	private readonly CursorCoordinatesPicker _cursorToolSystem;

	private readonly IBlockService _blockService;

	private readonly StackableBlockService _stackableBlockService;

	private readonly InputService _inputService;

	private readonly MouseController _mouseController;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly ToolService _toolService;

	private readonly ISpecService _specService;

	private readonly List<Matrix4x4> _tilesAtSameLevel = new List<Matrix4x4>();

	private readonly List<Matrix4x4> _footprintTiles = new List<Matrix4x4>();

	private readonly List<Matrix4x4> _tilesBelow = new List<Matrix4x4>();

	private readonly List<Matrix4x4> _tilesAbove = new List<Matrix4x4>();

	private readonly List<FootprintCoordinates> _footprintCoordinates = new List<FootprintCoordinates>();

	private MeshDrawer _tilesAtSameLevelDrawer;

	private MeshDrawer _footprintDrawer;

	private MeshDrawer _tilesBelowDrawer;

	private MeshDrawer _tilesAboveDrawer;

	private bool _guidelinesEnabled;

	private readonly List<ConstructionGuidelinesToggle> _toggles = new List<ConstructionGuidelinesToggle>();

	private bool _guidelinesKeyHeld;

	private readonly CrossParameters _lastCrossParameters = new CrossParameters();

	private int _radius;

	private bool GuidelinesVisible
	{
		get
		{
			if (_guidelinesKeyHeld || (_guidelinesEnabled && _toggles.FastAny((ConstructionGuidelinesToggle toggle) => toggle.Visible)))
			{
				return !_inputService.MouseOverUI;
			}
			return false;
		}
	}

	internal ConstructionGuidelinesRenderingService(MapSize mapSize, TileDrawerFactory tileDrawerFactory, ITerrainService terrainService, CursorCoordinatesPicker cursorToolSystem, IBlockService blockService, StackableBlockService stackableBlockService, InputService inputService, MouseController mouseController, ILevelVisibilityService levelVisibilityService, ToolService toolService, ISpecService specService)
	{
		_mapSize = mapSize;
		_tileDrawerFactory = tileDrawerFactory;
		_terrainService = terrainService;
		_cursorToolSystem = cursorToolSystem;
		_blockService = blockService;
		_stackableBlockService = stackableBlockService;
		_inputService = inputService;
		_mouseController = mouseController;
		_levelVisibilityService = levelVisibilityService;
		_toolService = toolService;
		_specService = specService;
	}

	public void Load()
	{
		_radius = _specService.GetSingleSpec<ConstructionGuidelinesSpec>().Radius;
		_tilesAtSameLevelDrawer = _tileDrawerFactory.CrateSameLevelTileDrawer();
		_footprintDrawer = _tileDrawerFactory.CreateFootprintTileDrawer();
		_tilesBelowDrawer = _tileDrawerFactory.CreateBelowTileDrawer();
		_tilesAboveDrawer = _tileDrawerFactory.CreateAboveTileDrawer();
		_inputService.AddInputProcessor(this);
		_levelVisibilityService.MaxVisibleLevelChanged += delegate
		{
			UpdateBlockObjectPreviewTiles(_lastCrossParameters.Min, _lastCrossParameters.Max, _lastCrossParameters.Center, _footprintCoordinates, forceUpdate: true);
		};
	}

	public bool ProcessInput()
	{
		_guidelinesKeyHeld = _inputService.IsKeyHeld(ShowGuidelinesKey);
		return false;
	}

	public void LateUpdateSingleton()
	{
		if (GuidelinesVisible && _mouseController.IsCursorVisible)
		{
			if (!(_toolService.ActiveTool is IBlockObjectGridTool) && HasCenterMoved())
			{
				_footprintTiles.Clear();
				GetGuidelinesFromMousePosition();
			}
			_tilesAtSameLevelDrawer.DrawMultipleInstanced(_tilesAtSameLevel);
			_tilesBelowDrawer.DrawMultipleInstanced(_tilesBelow);
			_tilesAboveDrawer.DrawMultipleInstanced(_tilesAbove);
		}
		else if (_tilesAtSameLevel.Count > 0)
		{
			_tilesAtSameLevel.Clear();
			_tilesBelow.Clear();
			_tilesAbove.Clear();
			_footprintCoordinates.Clear();
			_lastCrossParameters.Reset();
		}
	}

	public ConstructionGuidelinesToggle GetConstructionGuidelinesToggle()
	{
		ConstructionGuidelinesToggle constructionGuidelinesToggle = new ConstructionGuidelinesToggle();
		_toggles.Add(constructionGuidelinesToggle);
		return constructionGuidelinesToggle;
	}

	public void SetPreviewFootprint(Vector2Int min, Vector2Int max, Vector3 center, IReadOnlyCollection<FootprintCoordinates> footprintCoordinates)
	{
		if (GuidelinesVisible)
		{
			UpdateBlockObjectPreviewTiles(min, max, center, footprintCoordinates);
			_footprintDrawer.DrawMultipleInstanced(_footprintTiles);
		}
	}

	public void EnableGuidelines()
	{
		_guidelinesEnabled = true;
	}

	public void DisableGuidelines()
	{
		_guidelinesEnabled = false;
	}

	private bool HasCenterMoved()
	{
		if (TryFindCenter(out var center))
		{
			return _lastCrossParameters.CrossParametersUpdated(center, center.XY(), center.XY(), isFromPreview: false);
		}
		return false;
	}

	private void GetGuidelinesFromMousePosition()
	{
		SetCenterPosition(_lastCrossParameters.Center);
		IEnumerable<Vector2Int> guidelinesCoordinates = GetGuidelinesCoordinates(_lastCrossParameters.Center, _lastCrossParameters.Min, _lastCrossParameters.Max);
		AddCoordinatesToGuidelines(_lastCrossParameters.Center, guidelinesCoordinates);
		_tilesAtSameLevel.Add(CreateMatrix(_lastCrossParameters.Center, MarkerYOffset));
	}

	private void UpdateBlockObjectPreviewTiles(Vector2Int min, Vector2Int max, Vector3 center, IReadOnlyCollection<FootprintCoordinates> footprintCoordinates)
	{
		_footprintCoordinates.Clear();
		_footprintCoordinates.AddRange(footprintCoordinates);
		UpdateBlockObjectPreviewTiles(min, max, center, _footprintCoordinates);
	}

	private void UpdateBlockObjectPreviewTiles(Vector2Int min, Vector2Int max, Vector3 center, List<FootprintCoordinates> footprintCoordinates, bool forceUpdate = false)
	{
		Vector3Int center2 = center.FloorToInt();
		if (forceUpdate || _lastCrossParameters.CrossParametersUpdated(center2, min, max, isFromPreview: true))
		{
			UpdateFootprintTiles(center, footprintCoordinates);
			IEnumerable<Vector2Int> guidelinesCoordinates = GetGuidelinesCoordinates(center, min, max);
			IEnumerable<Vector2Int> tilesInsideFootprint = GetTilesInsideFootprint(min, max, footprintCoordinates);
			IEnumerable<Vector2Int> guidelinesCoordinates2 = guidelinesCoordinates.Concat(tilesInsideFootprint);
			AddCoordinatesToGuidelines(center, guidelinesCoordinates2);
			SetCenterPosition(center);
		}
	}

	private bool TryFindCenter(out Vector3Int center)
	{
		CursorCoordinates? cursorCoordinates = _cursorToolSystem.Pick();
		if (cursorCoordinates.HasValue)
		{
			center = cursorCoordinates.GetValueOrDefault().TileCoordinates;
			return true;
		}
		center = default(Vector3Int);
		return false;
	}

	private static void SetCenterPosition(Vector3 center)
	{
		Shader.SetGlobalVector(GuidelinesCenterCoordinatesKey, CoordinateSystem.GridToWorld(center));
	}

	private IEnumerable<Vector2Int> GetGuidelinesCoordinates(Vector3 center, Vector2Int min, Vector2Int max)
	{
		Vector3Int mapSize = _mapSize.TotalSize;
		int num = Math.Max(0, Mathf.FloorToInt(center.x - (float)_radius)) - 1;
		int maxX = Math.Min(mapSize.x, Mathf.CeilToInt(center.x + (float)_radius)) + 1;
		for (int x = num; x < maxX; x++)
		{
			for (int y = min.y; y <= max.y; y++)
			{
				if (x < min.x || x > max.x)
				{
					yield return new Vector2Int(x, y);
				}
			}
		}
		int num2 = Math.Max(0, Mathf.FloorToInt(center.y - (float)_radius)) - 1;
		int maxY = Math.Min(mapSize.y, Mathf.CeilToInt(center.y + (float)_radius)) + 1;
		for (int x = num2; x < maxY; x++)
		{
			for (int y = min.x; y <= max.x; y++)
			{
				if (x > max.y || x < min.y)
				{
					yield return new Vector2Int(y, x);
				}
			}
		}
	}

	private void AddCoordinatesToGuidelines(Vector3 center, IEnumerable<Vector2Int> guidelinesCoordinates)
	{
		_tilesAtSameLevel.Clear();
		_tilesBelow.Clear();
		_tilesAbove.Clear();
		foreach (Vector3Int groundOrStackableBlock in _stackableBlockService.GetGroundOrStackableBlocks(guidelinesCoordinates))
		{
			int num = Mathf.RoundToInt(center.z);
			if (num <= _levelVisibilityService.MaxVisibleLevel)
			{
				if (groundOrStackableBlock.z == num)
				{
					_tilesAtSameLevel.Add(CreateMatrix(groundOrStackableBlock, MarkerYOffset));
				}
				else if (groundOrStackableBlock.z < num)
				{
					_tilesBelow.Add(CreateMatrix(groundOrStackableBlock, MarkerYOffset));
				}
				else
				{
					_tilesAbove.Add(CreateMatrix(groundOrStackableBlock, MarkerYOffset));
				}
			}
		}
	}

	private void UpdateFootprintTiles(Vector3 center, IReadOnlyCollection<FootprintCoordinates> footprintCoordinates)
	{
		_footprintTiles.Clear();
		foreach (FootprintCoordinates footprintCoordinate in footprintCoordinates)
		{
			Vector3Int coordinates = footprintCoordinate.Coordinates;
			int heightBelowFootprint = GetHeightBelowFootprint(coordinates, (int)center.z);
			if ((heightBelowFootprint <= _levelVisibilityService.MaxVisibleLevel && heightBelowFootprint < Mathf.RoundToInt(coordinates.z)) || (footprintCoordinate.CanHaveFootprint && heightBelowFootprint <= Mathf.RoundToInt(coordinates.z)))
			{
				_footprintTiles.Add(CreateMatrix(new Vector3Int(coordinates.x, coordinates.y, heightBelowFootprint), MarkerYOffset));
			}
		}
	}

	private static Matrix4x4 CreateMatrix(Vector3Int coordinates, float markerYOffset)
	{
		return Matrix4x4.TRS(CoordinateSystem.GridToWorld(coordinates) + new Vector3(0.5f, markerYOffset, 0.5f), Quaternion.identity, Vector3.one);
	}

	private static IEnumerable<Vector2Int> GetTilesInsideFootprint(Vector2Int min, Vector2Int max, IReadOnlyCollection<FootprintCoordinates> footprintCoordinates)
	{
		for (int x = min.x; x <= max.x; x++)
		{
			int y;
			for (y = min.y; y <= max.y; y++)
			{
				if (!footprintCoordinates.Any((FootprintCoordinates coordinates) => coordinates.Coordinates.x == x && coordinates.Coordinates.y == y))
				{
					yield return new Vector2Int(x, y);
				}
			}
		}
	}

	private int GetHeightBelowFootprint(Vector3Int coordinates, int previewHeight)
	{
		int terrainHeight = _terrainService.GetTerrainHeight(coordinates);
		for (int num = previewHeight; num >= terrainHeight; num--)
		{
			Vector3Int vector3Int = new Vector3Int(coordinates.x, coordinates.y, num);
			if (_blockService.AnyObjectAt(vector3Int) && _stackableBlockService.IsStackableBlockAt(vector3Int))
			{
				return num + 1;
			}
		}
		return terrainHeight;
	}
}
