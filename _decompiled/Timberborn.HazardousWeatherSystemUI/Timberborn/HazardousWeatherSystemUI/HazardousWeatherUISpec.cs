using Timberborn.BlueprintSystem;

namespace Timberborn.HazardousWeatherSystemUI;

internal record HazardousWeatherUISpec : ComponentSpec
{
	[Serialize]
	public int ApproachingNotificationDays { get; init; }

	[Serialize]
	public float MaxDayProgressLeftToNotify { get; init; }

	[Serialize]
	public float NotificationDuration { get; init; }
}
