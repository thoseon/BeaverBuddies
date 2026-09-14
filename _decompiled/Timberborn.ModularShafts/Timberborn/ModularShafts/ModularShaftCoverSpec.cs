using Timberborn.BlueprintSystem;

namespace Timberborn.ModularShafts;

internal record ModularShaftCoverSpec : ComponentSpec
{
	[Serialize]
	public string CoverModelName { get; init; }
}
