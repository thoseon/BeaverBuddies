using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;

namespace Timberborn.Navigation;

internal class FlowFieldPathTransformer
{
	private static readonly float DefaultSpeed = 1f;

	private readonly ImmutableArray<IPathTransformer> _pathTransformers;

	public FlowFieldPathTransformer(IEnumerable<IPathTransformer> pathTransformers)
	{
		_pathTransformers = pathTransformers.ToImmutableArray();
	}

	public void TransformPath(List<FlowFieldPathNode> flowFieldPath, List<PathCorner> pathCorners)
	{
		pathCorners.Clear();
		if (flowFieldPath.Count == 1)
		{
			FlowFieldPathNode flowFieldPathNode = flowFieldPath[0];
			pathCorners.Add(new PathCorner(flowFieldPathNode.Position, DefaultSpeed, flowFieldPathNode.GroupId));
			return;
		}
		ReadOnlyList<FlowFieldPathNode> flowFieldPath2 = flowFieldPath.AsReadOnlyList();
		int index = 0;
		while (index < flowFieldPath2.Count)
		{
			if (!TransformNode(ref index, flowFieldPath2, pathCorners))
			{
				AddPathCorner(flowFieldPath2[index], pathCorners);
				index++;
			}
		}
	}

	public void TransformReversedPath(List<FlowFieldPathNode> flowFieldPath, List<PathCorner> pathCorners)
	{
		ReversePath(flowFieldPath);
		TransformPath(flowFieldPath, pathCorners);
	}

	private bool TransformNode(ref int index, ReadOnlyList<FlowFieldPathNode> flowFieldPath, List<PathCorner> pathCorners)
	{
		foreach (IPathTransformer pathTransformer in _pathTransformers)
		{
			if (pathTransformer.Transform(ref index, flowFieldPath, pathCorners))
			{
				return true;
			}
		}
		return false;
	}

	private void AddPathCorner(FlowFieldPathNode flowFieldPathNode, List<PathCorner> pathCorners)
	{
		pathCorners.Add(new PathCorner(flowFieldPathNode.Position, flowFieldPathNode.NormalizedSpeed, flowFieldPathNode.GroupId));
	}

	private static void ReversePath(List<FlowFieldPathNode> rawPath)
	{
		rawPath.Reverse();
		for (int i = 0; i < rawPath.Count - 1; i++)
		{
			FlowFieldPathNode flowFieldPathNode = rawPath[i + 1];
			rawPath[i] = new FlowFieldPathNode(rawPath[i].Position, flowFieldPathNode.Cost, flowFieldPathNode.DistanceToNext, flowFieldPathNode.GroupId);
		}
	}
}
