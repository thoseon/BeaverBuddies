using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.PathSystem;

internal record DrivewayModelsSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<DrivewayModelSpec> Driveways { get; init; }
}
