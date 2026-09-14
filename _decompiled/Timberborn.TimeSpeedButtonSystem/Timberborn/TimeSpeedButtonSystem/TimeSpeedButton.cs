using UnityEngine.UIElements;

namespace Timberborn.TimeSpeedButtonSystem;

public class TimeSpeedButton
{
	private static readonly string HighlightedClassName = "speed-button--highlighted";

	private readonly bool _devMode;

	public int TimeSpeed { get; }

	public Button Button { get; }

	public TimeSpeedButton(Button button, int timeSpeed)
	{
		Button = button;
		TimeSpeed = timeSpeed;
	}

	public void Highlight()
	{
		Button.AddToClassList(HighlightedClassName);
	}

	public void Unhighlight()
	{
		Button.RemoveFromClassList(HighlightedClassName);
	}
}
