using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.MechanicalSystem;

public record TransputProviderSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<TransputSpec> Transputs { get; init; }

	[Serialize]
	public bool IgnoreRotation { get; init; }
}
