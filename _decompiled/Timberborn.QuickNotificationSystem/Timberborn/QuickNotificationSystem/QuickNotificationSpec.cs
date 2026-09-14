using Timberborn.BlueprintSystem;

namespace Timberborn.QuickNotificationSystem;

internal record QuickNotificationSpec : ComponentSpec
{
	[Serialize]
	public float Duration { get; init; }

	[Serialize]
	public float ExtendedDuration { get; init; }
}
