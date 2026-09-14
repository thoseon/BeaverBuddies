using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.Common;

namespace Timberborn.GameFactionSystem;

public class FactionBlueprintModifierProvider : IBlueprintModifierProvider
{
	private ImmutableArray<BlueprintModifierSpec> _blueprintModifierSpecs;

	public string ModifierName => "Faction modifier";

	private bool Initialized => !_blueprintModifierSpecs.IsDefault;

	public void Initialize(IEnumerable<BlueprintModifierSpec> modifiers)
	{
		Asserts.IsFalse(this, Initialized, "Initialized");
		_blueprintModifierSpecs = modifiers.ToImmutableArray();
	}

	public IEnumerable<string> GetModifiers(string blueprintPath)
	{
		if (!Initialized)
		{
			yield break;
		}
		foreach (BlueprintModifierSpec blueprintModifierSpec in _blueprintModifierSpecs)
		{
			if (blueprintModifierSpec.Original.Asset.Path == blueprintPath)
			{
				yield return blueprintModifierSpec.Modifier.Asset.Content;
			}
		}
	}
}
