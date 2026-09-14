using System.Collections.Generic;

namespace Timberborn.BlueprintSystem;

public interface IBlueprintModifierProvider
{
	string ModifierName { get; }

	IEnumerable<string> GetModifiers(string blueprintPath);
}
