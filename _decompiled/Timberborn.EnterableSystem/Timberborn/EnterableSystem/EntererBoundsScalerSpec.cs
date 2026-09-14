using Timberborn.BlueprintSystem;

namespace Timberborn.EnterableSystem;

internal record EntererBoundsScalerSpec : ComponentSpec
{
	[Serialize]
	public float Scale { get; init; }
}
