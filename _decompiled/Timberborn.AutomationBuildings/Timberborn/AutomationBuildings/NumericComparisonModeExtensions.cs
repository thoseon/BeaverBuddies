using System;

namespace Timberborn.AutomationBuildings;

public static class NumericComparisonModeExtensions
{
	public static bool Evaluate(this NumericComparisonMode mode, int value, int reference)
	{
		return mode switch
		{
			NumericComparisonMode.Equal => value == reference, 
			NumericComparisonMode.NotEqual => value != reference, 
			NumericComparisonMode.Greater => value > reference, 
			NumericComparisonMode.GreaterOrEqual => value >= reference, 
			NumericComparisonMode.Less => value < reference, 
			NumericComparisonMode.LessOrEqual => value <= reference, 
			_ => throw new ArgumentOutOfRangeException("mode", mode, null), 
		};
	}

	public static bool Evaluate(this NumericComparisonMode mode, float value, float reference)
	{
		return mode switch
		{
			NumericComparisonMode.Equal => value == reference, 
			NumericComparisonMode.NotEqual => value != reference, 
			NumericComparisonMode.Greater => value > reference, 
			NumericComparisonMode.GreaterOrEqual => value >= reference, 
			NumericComparisonMode.Less => value < reference, 
			NumericComparisonMode.LessOrEqual => value <= reference, 
			_ => throw new ArgumentOutOfRangeException("mode", mode, null), 
		};
	}
}
