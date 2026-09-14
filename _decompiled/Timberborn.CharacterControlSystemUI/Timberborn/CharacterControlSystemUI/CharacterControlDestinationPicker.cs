using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CameraSystem;
using Timberborn.InputSystem;
using Timberborn.SelectionSystem;
using Timberborn.TerrainQueryingSystem;
using UnityEngine;

namespace Timberborn.CharacterControlSystemUI;

public class CharacterControlDestinationPicker
{
	private readonly SelectableObjectRaycaster _selectableObjectRaycaster;

	private readonly TerrainPicker _terrainPicker;

	private readonly CameraService _cameraService;

	private readonly InputService _inputService;

	public CharacterControlDestinationPicker(SelectableObjectRaycaster selectableObjectRaycaster, TerrainPicker terrainPicker, CameraService cameraService, InputService inputService)
	{
		_selectableObjectRaycaster = selectableObjectRaycaster;
		_terrainPicker = terrainPicker;
		_cameraService = cameraService;
		_inputService = inputService;
	}

	public Vector3? PickDestination()
	{
		Ray ray = _cameraService.ScreenPointToRayInGridSpace(_inputService.MousePosition);
		if (_selectableObjectRaycaster.TryHitSelectableObject(out var hitObject))
		{
			Vector3? vector = PickDestination(hitObject);
			if (vector.HasValue)
			{
				return vector.GetValueOrDefault();
			}
		}
		return _terrainPicker.PickTerrainCoordinates(ray)?.Intersection;
	}

	private static Vector3? PickDestination(BaseComponent hitObject)
	{
		BlockObject component = hitObject.GetComponent<BlockObject>();
		if (component != null)
		{
			ImmutableArray<Block> allBlocks = component.PositionedBlocks.GetAllBlocks();
			if (allBlocks.Any((Block block) => block.Stackable != BlockStackable.None))
			{
				int num = allBlocks.Max((Block block) => block.Coordinates.z);
				Vector3 gridCenterGrounded = component.GetComponent<BlockObjectCenter>().GridCenterGrounded;
				return new Vector3(gridCenterGrounded.x, gridCenterGrounded.y, num + 1);
			}
			if (component.HasEntrance)
			{
				return component.PositionedEntrance.DoorstepCoordinates;
			}
		}
		return null;
	}
}
