using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.CameraSystem;
using Timberborn.GridTraversing;
using Timberborn.InputSystem;
using Timberborn.SelectionSystem;
using Timberborn.TerrainQueryingSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.CursorToolSystem;

public class CursorCoordinatesPicker
{
	private readonly CameraService _cameraService;

	private readonly TerrainPicker _terrainPicker;

	private readonly InputService _inputService;

	private readonly SelectableObjectRaycaster _selectableObjectRaycaster;

	private readonly ITerrainService _terrainService;

	private readonly List<Vector3Int> _stackableCoordinatesCache = new List<Vector3Int>();

	public CursorCoordinatesPicker(CameraService cameraService, TerrainPicker terrainPicker, InputService inputService, SelectableObjectRaycaster selectableObjectRaycaster, ITerrainService terrainService)
	{
		_cameraService = cameraService;
		_terrainPicker = terrainPicker;
		_inputService = inputService;
		_selectableObjectRaycaster = selectableObjectRaycaster;
		_terrainService = terrainService;
	}

	public CursorCoordinates? Pick()
	{
		return PickCoordinates(ignoreUnfinished: false);
	}

	public CursorCoordinates? PickOnFinished()
	{
		return PickCoordinates(ignoreUnfinished: true);
	}

	private CursorCoordinates? PickCoordinates(bool ignoreUnfinished)
	{
		Ray ray = _cameraService.ScreenPointToRayInGridSpace(_inputService.MousePosition);
		if (TryGetBlockObjectCoordinates(ray, ignoreUnfinished, out var foundCoordinates))
		{
			return foundCoordinates;
		}
		TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinates(ray);
		if (traversedCoordinates.HasValue)
		{
			TraversedCoordinates valueOrDefault = traversedCoordinates.GetValueOrDefault();
			Vector3Int vector3Int = valueOrDefault.Coordinates + valueOrDefault.Face;
			if (_terrainService.Contains(vector3Int))
			{
				return new CursorCoordinates(valueOrDefault.Intersection, vector3Int);
			}
		}
		return null;
	}

	private bool TryGetBlockObjectCoordinates(Ray ray, bool ignoreUnfinished, out CursorCoordinates? foundCoordinates)
	{
		if (_selectableObjectRaycaster.TryHitSelectableObject(out var hitObject))
		{
			BlockObject component = hitObject.GetComponent<BlockObject>();
			if (component != null && !component.IsPreview && (!ignoreUnfinished || component.IsFinished))
			{
				foundCoordinates = GetFloorOrPathCoordinatesHit(ray, component) ?? GetStackableCoordinatesHit(ray, component);
				return foundCoordinates.HasValue;
			}
		}
		foundCoordinates = null;
		return false;
	}

	private static CursorCoordinates? GetFloorOrPathCoordinatesHit(Ray ray, BlockObject blockObject)
	{
		if (blockObject.PositionedBlocks.GetAllBlocks().Any(delegate(Block block)
		{
			BlockOccupations occupation = block.Occupation;
			return (occupation == BlockOccupations.Floor || occupation == BlockOccupations.Path) ? true : false;
		}))
		{
			Vector3? vector = GridSpaceRaycasting.HitHorizontalPlane(ray, blockObject.Coordinates.z);
			if (vector.HasValue)
			{
				return new CursorCoordinates(vector.Value, GetTile(vector.Value));
			}
		}
		return null;
	}

	private CursorCoordinates? GetStackableCoordinatesHit(Ray ray, BlockObject blockObject)
	{
		AddStackableCoordinates(blockObject);
		if (_stackableCoordinatesCache.Count == 0)
		{
			return null;
		}
		int num = _stackableCoordinatesCache.Max((Vector3Int coords) => coords.z);
		Vector3? vector = GridSpaceRaycasting.HitHorizontalPlane(ray, num + 1);
		if (vector.HasValue)
		{
			Vector3Int tile = GetTile(vector.Value);
			if (_stackableCoordinatesCache.Contains(tile - new Vector3Int(0, 0, 1)))
			{
				ClearStackableCoordinates();
				return new CursorCoordinates(vector.Value, tile);
			}
		}
		ClearStackableCoordinates();
		return null;
	}

	private static Vector3Int GetTile(Vector3 intersection)
	{
		return new Vector3Int(Mathf.FloorToInt(intersection.x), Mathf.FloorToInt(intersection.y), Mathf.RoundToInt(intersection.z));
	}

	private void AddStackableCoordinates(BlockObject blockObject)
	{
		IEnumerable<Vector3Int> collection = from block in blockObject.PositionedBlocks.GetAllBlocks()
			where block.Stackable != BlockStackable.None
			select block.Coordinates;
		_stackableCoordinatesCache.AddRange(collection);
	}

	private void ClearStackableCoordinates()
	{
		_stackableCoordinatesCache.Clear();
	}
}
