using Timberborn.BlueprintSystem;
using Timberborn.Goods;

namespace Timberborn.Yielding;

public record YielderSpec
{
	[Serialize]
	public string YielderComponentName { get; init; }

	[Serialize]
	public GoodAmountSpec Yield { get; init; }

	[Serialize]
	public float RemovalTimeInHours { get; init; }

	[Serialize]
	public string ResourceGroup { get; init; }
}
