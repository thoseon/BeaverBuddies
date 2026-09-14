using Timberborn.BaseComponentSystem;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class BlockObjectCenter : BaseComponent, IAwakableComponent
{
	private BlockObject _blockObject;

	public Vector3 GridCenter { get; private set; }

	public Vector3 WorldCenter { get; private set; }

	public Vector3 GridCenterGrounded { get; private set; }

	public Vector3 GridCenterAtBaseZ { get; private set; }

	public Vector3 WorldCenterGrounded { get; private set; }

	public Vector3 WorldCenterAtBaseZ { get; private set; }

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void UpdateCenter()
	{
		Vector3 vector = _blockObject.Blocks.Pivot(_blockObject.Coordinates, _blockObject.Orientation);
		Vector3 vector2 = _blockObject.GetComponent<BlockObjectSpec>().CalculateCenterOffset(_blockObject.Orientation);
		GridCenter = vector + vector2;
		WorldCenter = CoordinateSystem.GridToWorld(GridCenter);
		GridCenterGrounded = new Vector3(GridCenter.x, GridCenter.y, _blockObject.Coordinates.z);
		GridCenterAtBaseZ = GridCenterGrounded + new Vector3Int(0, 0, _blockObject.BaseZ);
		WorldCenterGrounded = CoordinateSystem.GridToWorld(GridCenterGrounded);
		WorldCenterAtBaseZ = new Vector3(WorldCenterGrounded.x, _blockObject.CoordinatesAtBaseZ.z, WorldCenterGrounded.z);
	}
}
