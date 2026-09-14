using Timberborn.BlueprintSystem;

namespace Timberborn.GameDistrictsMigration;

internal record MigrationCoordinatorSpec : ComponentSpec
{
	[Serialize]
	public int MaxAutomaticMigration { get; init; }
}
