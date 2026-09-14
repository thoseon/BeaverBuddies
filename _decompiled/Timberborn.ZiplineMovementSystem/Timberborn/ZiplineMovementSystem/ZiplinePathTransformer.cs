using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using Timberborn.ZiplineSystem;
using UnityEngine;

namespace Timberborn.ZiplineMovementSystem;

internal class ZiplinePathTransformer : IPathTransformer, ILoadableSingleton
{
	private static readonly float TurnAngleThreshold = 45f;

	private static readonly Vector3 PathOffset = new Vector3(0f, -0.44f, 0f);

	private readonly IBlockService _blockService;

	private readonly ISpecService _specService;

	private readonly ZiplineGroupService _ziplineGroupService;

	private float _pathSpeed;

	public ZiplinePathTransformer(IBlockService blockService, ISpecService specService, ZiplineGroupService ziplineGroupService)
	{
		_blockService = blockService;
		_specService = specService;
		_ziplineGroupService = ziplineGroupService;
	}

	public void Load()
	{
		_pathSpeed = 1f / _specService.GetSingleSpec<ZiplineCableNavMeshSpec>().CableUnitCost;
	}

	public bool Transform(ref int index, ReadOnlyList<FlowFieldPathNode> flowFieldPath, List<PathCorner> pathCorners)
	{
		if (index < flowFieldPath.Count - 1)
		{
			FlowFieldPathNode flowFieldPathNode = flowFieldPath[index];
			FlowFieldPathNode flowFieldPathNode2 = flowFieldPath[index + 1];
			if (IsValidEdge(flowFieldPathNode, flowFieldPathNode2) && TryGetAnchors(flowFieldPathNode.Position, flowFieldPathNode2.Position, out var fromAnchor, out var toAnchor))
			{
				(Vector3, Vector3) tuple = ZiplineCalculator.CalculateWorldConnections(fromAnchor, toAnchor);
				Vector3 item = tuple.Item1;
				Vector3 item2 = tuple.Item2;
				int regularGroupId = _ziplineGroupService.RegularGroupId;
				bool num = flowFieldPathNode2.Cost <= 0f;
				float speed = (num ? float.MaxValue : _pathSpeed);
				pathCorners.Add(new PathCorner(item + PathOffset, _pathSpeed, regularGroupId));
				pathCorners.Add(new PathCorner(item2 + PathOffset, speed, regularGroupId));
				if (!num && index < flowFieldPath.Count - 2)
				{
					FlowFieldPathNode to = flowFieldPath[index + 2];
					AddTurn(pathCorners, flowFieldPathNode, flowFieldPathNode2, to, item2);
				}
				index++;
				return true;
			}
		}
		return false;
	}

	private bool TryGetAnchors(Vector3 from, Vector3 to, out Vector3 fromAnchor, out Vector3 toAnchor)
	{
		Vector3Int coordinates = CoordinateSystem.WorldToGridInt(from);
		Vector3Int coordinates2 = CoordinateSystem.WorldToGridInt(to);
		ZiplineTower bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<ZiplineTower>(coordinates);
		ZiplineTower bottomObjectComponentAt2 = _blockService.GetBottomObjectComponentAt<ZiplineTower>(coordinates2);
		if (bottomObjectComponentAt == null || bottomObjectComponentAt2 == null)
		{
			fromAnchor = Vector3.zero;
			toAnchor = Vector3.zero;
			return false;
		}
		fromAnchor = CoordinateSystem.GridToWorld(bottomObjectComponentAt.CableAnchorPoint);
		toAnchor = CoordinateSystem.GridToWorld(bottomObjectComponentAt2.CableAnchorPoint);
		return true;
	}

	private bool IsValidEdge(FlowFieldPathNode from, FlowFieldPathNode to)
	{
		if (from.Cost > 0f)
		{
			return _ziplineGroupService.IsRegularEdge(from.GroupId, to.GroupId);
		}
		return false;
	}

	private void AddTurn(List<PathCorner> pathCorners, FlowFieldPathNode from, FlowFieldPathNode through, FlowFieldPathNode to, Vector3 throughConnection)
	{
		if (CanAddTurn(from, through, to) && TryGetAnchors(through.Position, to.Position, out var fromAnchor, out var toAnchor))
		{
			Vector3 vector = ZiplineCalculator.CalculateTurn(fromAnchor, toAnchor, throughConnection);
			pathCorners.Add(new PathCorner(vector + PathOffset, _pathSpeed, _ziplineGroupService.TurnGroupId));
		}
	}

	private bool CanAddTurn(FlowFieldPathNode from, FlowFieldPathNode through, FlowFieldPathNode to)
	{
		if (IsValidEdge(through, to))
		{
			Vector3 normalized = (through.Position - from.Position).normalized;
			Vector3 normalized2 = (to.Position - through.Position).normalized;
			return Vector3.SignedAngle(normalized, normalized2, Vector3.up) < TurnAngleThreshold;
		}
		return false;
	}
}
