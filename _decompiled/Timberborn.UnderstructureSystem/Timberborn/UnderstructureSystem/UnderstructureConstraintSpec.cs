using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.UnderstructureSystem;

public record UnderstructureConstraintSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> UnderstructureTemplateNames { get; init; }
}
