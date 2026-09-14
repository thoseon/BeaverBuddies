using Timberborn.BlueprintSystem;

namespace Timberborn.MechanicalSystem;

internal record MechanicalNodeTransformHeightSpec : ComponentSpec
{
	[Serialize]
	public string TransformName { get; init; }

	[Serialize]
	public float Range { get; init; }

	[Serialize]
	public float ChangeSpeed { get; init; }
}
