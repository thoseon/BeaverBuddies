using Timberborn.BlueprintSystem;

namespace Timberborn.Carrying;

internal record GoodCarrierSpec : ComponentSpec
{
	[Serialize]
	public int BaseLiftingCapacity { get; init; }
}
