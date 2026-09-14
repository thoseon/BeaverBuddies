using System;

namespace Timberborn.Brushes;

public static class BrushShapeExtensions
{
	private static readonly string RoundLocKey = "MapEditor.Brush.Shape.Round";

	private static readonly string SquareLocKey = "MapEditor.Brush.Shape.Square";

	public static string GetLocKey(this BrushShape brushShape)
	{
		return brushShape switch
		{
			BrushShape.Square => SquareLocKey, 
			BrushShape.Round => RoundLocKey, 
			_ => throw new ArgumentOutOfRangeException("brushShape", brushShape, null), 
		};
	}
}
