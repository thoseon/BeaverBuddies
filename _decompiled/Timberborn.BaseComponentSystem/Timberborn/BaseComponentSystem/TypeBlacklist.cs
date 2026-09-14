using System;
using System.Collections.Generic;
using Timberborn.BlueprintSystem;

namespace Timberborn.BaseComponentSystem;

internal class TypeBlacklist
{
	private static readonly HashSet<Type> BlacklistedTypes = new HashSet<Type>
	{
		typeof(BaseComponent),
		typeof(ComponentSpec),
		typeof(object)
	};

	public void Verify(Type type)
	{
		if (BlacklistedTypes.Contains(type))
		{
			string text = "BaseComponent";
			string arg = text + ".AllComponents";
			throw new InvalidOperationException($"Retrieving {type} through {text} is not allowed. Use {arg} instead.");
		}
	}
}
