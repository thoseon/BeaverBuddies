using Timberborn.BlueprintSystem;
using Timberborn.Yielding;

namespace Timberborn.Ruins;

internal record RuinSpec : ComponentSpec, IYielderDecorable, IOrderableYielder
{
	[Serialize]
	public string ModelParentName { get; init; }

	[Serialize]
	public int RuinHeight { get; init; }

	[Serialize]
	public YielderSpec Yielder { get; init; }

	public int Order => RuinHeight;
}
