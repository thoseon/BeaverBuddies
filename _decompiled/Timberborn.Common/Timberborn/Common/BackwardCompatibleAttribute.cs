using System;

namespace Timberborn.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class BackwardCompatibleAttribute : Attribute
{
	public Compatibility Compatibility { get; }

	public DateTime Date { get; }

	public BackwardCompatibleAttribute(int year, int month, int day, Compatibility compatibility)
	{
		Date = new DateTime(year, month, day);
		Compatibility = compatibility;
	}
}
