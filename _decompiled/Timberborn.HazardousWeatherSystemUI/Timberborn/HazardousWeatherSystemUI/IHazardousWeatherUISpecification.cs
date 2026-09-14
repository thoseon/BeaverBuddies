namespace Timberborn.HazardousWeatherSystemUI;

internal interface IHazardousWeatherUISpecification
{
	string NameLocKey { get; }

	string ApproachingLocKey { get; }

	string InProgressLocKey { get; }

	string StartedNotificationLocKey { get; }

	string EndedNotificationLocKey { get; }

	string InProgressClass { get; }

	string IconClass { get; }

	string NotificationBackgroundClass { get; }
}
