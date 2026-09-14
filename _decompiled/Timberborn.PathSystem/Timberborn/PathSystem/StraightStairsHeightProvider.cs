using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.PathSystem;

internal class StraightStairsHeightProvider : BaseComponent, IAwakableComponent, IPathHeightProvider
{
	private static readonly float LinearCoefficient = -1f;

	private static readonly float LinearConstant = 1f;

	private BlockObject _blockObject;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public bool TryGetHeight(Vector3 worldPosition, out float pathHeight)
	{
		if (_blockObject.IsFinished)
		{
			Vector3 vector = base.Transform.InverseTransformPoint(worldPosition);
			pathHeight = (float)_blockObject.Coordinates.z + (LinearCoefficient * vector.z + LinearConstant);
			return true;
		}
		pathHeight = 0f;
		return false;
	}
}
