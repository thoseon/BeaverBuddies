using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Timberborn.BlueprintSystem;

[AttributeUsage(AttributeTargets.Class)]
public class SpecAliasAttribute : Attribute
{
	public ImmutableArray<string> Aliases { get; }

	[UsedImplicitly]
	public SpecAliasAttribute(params string[] aliases)
	{
		Aliases = ((IEnumerable<string>)aliases).ToImmutableArray();
	}
}
