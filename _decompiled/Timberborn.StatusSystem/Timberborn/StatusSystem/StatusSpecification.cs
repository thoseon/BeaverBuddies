namespace Timberborn.StatusSystem;

public class StatusSpecification
{
	public string SpriteName { get; }

	public string StatusDescription { get; }

	public string AlertDescription { get; }

	public float DelayInHours { get; }

	public bool ShowFloatingIcon { get; }

	private StatusSpecification(string spriteName, string statusDescription, string alertDescription, float delayInHours, bool showFloatingIcon)
	{
		SpriteName = spriteName;
		StatusDescription = statusDescription;
		AlertDescription = alertDescription;
		DelayInHours = delayInHours;
		ShowFloatingIcon = showFloatingIcon;
	}

	public static StatusSpecification Create(string spriteName, string statusDescription)
	{
		return new StatusSpecification(spriteName, statusDescription, "", 0f, showFloatingIcon: false);
	}

	public static StatusSpecification CreateWithIcon(string spriteName, string statusDescription, float delayInHours)
	{
		return new StatusSpecification(spriteName, statusDescription, "", delayInHours, showFloatingIcon: true);
	}

	public static StatusSpecification CreateWithAlert(string spriteName, string statusDescription, string alertDescription, float delayInHours)
	{
		return new StatusSpecification(spriteName, statusDescription, alertDescription, delayInHours, showFloatingIcon: false);
	}

	public static StatusSpecification CreateWithAlertAndIcon(string spriteName, string statusDescription, string alertDescription, float delayInHours)
	{
		return new StatusSpecification(spriteName, statusDescription, alertDescription, delayInHours, showFloatingIcon: true);
	}
}
