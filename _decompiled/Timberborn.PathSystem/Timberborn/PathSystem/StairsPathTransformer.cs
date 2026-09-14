using System;
using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.PathSystem;

internal class StairsPathTransformer : IPathTransformer, ILoadableSingleton
{
	private readonly IBlockService _blockService;

	private readonly NavMeshGroupService _navMeshGroupService;

	private int _validGroupId;

	public StairsPathTransformer(IBlockService blockService, NavMeshGroupService navMeshGroupService)
	{
		_blockService = blockService;
		_navMeshGroupService = navMeshGroupService;
	}

	public void Load()
	{
		_validGroupId = _navMeshGroupService.GetDefaultGroupId();
	}

	public bool Transform(ref int index, ReadOnlyList<FlowFieldPathNode> flowFieldPath, List<PathCorner> pathCorners)
	{
		if (flowFieldPath[index].GroupId == _validGroupId)
		{
			if (index <= 0 || index >= flowFieldPath.Count - 1)
			{
				return AdjustPathEnd(ref index, flowFieldPath, pathCorners);
			}
			return ApplyStairsPath(ref index, flowFieldPath, pathCorners);
		}
		return false;
	}

	private bool ApplyStairsPath(ref int index, ReadOnlyList<FlowFieldPathNode> flowFieldPath, List<PathCorner> pathCorners)
	{
		FlowFieldPathNode flowFieldPathNode = flowFieldPath[index - 1];
		FlowFieldPathNode flowFieldPathNode2 = flowFieldPath[index];
		FlowFieldPathNode flowFieldPathNode3 = flowFieldPath[index + 1];
		Vector3 offset;
		Vector3 vector;
		if (flowFieldPathNode3.GroupId == _validGroupId && IsVerticalEdge(flowFieldPathNode2, flowFieldPathNode3))
		{
			offset = GetOffset(flowFieldPathNode, flowFieldPathNode2);
			vector = flowFieldPathNode2.Position - offset;
			if (pathCorners.Count != 0)
			{
				if (!(pathCorners[pathCorners.Count - 1].Position != vector))
				{
					goto IL_0097;
				}
			}
			pathCorners.Add(new PathCorner(vector, 1f, _validGroupId));
			goto IL_0097;
		}
		return false;
		IL_0097:
		bool flag = index < flowFieldPath.Count - 2;
		if (flag)
		{
			Vector3 vector2 = 0.5f * (flowFieldPathNode2.Position + flowFieldPathNode3.Position);
			Vector3Int coordinates = CoordinateSystem.WorldToGridInt(vector2);
			if (_blockService.GetPathObjectComponentAt<SpiralStairsSpec>(coordinates) != null)
			{
				ApplySpiralStairsPath(vector2, vector, flowFieldPathNode3, flowFieldPath[index + 2], pathCorners);
			}
			else
			{
				pathCorners.Add(new PathCorner(flowFieldPathNode3.Position + offset, 1f, _validGroupId));
			}
		}
		index += ((!flag) ? 1 : 2);
		return true;
	}

	private static bool IsVerticalEdge(FlowFieldPathNode from, FlowFieldPathNode to)
	{
		return Math.Abs(from.Position.y - to.Position.y) > 0.5f;
	}

	private static Vector3 GetOffset(FlowFieldPathNode from, FlowFieldPathNode to)
	{
		Vector3 vector = to.Position - from.Position;
		vector.y = 0f;
		return vector.normalized * 0.5f;
	}

	private void ApplySpiralStairsPath(Vector3 center, Vector3 startPosition, FlowFieldPathNode outboundEdgeStart, FlowFieldPathNode outboundEdgeEnd, List<PathCorner> pathCorners)
	{
		Vector3 vector = outboundEdgeStart.Position + GetOffset(outboundEdgeStart, outboundEdgeEnd);
		Vector3 normalized = (startPosition - center).normalized;
		Vector3 normalized2 = (vector - center).normalized;
		Vector3 position = center + normalized * 0.275f + normalized2 * 0.125f;
		Vector3 position2 = center + normalized2 * 0.275f + normalized * 0.125f;
		pathCorners.Add(new PathCorner(position, 1f, _validGroupId));
		pathCorners.Add(new PathCorner(position2, 1f, _validGroupId));
		pathCorners.Add(new PathCorner(vector, 1f, _validGroupId));
	}

	private bool AdjustPathEnd(ref int index, ReadOnlyList<FlowFieldPathNode> flowFieldPath, List<PathCorner> pathCorners)
	{
		FlowFieldPathNode flowFieldPathNode = flowFieldPath[index];
		Vector3 position = flowFieldPathNode.Position;
		Vector3Int coordinates = CoordinateSystem.WorldToGridInt(position);
		IPathHeightProvider bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<IPathHeightProvider>(coordinates);
		if (bottomObjectComponentAt != null && bottomObjectComponentAt.TryGetHeight(position, out var pathHeight))
		{
			position.y = pathHeight;
			pathCorners.Add(new PathCorner(position, flowFieldPathNode.NormalizedSpeed, _validGroupId));
			index++;
			return true;
		}
		return false;
	}
}
