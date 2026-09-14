using Timberborn.BlueprintSystem;

namespace Timberborn.ActivatorSystem;

public record TimedComponentActivatorSpec : ComponentSpec
{
	[Serialize]
	public bool IsOptionallyActivable { get; init; }

	[Serialize]
	public int CyclesUntilCountdownActivation { get; init; }

	[Serialize]
	public float DaysUntilActivation { get; init; }

	[Serialize]
	public string ProgressBarActiveLabelLocKey { get; init; }

	[Serialize]
	public string ProgressBarNotActiveLabelLocKey { get; init; }

	[Serialize]
	public bool IsHazardousActivator { get; init; }
}
