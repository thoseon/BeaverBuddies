using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Workshops;

public record ManufactorySpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> ProductionRecipeIds { get; init; }
}
