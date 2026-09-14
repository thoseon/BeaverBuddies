namespace Timberborn.BlockSystem;

public static class MatterBelowExtensions
{
	public static bool IsSolidMatter(this MatterBelow matterBelow)
	{
		if (matterBelow != MatterBelow.Ground && matterBelow != MatterBelow.GroundOrStackable)
		{
			return matterBelow == MatterBelow.Stackable;
		}
		return true;
	}
}
