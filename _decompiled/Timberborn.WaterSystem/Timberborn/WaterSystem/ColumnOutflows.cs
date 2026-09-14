using System.Collections.Generic;

namespace Timberborn.WaterSystem;

internal struct ColumnOutflows
{
	public TargetedFlow BottomFlow;

	public TargetedFlow LeftFlow;

	public TargetedFlow TopFlow;

	public TargetedFlow RightFlow;

	public List<TargetedFlow> Outflows;
}
