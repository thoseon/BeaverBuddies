using Timberborn.BlueprintSystem;

namespace Timberborn.RecoveredGoodSystem;

internal record RecoveredGoodStackModelSpec : ComponentSpec
{
	[Serialize]
	public string ModelName { get; init; }
}
