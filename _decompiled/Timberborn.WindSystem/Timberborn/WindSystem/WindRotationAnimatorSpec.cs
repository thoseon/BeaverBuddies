using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WindSystem;

internal record WindRotationAnimatorSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<WindRotatorSpec> WindRotators { get; init; }

	[Serialize]
	public WindRotatorSpec Tower { get; init; }
}
