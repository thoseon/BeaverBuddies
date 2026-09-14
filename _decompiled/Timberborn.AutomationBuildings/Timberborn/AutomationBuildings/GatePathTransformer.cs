using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

internal class GatePathTransformer : IPathTransformer, ILoadableSingleton
{
	private readonly IBlockService _blockService;

	private readonly NavMeshGroupService _navMeshGroupService;

	private int _validGroupId;

	public GatePathTransformer(IBlockService blockService, NavMeshGroupService navMeshGroupService)
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
		FlowFieldPathNode flowFieldPathNode = flowFieldPath[index];
		if (flowFieldPathNode.GroupId == _validGroupId)
		{
			if (TryAdjustSpeedOnGate(ref index, pathCorners, flowFieldPathNode, flowFieldPathNode))
			{
				return true;
			}
			if (index < flowFieldPath.Count - 1)
			{
				FlowFieldPathNode nodePosition = flowFieldPath[index + 1];
				if (TryAdjustSpeedOnGate(ref index, pathCorners, flowFieldPathNode, nodePosition))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool TryAdjustSpeedOnGate(ref int index, List<PathCorner> pathCorners, FlowFieldPathNode referenceNode, FlowFieldPathNode nodePosition)
	{
		Vector3Int coordinates = CoordinateSystem.WorldToGridInt(nodePosition.Position);
		if ((bool)_blockService.GetPathObjectComponentAt<Gate>(coordinates))
		{
			pathCorners.Add(new PathCorner(referenceNode.Position, 1f, _validGroupId));
			index++;
			return true;
		}
		return false;
	}
}
