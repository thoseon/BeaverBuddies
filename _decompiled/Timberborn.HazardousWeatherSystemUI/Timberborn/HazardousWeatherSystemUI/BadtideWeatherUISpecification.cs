namespace Timberborn.HazardousWeatherSystemUI;

public class BadtideWeatherUISpecification : IHazardousWeatherUISpecification
{
	public string NameLocKey => "Weather.Badtide";

	public string ApproachingLocKey => "Weather.Notification.BadtideApproaching";

	public string InProgressLocKey => "Weather.Notification.BadtideInProgress";

	public string StartedNotificationLocKey => "Weather.BadtideStartedNotification";

	public string EndedNotificationLocKey => "Weather.BadtideEndedNotification";

	public string InProgressClass => "weather-panel--badtide";

	public string IconClass => "date-panel--badtide";

	public string NotificationBackgroundClass => "hazardous-weather-notification__background--badtide";
}
