using Timberborn.BlueprintSystem;

namespace Timberborn.CharacterModelSystem;

internal record CharacterModelSpec : ComponentSpec
{
	[Serialize]
	public string ModelName { get; init; }
}
