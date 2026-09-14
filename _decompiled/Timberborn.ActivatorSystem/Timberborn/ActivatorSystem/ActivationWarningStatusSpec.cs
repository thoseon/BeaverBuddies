using Timberborn.BlueprintSystem;

namespace Timberborn.ActivatorSystem;

internal record ActivationWarningStatusSpec : ComponentSpec
{
	[Serialize]
	public string StatusSpriteName { get; init; }

	[Serialize]
	public string StatusLocKey { get; init; }

	[Serialize]
	public bool UseInfiniteWarning { get; init; }

	[Serialize]
	public string WarningSound { get; init; }
}
