using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Navigation;

public interface IPathTransformer
{
	bool Transform(ref int index, ReadOnlyList<FlowFieldPathNode> flowFieldPath, List<PathCorner> pathCorners);
}
