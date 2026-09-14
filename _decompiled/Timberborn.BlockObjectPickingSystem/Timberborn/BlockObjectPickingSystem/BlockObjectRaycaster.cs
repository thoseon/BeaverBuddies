using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.BlockObjectPickingSystem;

public class BlockObjectRaycaster
{
	private static readonly float ColliderInwardOffset = 0.001f;

	private readonly SelectableObjectRaycaster _selectableObjectRaycaster;

	public BlockObjectRaycaster(SelectableObjectRaycaster selectableObjectRaycaster)
	{
		_selectableObjectRaycaster = selectableObjectRaycaster;
	}

	public bool TryHitBlockObject<T>(Ray ray, out BlockObjectHit blockObjectHit)
	{
		Ray worldSpaceRay = CoordinateSystem.GridToWorld(ray);
		if (_selectableObjectRaycaster.TryHitSelectableObjectIncludeTerrainStump(worldSpaceRay, out var hitObject, out var raycastHit))
		{
			if (hitObject.GetComponent<T>() != null)
			{
				BlockObject component = hitObject.GetComponent<BlockObject>();
				if (component != null)
				{
					Block hitBlock = ClosestBlock(component, raycastHit);
					Vector3Int hitProjectedOnGround = new Vector3Int(hitBlock.Coordinates.x, hitBlock.Coordinates.y, Mathf.Min(component.CoordinatesAtBaseZ.z, hitBlock.Coordinates.z));
					blockObjectHit = new BlockObjectHit(component, hitBlock, hitProjectedOnGround, raycastHit.point);
					return true;
				}
			}
			GameObject gameObject = raycastHit.collider.gameObject;
			int layer = gameObject.layer;
			gameObject.layer = Layers.IgnoreRaycastMask;
			bool result = TryHitBlockObject<T>(ray, out blockObjectHit);
			gameObject.layer = layer;
			return result;
		}
		blockObjectHit = default(BlockObjectHit);
		return false;
	}

	private static Block ClosestBlock(BlockObject blockObject, RaycastHit hitInWorldSpace)
	{
		Vector3 point = hitInWorldSpace.point;
		Vector3 normal = hitInWorldSpace.normal;
		if (normal.x >= 0f && normal.y >= 0f && normal.z >= 0f)
		{
			if (IsEdgeHit(point.x))
			{
				point.x -= ColliderInwardOffset;
			}
			else if (IsEdgeHit(point.y))
			{
				point.y -= ColliderInwardOffset;
			}
			else if (IsEdgeHit(point.z))
			{
				point.z -= ColliderInwardOffset;
			}
		}
		Vector3 hitCoordinates = CoordinateSystem.WorldToGrid(point);
		return blockObject.PositionedBlocks.GetOccupiedBlocks().Aggregate((Block minItem, Block nextItem) => CloserCoordinates(hitCoordinates, minItem, nextItem));
	}

	private static bool IsEdgeHit(float coordinate)
	{
		return coordinate % 1f == 0f;
	}

	private static Block CloserCoordinates(Vector3 reference, Block block1, Block block2)
	{
		Vector3Int coordinates = block1.Coordinates;
		Vector3Int coordinates2 = block2.Coordinates;
		if (!(SquaredDistance(reference, coordinates) < SquaredDistance(reference, coordinates2)))
		{
			return block2;
		}
		return block1;
	}

	private static float SquaredDistance(Vector3 reference, Vector3Int block)
	{
		Vector3 vector = new Vector3((float)block.x + 0.5f, (float)block.y + 0.5f, (float)block.z + 0.5f);
		return (reference - vector).sqrMagnitude;
	}
}
