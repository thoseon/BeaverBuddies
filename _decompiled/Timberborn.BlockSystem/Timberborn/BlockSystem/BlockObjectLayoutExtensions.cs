using System;

namespace Timberborn.BlockSystem;

public static class BlockObjectLayoutExtensions
{
	public static int GetPreviewCount(this BlockObjectLayout blockObjectLayout)
	{
		return blockObjectLayout switch
		{
			BlockObjectLayout.Single => 1, 
			BlockObjectLayout.Rectangle => 100, 
			BlockObjectLayout.Line => 25, 
			BlockObjectLayout.Half => 2, 
			BlockObjectLayout.SideLine => 25, 
			BlockObjectLayout.TwoSegmentLine => 40, 
			_ => throw new ArgumentOutOfRangeException("blockObjectLayout", blockObjectLayout, null), 
		};
	}

	public static bool ShouldShowAllPreviews(this BlockObjectLayout blockObjectLayout)
	{
		return blockObjectLayout == BlockObjectLayout.Half;
	}
}
