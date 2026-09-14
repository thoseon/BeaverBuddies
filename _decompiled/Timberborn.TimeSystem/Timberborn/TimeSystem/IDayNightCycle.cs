namespace Timberborn.TimeSystem;

public interface IDayNightCycle
{
	float DayLengthInSeconds { get; }

	int DayNumber { get; }

	float DaytimeLengthInHours { get; }

	float NighttimeLengthInHours { get; }

	float HoursPassedToday { get; }

	float DayProgress { get; }

	float PartialDayNumber { get; }

	bool IsDaytime { get; }

	bool IsNighttime { get; }

	float FixedDeltaTimeInHours { get; }

	float FluidSecondsPassedToday { get; }

	float DayNumberHoursFromNow(float hours);

	(float start, float end) BoundsInHours(TimeOfDay timeOfDay);

	float HoursToNextStartOf(TimeOfDay timeOfDay);

	float SecondsToHours(float seconds);

	int HoursToTicks(float hours);

	float TicksToHours(int ticks);

	float FluidHoursToNextStartOf(TimeOfDay timeOfDay);

	void SetTimeToNextDay();

	void JumpTimeInHours(float hours);
}
