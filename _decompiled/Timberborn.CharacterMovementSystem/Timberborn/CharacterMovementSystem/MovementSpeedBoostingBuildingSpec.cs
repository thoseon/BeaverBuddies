using Timberborn.BlueprintSystem;

namespace Timberborn.CharacterMovementSystem;

public record MovementSpeedBoostingBuildingSpec : ComponentSpec
{
	[Serialize]
	public int BoostPercentage { get; init; }
}
