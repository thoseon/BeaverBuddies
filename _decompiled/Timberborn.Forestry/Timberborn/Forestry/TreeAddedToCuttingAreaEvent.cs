namespace Timberborn.Forestry;

public class TreeAddedToCuttingAreaEvent
{
	public TreeComponent TreeComponent { get; }

	public TreeAddedToCuttingAreaEvent(TreeComponent treeComponent)
	{
		TreeComponent = treeComponent;
	}
}
