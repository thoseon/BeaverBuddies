using Timberborn.BlueprintSystem;

namespace Timberborn.Workshops;

internal record ProductionResetterSpec : ComponentSpec
{
	[Serialize]
	public float HoursToResetProgress { get; init; }

	[Serialize]
	public string StatusLocKey { get; init; }

	[Serialize]
	public string AlertLocKey { get; init; }

	[Serialize]
	public string StatusIcon { get; init; }
}
