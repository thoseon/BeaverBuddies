namespace Timberborn.TimeSystem;

public class TimeFastForwarder
{
	private static readonly float[] JumpHours = new float[4] { 0.5f, 4f, 16.5f, 20f };

	private readonly IDayNightCycle _dayNightCycle;

	public TimeFastForwarder(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void JumpToNextDaytime()
	{
		_dayNightCycle.JumpTimeInHours(GetJumpDeltaInHours());
	}

	private float GetJumpDeltaInHours()
	{
		float hoursPassedToday = _dayNightCycle.HoursPassedToday;
		for (int i = 0; i < JumpHours.Length; i++)
		{
			float num = JumpHours[i];
			if (hoursPassedToday < num)
			{
				return num - hoursPassedToday;
			}
		}
		return 24f - hoursPassedToday + JumpHours[0];
	}
}
