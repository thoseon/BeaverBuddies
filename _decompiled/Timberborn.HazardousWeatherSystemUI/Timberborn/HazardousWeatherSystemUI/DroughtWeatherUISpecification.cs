namespace Timberborn.HazardousWeatherSystemUI;

public class DroughtWeatherUISpecification : IHazardousWeatherUISpecification
{
	public string NameLocKey => "Weather.Drought";

	public string ApproachingLocKey => "Weather.Notification.DroughtApproaching";

	public string InProgressLocKey => "Weather.Notification.DroughtInProgress";

	public string StartedNotificationLocKey => "Weather.DroughtStartedNotification";

	public string EndedNotificationLocKey => "Weather.DroughtEndedNotification";

	public string InProgressClass => "weather-panel--dry";

	public string IconClass => "date-panel--drought";

	public string NotificationBackgroundClass => "hazardous-weather-notification__background--dry";
}
