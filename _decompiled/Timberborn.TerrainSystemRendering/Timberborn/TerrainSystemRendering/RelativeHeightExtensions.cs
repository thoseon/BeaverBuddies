using System;

namespace Timberborn.TerrainSystemRendering;

public static class RelativeHeightExtensions
{
	public static RelativeHeight FromModelNameCharacter(char modelNameCharacter)
	{
		return modelNameCharacter switch
		{
			'0' => RelativeHeight.Equal, 
			'U' => RelativeHeight.Higher, 
			'D' => RelativeHeight.Lower, 
			'E' => RelativeHeight.Empty, 
			'V' => RelativeHeight.Overhang, 
			_ => throw new ArgumentException($"Invalid input: {modelNameCharacter}", "modelNameCharacter"), 
		};
	}

	public static char ToModelNameCharacter(RelativeHeight relativeHeight)
	{
		return relativeHeight switch
		{
			RelativeHeight.Equal => '0', 
			RelativeHeight.Higher => 'U', 
			RelativeHeight.Lower => 'D', 
			RelativeHeight.Empty => 'E', 
			RelativeHeight.Overhang => 'V', 
			_ => throw new ArgumentException($"Unexpected input: {relativeHeight}", "relativeHeight"), 
		};
	}
}
