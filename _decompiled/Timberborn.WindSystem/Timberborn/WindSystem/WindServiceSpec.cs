using Timberborn.BlueprintSystem;

namespace Timberborn.WindSystem;

internal record WindServiceSpec : ComponentSpec
{
	[Serialize]
	public float MinWindTimeInHours { get; init; }

	[Serialize]
	public float MaxWindTimeInHours { get; init; }

	[Serialize]
	public float MinWindStrength { get; init; }

	[Serialize]
	public float MaxWindStrength { get; init; }
}
