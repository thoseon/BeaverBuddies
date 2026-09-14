using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.PathSystem;

internal class SpiralStairsHeightProvider : BaseComponent, IAwakableComponent, IPathHeightProvider
{
	private static readonly float MinHeight = 0.075f;

	private static readonly float MaxAngle = MathF.PI / 2f;

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
			float num = MaxAngle - Mathf.Atan2(vector.x, 1f - vector.z);
			pathHeight = (float)_blockObject.Coordinates.z + Mathf.Clamp01(MinHeight + num / MaxAngle);
			return true;
		}
		pathHeight = 0f;
		return false;
	}
}
