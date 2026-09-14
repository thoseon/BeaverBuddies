using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WaterSourceSystem;

internal record WaterSourceRegulatorAnimationControllerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<RegulatorTransformSpec> RegulatorTransforms { get; init; }
}
