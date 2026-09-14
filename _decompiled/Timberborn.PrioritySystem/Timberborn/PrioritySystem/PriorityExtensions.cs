using System;

namespace Timberborn.PrioritySystem;

public static class PriorityExtensions
{
	public static string GetLocKey(this Priority priority)
	{
		return priority switch
		{
			Priority.VeryLow => "Priorities.VeryLow", 
			Priority.Low => "Priorities.Low", 
			Priority.Normal => "Priorities.Normal", 
			Priority.High => "Priorities.High", 
			Priority.VeryHigh => "Priorities.VeryHigh", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
