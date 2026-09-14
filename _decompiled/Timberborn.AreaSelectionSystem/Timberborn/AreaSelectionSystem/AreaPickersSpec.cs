using Timberborn.BlueprintSystem;

namespace Timberborn.AreaSelectionSystem;

internal record AreaPickersSpec : ComponentSpec
{
	[Serialize]
	public int AreaMaxBlocks { get; init; }

	[Serialize]
	public int SculptingMaxBlocks { get; init; }
}
