using Timberborn.BlueprintSystem;

namespace Timberborn.WorkshopsEffects;

internal record ObservatoryAnimatorSpec : ComponentSpec
{
	[Serialize]
	public string DomeName { get; init; }

	[Serialize]
	public string TelescopeName { get; init; }
}
