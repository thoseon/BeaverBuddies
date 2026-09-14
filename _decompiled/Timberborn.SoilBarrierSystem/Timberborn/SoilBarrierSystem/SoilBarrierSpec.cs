using Timberborn.BlueprintSystem;

namespace Timberborn.SoilBarrierSystem;

internal record SoilBarrierSpec : ComponentSpec
{
	[Serialize]
	public bool BlockAboveMoisture { get; init; }

	[Serialize]
	public bool BlockFullMoisture { get; init; }

	[Serialize]
	public bool BlockContamination { get; init; }
}
