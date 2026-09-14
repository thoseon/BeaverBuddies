using Timberborn.BlueprintSystem;

namespace Timberborn.BlockSystem;

public record BlockSpec
{
	[Serialize]
	public MatterBelow MatterBelow { get; init; }

	[Serialize]
	public BlockOccupations Occupations { get; init; }

	[Serialize]
	public BlockStackable Stackable { get; init; }

	[Serialize]
	public bool OccupyAllBelow { get; init; }

	[Serialize]
	public bool Underground { get; init; }
}
