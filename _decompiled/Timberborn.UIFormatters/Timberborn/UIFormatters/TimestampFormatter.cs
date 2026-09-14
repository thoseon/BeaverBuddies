using Timberborn.Localization;

namespace Timberborn.UIFormatters;

public class TimestampFormatter
{
	private static readonly string CycleAndDayLongLocKey = "Weather.CycleAndDayLong";

	private static readonly string CycleAndDayShortLocKey = "Weather.CycleAndDayShort";

	private readonly ILoc _loc;

	public TimestampFormatter(ILoc loc)
	{
		_loc = loc;
	}

	public string FormatLongLocalized(int cycle, int day)
	{
		return _loc.T(CycleAndDayLongLocKey, cycle, day);
	}

	public string FormatShortLocalized(int cycle, int day)
	{
		return _loc.T(CycleAndDayShortLocKey, cycle, day);
	}

	public string FormatShort(int cycle, int day)
	{
		return $"{cycle}-{day}";
	}
}
