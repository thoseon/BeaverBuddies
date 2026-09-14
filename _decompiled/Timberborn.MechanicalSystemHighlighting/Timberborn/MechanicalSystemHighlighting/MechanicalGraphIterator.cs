using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.MechanicalSystem;
using UnityEngine;

namespace Timberborn.MechanicalSystemHighlighting;

internal class MechanicalGraphIterator
{
	private readonly struct GraphNode
	{
		public TransputSpec Transput { get; }

		public BlockObject Parent { get; }

		public GraphNode(TransputSpec transput, BlockObject parent)
		{
			Transput = transput;
			Parent = parent;
		}
	}

	private readonly IBlockService _blockService;

	private readonly Stack<GraphNode> _graphStack = new Stack<GraphNode>();

	public MechanicalGraphIterator(IBlockService blockService)
	{
		_blockService = blockService;
	}

	public void Iterate(IEnumerable<MechanicalNode> rootNodes, ICollection<MechanicalNode> graphNodes, bool includeUnfinished)
	{
		InitializeStackFromRootNodes(rootNodes, graphNodes);
		while (_graphStack.Count > 0)
		{
			GraphNode graphNode = _graphStack.Pop();
			foreach (Direction3D item in graphNode.Transput.Directions.GetEnumerator())
			{
				MechanicalNode neighborNode = GetNeighborNode(graphNode, item);
				if (!neighborNode || graphNodes.Contains(neighborNode))
				{
					continue;
				}
				BlockObject component = neighborNode.GetComponent<BlockObject>();
				if (includeUnfinished || component.IsFinished)
				{
					graphNodes.Add(neighborNode);
					foreach (TransputSpec transput in neighborNode.GetComponent<TransputProviderSpec>().Transputs)
					{
						_graphStack.Push(new GraphNode(transput, component));
					}
				}
			}
		}
		_graphStack.Clear();
	}

	private void InitializeStackFromRootNodes(IEnumerable<MechanicalNode> rootNodes, ICollection<MechanicalNode> graphNodes)
	{
		foreach (MechanicalNode rootNode in rootNodes)
		{
			if ((bool)rootNode && !rootNode.IsDetached)
			{
				graphNodes.Add(rootNode);
				TransputProviderSpec component = rootNode.GetComponent<TransputProviderSpec>();
				BlockObject component2 = rootNode.GetComponent<BlockObject>();
				foreach (TransputSpec transput in component.Transputs)
				{
					_graphStack.Push(new GraphNode(transput, component2));
				}
			}
		}
	}

	private MechanicalNode GetNeighborNode(GraphNode graphNode, Direction3D direction)
	{
		Vector3Int vector3Int = graphNode.Parent.TransformCoordinates(graphNode.Transput.Coordinates);
		Direction3D direction3D = graphNode.Parent.TransformDirection(direction);
		Vector3Int coordinates = vector3Int + direction3D.ToOffset();
		MechanicalNode firstObjectWithComponentAt = _blockService.GetFirstObjectWithComponentAt<MechanicalNode>(coordinates);
		if (!IsValidNeighborNode(firstObjectWithComponentAt, vector3Int, direction3D))
		{
			return null;
		}
		return firstObjectWithComponentAt;
	}

	private static bool IsValidNeighborNode(MechanicalNode node, Vector3Int startCoordinates, Direction3D startDirection)
	{
		if ((bool)node && !node.IsDetached)
		{
			TransputProviderSpec component = node.GetComponent<TransputProviderSpec>();
			BlockObject component2 = node.GetComponent<BlockObject>();
			foreach (TransputSpec transput in component.Transputs)
			{
				foreach (Direction3D item in transput.Directions.GetEnumerator())
				{
					Direction3D direction3D = component2.TransformDirection(item);
					if (component2.TransformCoordinates(transput.Coordinates) + direction3D.ToOffset() == startCoordinates && direction3D == startDirection.Across())
					{
						return true;
					}
				}
			}
		}
		return false;
	}
}
