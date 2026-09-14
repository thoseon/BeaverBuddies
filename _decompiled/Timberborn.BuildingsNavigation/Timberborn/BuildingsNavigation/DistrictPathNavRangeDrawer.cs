using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal class DistrictPathNavRangeDrawer : BaseComponent, IAwakableComponent, IInitializablePreview, IInitializableEntity, ILateUpdatableComponent
{
	private readonly PathMeshDrawerFactory _pathMeshDrawerFactory;

	private readonly IBlockService _blockService;

	private readonly INavMeshService _navMeshService;

	private readonly PreviewBlockService _previewBlockService;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly INavigationRangeService _navigationRangeService;

	private BuildingAccessible _buildingAccessible;

	private PathMeshDrawer _regularMeshDrawer;

	private PathMeshDrawer _stairsMeshDrawer;

	private readonly HashSet<WeightedCoordinates> _roadNodes = new HashSet<WeightedCoordinates>();

	private bool _dirty;

	private DrawingParameters _drawingParameters;

	public DistrictPathNavRangeDrawer(PathMeshDrawerFactory pathMeshDrawerFactory, IBlockService blockService, INavMeshService navMeshService, PreviewBlockService previewBlockService, ILevelVisibilityService levelVisibilityService, INavigationRangeService navigationRangeService)
	{
		_pathMeshDrawerFactory = pathMeshDrawerFactory;
		_blockService = blockService;
		_navMeshService = navMeshService;
		_previewBlockService = previewBlockService;
		_levelVisibilityService = levelVisibilityService;
		_navigationRangeService = navigationRangeService;
	}

	public void Awake()
	{
		_buildingAccessible = GetComponent<BuildingAccessible>();
		DisableComponent();
	}

	public void InitializePreview()
	{
		Initialize();
	}

	public void InitializeEntity()
	{
		Initialize();
	}

	public void LateUpdate()
	{
		if (_dirty)
		{
			UpdateAllNodes();
			UpdateDrawers();
			_dirty = false;
		}
		Draw();
		DisableComponent();
	}

	public void DrawRange(DrawingParameters drawingParameters)
	{
		if (!_drawingParameters.Equals(drawingParameters))
		{
			MarkDirty();
			_drawingParameters = drawingParameters;
		}
		EnableComponent();
	}

	public void MarkDirty()
	{
		_dirty = true;
	}

	private void Initialize()
	{
		_regularMeshDrawer = _pathMeshDrawerFactory.CreateRegularDrawer(RegularConnectionKey);
		_stairsMeshDrawer = _pathMeshDrawerFactory.CreateStairsDrawer(StairsConnectionKey);
	}

	private void UpdateAllNodes()
	{
		_roadNodes.Clear();
		Vector3? vector = (_drawingParameters.IsPreview ? new Vector3?(_buildingAccessible.CalculateAccess()) : _buildingAccessible.Accessible.UnblockedSingleAccessInstant);
		if (vector.HasValue)
		{
			IEnumerable<WeightedCoordinates> values = (_drawingParameters.IsPreview ? _navigationRangeService.GetRoadPreviewNodesInRange(vector.Value) : _navigationRangeService.GetRoadNodesInRange(vector.Value));
			_roadNodes.AddRange(values);
		}
	}

	private void Draw()
	{
		_regularMeshDrawer.Draw();
		_stairsMeshDrawer.Draw();
	}

	private void UpdateDrawers()
	{
		_regularMeshDrawer.Reset();
		_stairsMeshDrawer.Reset();
		foreach (WeightedCoordinates roadNode in _roadNodes)
		{
			AddTile(roadNode);
		}
		_regularMeshDrawer.Build();
		_stairsMeshDrawer.Build();
	}

	private void AddTile(WeightedCoordinates node)
	{
		Vector3Int coordinates = node.Coordinates;
		if (IsTileVisible(coordinates) && _levelVisibilityService.BlockIsVisible(coordinates))
		{
			if (IsConnectedToPath(coordinates, coordinates.Above()))
			{
				_stairsMeshDrawer.Add(node);
			}
			else if (!IsConnectedToPath(coordinates, coordinates.Below()))
			{
				_regularMeshDrawer.Add(node);
			}
		}
	}

	private byte StairsConnectionKey(Vector3Int coordinates, Vector3Int direction)
	{
		byte b = RegularConnectionKey(coordinates, direction);
		if (b != PathMeshConnectionKeys.Nothing)
		{
			return PathMeshConnectionKeys.ToAlternativeKey(b);
		}
		Vector3Int coordinates2 = coordinates.Above();
		if (RoadNodesContains(coordinates2))
		{
			return RegularConnectionKey(coordinates2, direction);
		}
		return PathMeshConnectionKeys.Nothing;
	}

	private byte RegularConnectionKey(Vector3Int coordinates, Vector3Int direction)
	{
		Vector3Int vector3Int = coordinates + direction;
		if (IsDoorstep(coordinates, vector3Int))
		{
			return PathMeshConnectionKeys.Building;
		}
		if (!IsConnectedToPathInArea(coordinates, vector3Int))
		{
			return PathMeshConnectionKeys.Nothing;
		}
		return PathMeshConnectionKeys.Path;
	}

	private bool IsDoorstep(Vector3Int entrance, Vector3Int inside)
	{
		foreach (BlockObject blockObject in GetBlockObjects(inside))
		{
			if (HasValidEntrance(blockObject, entrance, inside))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsTileVisible(Vector3Int coordinates)
	{
		if (!_blockService.GetPathObjectComponentAt<PathMeshHider>(coordinates))
		{
			return !_previewBlockService.GetPathObjectComponentAt<PathMeshHider>(coordinates);
		}
		return false;
	}

	private IEnumerable<BlockObject> GetBlockObjects(Vector3Int coordinates)
	{
		ReadOnlyList<BlockObject> objectsAt = _blockService.GetObjectsAt(coordinates);
		if (objectsAt.IsEmpty() && _drawingParameters.IsPreview)
		{
			return _previewBlockService.GetPreviewsAt(coordinates);
		}
		return objectsAt;
	}

	private bool HasValidEntrance(BlockObject entranceOwner, Vector3Int entrance, Vector3Int inside)
	{
		if ((bool)entranceOwner && (entranceOwner.IsFinished || _drawingParameters.IsPreview) && entranceOwner.HasEntrance && entranceOwner.PositionedEntrance.Coordinates == entrance && entranceOwner.PositionedEntrance.DoorstepCoordinates == inside)
		{
			return IsConnected(entrance, inside);
		}
		return false;
	}

	private bool IsConnected(Vector3Int coordinates, Vector3Int neighbor)
	{
		if (!_drawingParameters.IsPreview)
		{
			return _navMeshService.AreConnectedInstant(coordinates, neighbor);
		}
		return _navMeshService.AreConnectedPreview(coordinates, neighbor);
	}

	private bool IsConnectedToPathInArea(Vector3Int coordinates, Vector3Int neighbor)
	{
		if (IsConnectedToPath(coordinates, neighbor))
		{
			return RoadNodesContains(neighbor);
		}
		return false;
	}

	private bool IsConnectedToPath(Vector3Int coordinates, Vector3Int neighbor)
	{
		if (!_drawingParameters.IsPreview)
		{
			return _navMeshService.AreConnectedRoadInstant(coordinates, neighbor);
		}
		return _navMeshService.AreConnectedRoadPreview(coordinates, neighbor);
	}

	private bool RoadNodesContains(Vector3Int coordinates)
	{
		return _roadNodes.Contains(new WeightedCoordinates(coordinates, 0f));
	}
}
