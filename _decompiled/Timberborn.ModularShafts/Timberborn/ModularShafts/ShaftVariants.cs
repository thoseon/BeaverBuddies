using System.Collections.Generic;

namespace Timberborn.ModularShafts;

internal static class ShaftVariants
{
	private static readonly List<ShaftVariant> HorizontalVariants = new List<ShaftVariant>
	{
		ShaftVariant.CreateHorizontal(0, 0, 0, 0),
		ShaftVariant.CreateHorizontal(0, 0, 0, 1),
		ShaftVariant.CreateHorizontal(0, 0, 0, 2),
		ShaftVariant.CreateHorizontal(0, 1, 0, 1),
		ShaftVariant.CreateHorizontal(0, 1, 0, 2),
		ShaftVariant.CreateHorizontal(0, 2, 0, 1),
		ShaftVariant.CreateHorizontal(0, 2, 0, 2),
		ShaftVariant.CreateHorizontal(1, 0, 0, 1),
		ShaftVariant.CreateHorizontal(1, 0, 0, 2),
		ShaftVariant.CreateHorizontal(2, 0, 0, 1),
		ShaftVariant.CreateHorizontal(2, 0, 0, 2),
		ShaftVariant.CreateHorizontal(1, 1, 0, 1),
		ShaftVariant.CreateHorizontal(1, 1, 0, 2),
		ShaftVariant.CreateHorizontal(1, 2, 0, 1),
		ShaftVariant.CreateHorizontal(2, 1, 0, 1),
		ShaftVariant.CreateHorizontal(2, 2, 0, 1),
		ShaftVariant.CreateHorizontal(1, 2, 0, 2),
		ShaftVariant.CreateHorizontal(2, 1, 0, 2),
		ShaftVariant.CreateHorizontal(2, 2, 0, 2),
		ShaftVariant.CreateHorizontal(1, 1, 1, 1),
		ShaftVariant.CreateHorizontal(1, 1, 1, 2),
		ShaftVariant.CreateHorizontal(1, 2, 1, 1),
		ShaftVariant.CreateHorizontal(1, 2, 1, 2),
		ShaftVariant.CreateHorizontal(2, 1, 1, 1),
		ShaftVariant.CreateHorizontal(2, 1, 1, 2),
		ShaftVariant.CreateHorizontal(2, 2, 1, 1),
		ShaftVariant.CreateHorizontal(2, 2, 1, 2),
		ShaftVariant.CreateHorizontal(1, 1, 2, 1),
		ShaftVariant.CreateHorizontal(1, 1, 2, 2),
		ShaftVariant.CreateHorizontal(1, 2, 2, 1),
		ShaftVariant.CreateHorizontal(1, 2, 2, 2),
		ShaftVariant.CreateHorizontal(2, 1, 2, 1),
		ShaftVariant.CreateHorizontal(2, 1, 2, 2),
		ShaftVariant.CreateHorizontal(2, 2, 2, 1),
		ShaftVariant.CreateHorizontal(2, 2, 2, 2)
	};

	public static IEnumerable<ShaftVariant> GetAllVariants()
	{
		foreach (ShaftVariant horizontalVariant in HorizontalVariants)
		{
			yield return horizontalVariant;
			yield return horizontalVariant.ToFacingTop();
			yield return horizontalVariant.ToFacingTopReversed();
			yield return horizontalVariant.ToFacingBottom();
			yield return horizontalVariant.ToFacingBottomReversed();
			yield return horizontalVariant.ToFacingTopAndBottom(reverseBottom: false, reverseTop: false);
			yield return horizontalVariant.ToFacingTopAndBottom(reverseBottom: true, reverseTop: false);
			yield return horizontalVariant.ToFacingTopAndBottom(reverseBottom: false, reverseTop: true);
			yield return horizontalVariant.ToFacingTopAndBottom(reverseBottom: true, reverseTop: true);
		}
	}
}
