namespace Timberborn.TimeSystem;

public class CurrentSpeedChangedEvent
{
	public float CurrentSpeed { get; }

	public CurrentSpeedChangedEvent(float currentSpeed)
	{
		CurrentSpeed = currentSpeed;
	}
}
