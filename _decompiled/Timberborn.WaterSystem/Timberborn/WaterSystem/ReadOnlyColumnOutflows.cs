using System.Collections.Generic;

namespace Timberborn.WaterSystem;

public readonly struct ReadOnlyColumnOutflows
{
	public TargetedFlow BottomFlow { get; }

	public TargetedFlow LeftFlow { get; }

	public TargetedFlow TopFlow { get; }

	public TargetedFlow RightFlow { get; }

	public List<TargetedFlow> Outflows { get; }

	public ReadOnlyColumnOutflows(TargetedFlow bottomFlow, TargetedFlow leftFlow, TargetedFlow topFlow, TargetedFlow rightFlow, List<TargetedFlow> outflows)
	{
		BottomFlow = bottomFlow;
		LeftFlow = leftFlow;
		TopFlow = topFlow;
		RightFlow = rightFlow;
		Outflows = outflows;
	}
}
