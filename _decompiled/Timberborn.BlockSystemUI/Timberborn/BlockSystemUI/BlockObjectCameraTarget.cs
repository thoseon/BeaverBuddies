using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.BlockSystemUI;

public class BlockObjectCameraTarget : BaseComponent, IAwakableComponent, ICameraTarget
{
	private BlockObjectCenter _blockObjectCenter;

	public Vector3 CameraTargetPosition => _blockObjectCenter.WorldCenterAtBaseZ;

	public void Awake()
	{
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
	}
}
