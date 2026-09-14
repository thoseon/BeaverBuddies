using System;
using JetBrains.Annotations;

namespace Timberborn.BlueprintSystem;

[AttributeUsage(AttributeTargets.Property)]
[MeansImplicitUse]
public class SerializeAttribute : Attribute
{
	public string SourceName { get; }

	public bool HasSource => SourceName != null;

	public SerializeAttribute()
	{
	}

	public SerializeAttribute(string sourceName)
	{
		SourceName = sourceName;
	}
}
