using Timberborn.BlockObjectPickingSystem;
using Timberborn.BlockSystem;
using Timberborn.GridTraversing;
using Timberborn.TerrainQueryingSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.AreaSelectionSystem;

public class AreaSelector
{
	private readonly BlockObjectRaycaster _blockObjectRaycaster;

	private readonly TerrainPicker _terrainPicker;

	private readonly AreaClamper _areaClamper;

	private readonly ITerrainService _terrainService;

	public AreaSelector(BlockObjectRaycaster blockObjectRaycaster, TerrainPicker terrainPicker, AreaClamper areaClamper, ITerrainService terrainService)
	{
		_blockObjectRaycaster = blockObjectRaycaster;
		_terrainPicker = terrainPicker;
		_areaClamper = areaClamper;
		_terrainService = terrainService;
	}

	public SelectionStart? GetSelectionStart<T>(Ray ray)
	{
		if (_blockObjectRaycaster.TryHitBlockObject<T>(ray, out var blockObjectHit))
		{
			return new SelectionStart(blockObjectHit);
		}
		TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinates(ray);
		if (traversedCoordinates.HasValue)
		{
			return new SelectionStart(traversedCoordinates.Value.Coordinates + traversedCoordinates.Value.Face);
		}
		return null;
	}

	public Vector3Int GetSelectionEnd(SelectionStart selectionStart, Ray endRay)
	{
		Vector3Int coordinates = selectionStart.Coordinates;
		Vector3Int endCoords = ProjectEndOnStartLevel(_terrainPicker.FindCoordinatesOnLevelInMap(endRay, selectionStart.HitLevel), coordinates);
		endCoords.z += selectionStart.VerticalOffset;
		return ClampSelectionEnd(coordinates, endCoords);
	}

	private static Vector3Int ProjectEndOnStartLevel(TraversedCoordinates? traversedCoordinates, Vector3Int startCoords)
	{
		if (traversedCoordinates.HasValue)
		{
			Vector3Int coordinates = traversedCoordinates.Value.Coordinates;
			coordinates.z = startCoords.z;
			return coordinates;
		}
		return startCoords;
	}

	private Vector3Int ClampSelectionEnd(Vector3Int startCoords, Vector3Int endCoords)
	{
		Vector3Int vector3Int = _areaClamper.ClampEnd(startCoords, endCoords, 30);
		if (endCoords != vector3Int)
		{
			int terrainHeight = _terrainService.GetTerrainHeight(vector3Int);
			vector3Int.z = Mathf.Max(startCoords.z, terrainHeight);
		}
		vector3Int.z = Mathf.Min(startCoords.z, vector3Int.z);
		return vector3Int;
	}
}
