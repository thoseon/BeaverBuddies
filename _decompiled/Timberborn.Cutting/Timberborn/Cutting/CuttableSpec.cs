using Timberborn.BlueprintSystem;
using Timberborn.Yielding;

namespace Timberborn.Cutting;

public record CuttableSpec : ComponentSpec, IYielderDecorable
{
	[Serialize]
	public bool RemoveOnCut { get; init; }

	[Serialize]
	public string LeftoverModelName { get; init; }

	[Serialize]
	public YielderSpec Yielder { get; init; }
}
