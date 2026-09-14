using Timberborn.BlueprintSystem;

namespace Timberborn.RecoveredGoodSystem;

internal record RecoveredGoodStackDisintegrationSpec : ComponentSpec
{
	[Serialize]
	public float DaysToDisintegrate { get; init; }
}
