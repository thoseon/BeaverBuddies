using Timberborn.BlueprintSystem;

namespace Timberborn.MechanicalSystemUI;

internal record MechanicalNodeDescriptionSpec : ComponentSpec
{
	[Serialize]
	public string AlternativePowerUnitLocKey { get; init; }
}
