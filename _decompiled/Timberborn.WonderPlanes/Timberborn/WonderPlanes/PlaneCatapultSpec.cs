using Timberborn.BlueprintSystem;
using Timberborn.UnityEngineSpecs;

namespace Timberborn.WonderPlanes;

internal record PlaneCatapultSpec : ComponentSpec
{
	[Serialize]
	public AnimationCurveSpec SpeedCurve { get; init; }
}
