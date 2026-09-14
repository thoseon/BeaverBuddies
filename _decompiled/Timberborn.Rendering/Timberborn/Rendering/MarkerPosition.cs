using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.Rendering;

public class MarkerPosition : BaseComponent, IAwakableComponent
{
	private static readonly float YOffset = 0.75f;

	private readonly BoundsCalculator _boundsCalculator;

	private BlockObjectCenter _blockObjectCenter;

	public Vector3 Position { get; private set; }

	public MarkerPosition(BoundsCalculator boundsCalculator)
	{
		_boundsCalculator = boundsCalculator;
	}

	public void Awake()
	{
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
	}

	public void UpdatePosition(Vector3 gridOffset = default(Vector3))
	{
		float enabledRendererYMaxBound = _boundsCalculator.GetEnabledRendererYMaxBound(base.Transform);
		float num = ((gridOffset.z > 0f) ? gridOffset.z : YOffset);
		float y = enabledRendererYMaxBound + num;
		Vector3 vector = CoordinateSystem.GridToWorld(_blockObjectCenter.GridCenter + gridOffset);
		Position = new Vector3(vector.x, y, vector.z);
	}
}
