using System.Collections.Generic;
using Timberborn.Coordinates;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainSystemRendering;

public class TerrainHighlightingService : ILoadableSingleton, ILateUpdatableSingleton
{
	private static readonly Vector3 StumpMarkerOffset = Vector3.up * TerrainMeshManager.TerrainStumpHeight;

	private static readonly Vector3 StumpMarkerSize = Vector3.one * 0.9f;

	private readonly MarkerDrawerFactory _meshDrawerFactory;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly ITerrainService _terrainService;

	private readonly List<Vector3Int> _coordinatesToHighlight = new List<Vector3Int>();

	private readonly List<Matrix4x4> _highlightedCoords = new List<Matrix4x4>();

	private readonly List<Matrix4x4> _topHighlightedCoords = new List<Matrix4x4>();

	private MeshDrawer _markerDrawer;

	private MeshDrawer _topMarkerDrawer;

	public TerrainHighlightingService(MarkerDrawerFactory meshDrawerFactory, ILevelVisibilityService levelVisibilityService, ITerrainService terrainService)
	{
		_meshDrawerFactory = meshDrawerFactory;
		_levelVisibilityService = levelVisibilityService;
		_terrainService = terrainService;
	}

	public void Load()
	{
		_markerDrawer = _meshDrawerFactory.CreateTerrainTileDrawer();
		_topMarkerDrawer = _meshDrawerFactory.CreateTopTerrainTileDrawer();
		_levelVisibilityService.MaxVisibleLevelChanged += OnMaxVisibleLevelChanged;
	}

	public void UpdateHighlight(IEnumerable<Vector3Int> highlightedTerrain)
	{
		ClearHighlight();
		_coordinatesToHighlight.AddRange(highlightedTerrain);
		UpdateHighlightMatrices();
	}

	public void ClearHighlight()
	{
		_coordinatesToHighlight.Clear();
		_highlightedCoords.Clear();
		_topHighlightedCoords.Clear();
	}

	public void LateUpdateSingleton()
	{
		if (_highlightedCoords.Count > 0)
		{
			_markerDrawer.DrawMultipleInstanced(_highlightedCoords);
			_topMarkerDrawer.DrawMultipleInstanced(_topHighlightedCoords);
		}
	}

	private void OnMaxVisibleLevelChanged(object sender, int maxVisibleLevel)
	{
		_topHighlightedCoords.Clear();
		if (_levelVisibilityService.TerrainLevelIsAtMax)
		{
			return;
		}
		foreach (Vector3Int item in _coordinatesToHighlight)
		{
			Vector3 position = CoordinateSystem.GridToWorldCentered(item);
			AddTerrainStumpMarker(position);
		}
	}

	private void UpdateHighlightMatrices()
	{
		foreach (Vector3Int item2 in _coordinatesToHighlight)
		{
			Vector3 vector = CoordinateSystem.GridToWorldCentered(item2);
			if (_terrainService.IsVisible(item2))
			{
				Matrix4x4 item = Matrix4x4.TRS(vector, Quaternion.identity, Vector3.one);
				_highlightedCoords.Add(item);
			}
			AddTerrainStumpMarker(vector);
		}
	}

	private void AddTerrainStumpMarker(Vector3 position)
	{
		float y = position.y;
		if (y >= (float)_levelVisibilityService.MaxVisibleLevel && y < (float)(_levelVisibilityService.MaxVisibleLevel + 1))
		{
			_topHighlightedCoords.Add(Matrix4x4.TRS(position + StumpMarkerOffset, Quaternion.identity, StumpMarkerSize));
		}
	}
}
