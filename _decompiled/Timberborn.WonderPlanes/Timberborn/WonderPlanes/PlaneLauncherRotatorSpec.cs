using Timberborn.BlueprintSystem;
using Timberborn.UnityEngineSpecs;

namespace Timberborn.WonderPlanes;

internal record PlaneLauncherRotatorSpec : ComponentSpec
{
	[Serialize]
	public string RotatedElementName { get; init; }

	[Serialize]
	public float FullRotationDuration { get; init; }

	[Serialize]
	public AnimationCurveSpec RotationCurve { get; init; }
}
