using Timberborn.BlueprintSystem;

namespace Timberborn.MechanicalSystem;

public record MechanicalNodeSpec : ComponentSpec
{
	[Serialize]
	public int PowerOutput { get; init; }

	[Serialize]
	public int PowerInput { get; init; }

	[Serialize]
	public bool IsShaft { get; init; }
}
