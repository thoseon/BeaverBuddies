using Timberborn.Debugging;
using Timberborn.QuickNotificationSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.TimeSystemUI;

internal class StopwatchDevModule : IDevModule
{
	private readonly IDayNightCycle _dayNightCycle;

	private readonly EventBus _eventBus;

	private readonly QuickNotificationService _quickNotificationService;

	private int _dayCounter;

	private float? _hoursPassedAtStart;

	public StopwatchDevModule(IDayNightCycle dayNightCycle, EventBus eventBus, QuickNotificationService quickNotificationService)
	{
		_dayNightCycle = dayNightCycle;
		_eventBus = eventBus;
		_quickNotificationService = quickNotificationService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Stopwatch: Restart", RestartStopwatch)).AddMethod(DevMethod.Create("Stopwatch: Log", LogStopwatch)).Build();
	}

	[OnEvent]
	public void OnDaytimeStarted(DaytimeStartEvent daytimeStartEvent)
	{
		_dayCounter++;
	}

	private void RestartStopwatch()
	{
		if (!_hoursPassedAtStart.HasValue)
		{
			_eventBus.Register(this);
			LogMessage("Stopwatch started");
		}
		else
		{
			LogMessage("Stopwatch restarted\n" + GetElapsedHoursMessage(_hoursPassedAtStart.Value));
		}
		_hoursPassedAtStart = _dayNightCycle.HoursPassedToday;
		_dayCounter = 0;
	}

	private void LogStopwatch()
	{
		LogMessage(_hoursPassedAtStart.HasValue ? GetElapsedHoursMessage(_hoursPassedAtStart.Value) : "Stopwatch not started");
	}

	private void LogMessage(string message)
	{
		Debug.Log(message);
		_quickNotificationService.SendNotification(message);
	}

	private string GetElapsedHoursMessage(float hoursPassedAtStart)
	{
		float num = CalculateElapsedHours(hoursPassedAtStart);
		return $"Elapsed hours: {num:F4}";
	}

	private float CalculateElapsedHours(float hoursPassedAtStart)
	{
		if (_dayCounter > 0)
		{
			float num = 24f - hoursPassedAtStart;
			int num2 = (_dayCounter - 1) * 24;
			return num + (float)num2 + _dayNightCycle.HoursPassedToday;
		}
		return _dayNightCycle.HoursPassedToday - hoursPassedAtStart;
	}
}
