using System.Text;
using Timberborn.Common;
using Timberborn.DebuggingUI;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.TimeSystemUI;

public class ClockDebuggingPanel : ILoadableSingleton, IDebuggingPanel
{
	private readonly IDayNightCycle _dayNightCycle;

	private readonly DebuggingPanel _debuggingPanel;

	public ClockDebuggingPanel(IDayNightCycle dayNightCycle, DebuggingPanel debuggingPanel)
	{
		_dayNightCycle = dayNightCycle;
		_debuggingPanel = debuggingPanel;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Clock");
	}

	public string GetText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"Hours passed today: {_dayNightCycle.HoursPassedToday}");
		stringBuilder.AppendLine($"Day progress: {_dayNightCycle.DayProgress}");
		stringBuilder.AppendLine($"Partial day number: {_dayNightCycle.PartialDayNumber}");
		return stringBuilder.ToStringWithoutNewLineEnd();
	}
}
