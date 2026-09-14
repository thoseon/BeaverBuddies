using Timberborn.CameraSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.GridTraversing;
using Timberborn.InputSystem;
using Timberborn.TerrainQueryingSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.SelectionSystem;

public class SelectableObjectRaycaster
{
	private readonly TerrainPicker _terrainPicker;

	private readonly CameraService _cameraService;

	private readonly InputService _inputService;

	private readonly SelectableObjectRetriever _selectableObjectRetriever;

	private readonly ITerrainService _terrainService;

	public SelectableObjectRaycaster(TerrainPicker terrainPicker, CameraService cameraService, InputService inputService, SelectableObjectRetriever selectableObjectRetriever, ITerrainService terrainService)
	{
		_terrainPicker = terrainPicker;
		_cameraService = cameraService;
		_inputService = inputService;
		_selectableObjectRetriever = selectableObjectRetriever;
		_terrainService = terrainService;
	}

	public bool TryHitSelectableObjectIncludeTerrainStump(Ray worldSpaceRay, out SelectableObject hitObject, out RaycastHit raycastHit)
	{
		return TryHitSelectableObject(worldSpaceRay, includeTerrainStump: true, out hitObject, out raycastHit);
	}

	public bool TryHitSelectableObjectIncludeTerrainStump(out SelectableObject hitObject)
	{
		Ray worldSpaceRay = _cameraService.ScreenPointToRayInWorldSpace(_inputService.MousePosition);
		RaycastHit raycastHit;
		return TryHitSelectableObject(worldSpaceRay, includeTerrainStump: true, out hitObject, out raycastHit);
	}

	public bool TryHitSelectableObject(out SelectableObject hitObject)
	{
		Ray worldSpaceRay = _cameraService.ScreenPointToRayInWorldSpace(_inputService.MousePosition);
		RaycastHit raycastHit;
		return TryHitSelectableObject(worldSpaceRay, includeTerrainStump: false, out hitObject, out raycastHit);
	}

	private bool TryHitSelectableObject(Ray worldSpaceRay, bool includeTerrainStump, out SelectableObject hitObject, out RaycastHit raycastHit)
	{
		if (Physics.Raycast(worldSpaceRay, out raycastHit) && HitIsCloserThanTerrain(worldSpaceRay, includeTerrainStump, raycastHit))
		{
			GameObject gameObject = raycastHit.collider.gameObject;
			if ((bool)gameObject && _selectableObjectRetriever.TryGetSelectableObject(gameObject, out hitObject))
			{
				return true;
			}
		}
		hitObject = null;
		return false;
	}

	private bool HitIsCloserThanTerrain(Ray ray, bool includeTerrainStump, RaycastHit hit)
	{
		if (HitTerrain(ray, includeTerrainStump, out var distance))
		{
			return hit.distance < distance;
		}
		return true;
	}

	private bool HitTerrain(Ray ray, bool includeTerrainStump, out float distance)
	{
		Ray gridRay = CoordinateSystem.WorldToGrid(ray);
		TraversedCoordinates? traversedCoordinates = PickTerrainCoordinates(gridRay, includeTerrainStump);
		if (traversedCoordinates.HasValue)
		{
			TraversedCoordinates valueOrDefault = traversedCoordinates.GetValueOrDefault();
			distance = Vector3.Distance(gridRay.origin, valueOrDefault.Intersection);
			return !WasCutoutHit(valueOrDefault);
		}
		distance = 0f;
		return false;
	}

	private TraversedCoordinates? PickTerrainCoordinates(Ray gridRay, bool includeTerrainStump)
	{
		if (!includeTerrainStump)
		{
			return _terrainPicker.PickTerrainCoordinates(gridRay);
		}
		return _terrainPicker.PickTerrainCoordinatesWithStump(gridRay);
	}

	private bool WasCutoutHit(TraversedCoordinates terrainCoordinates)
	{
		if (terrainCoordinates.Face.z == 1)
		{
			return _terrainService.CellIsCutout(terrainCoordinates.Coordinates.Above());
		}
		return false;
	}
}
