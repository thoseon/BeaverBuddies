using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;

namespace Timberborn.WaterBuildingsUI;

internal record WaterOutputParticleColorsSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<GradientPointSpec> WaterContaminationParticleGradient { get; init; }
}
