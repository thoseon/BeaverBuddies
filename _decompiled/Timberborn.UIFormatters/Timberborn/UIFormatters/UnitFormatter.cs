using System;
using System.Globalization;
using Timberborn.Localization;
using UnityEngine;

namespace Timberborn.UIFormatters;

public static class UnitFormatter
{
	private static readonly string PercentUnitLocKey = "Unit.Percent.NumberAndUnit";

	private static readonly string TickUnitLocKey = "Unit.Tick.NumberAndUnit";

	private static readonly string HourUnitLocKey = "Unit.Hour.NumberAndUnit";

	private static readonly string DayUnitLocKey = "Unit.Day.NumberAndUnit";

	private static readonly string FlowUnitLocKey = "Unit.CubicMeterPerSecond.NumberAndUnit";

	private static readonly string DistanceUnitLocKey = "Unit.Meter.NumberAndUnit";

	private static readonly string AngleUnitLocKey = "Unit.Degree.NumberAndUnit";

	private static readonly string KilogramUnitLocKey = "Unit.Kilogram.NumberAndUnit";

	private static readonly string PowerUnitLocKey = "Unit.HorsePower.NumberAndUnit";

	private static readonly string PowerCapacityUnitLocKey = "Unit.HorsePowerHour.NumberAndUnit";

	private static readonly string PowerCapacityPerMeterUnitLocKey = "Unit.HorsePowerHourPerMeter.NumberAndUnit";

	private static CultureInfo Culture => CultureInfo.InvariantCulture;

	public static Phrase FormatPercentRounded(this Phrase phrase, string format = "F0")
	{
		return FormatPhraseWithPercent(phrase, (float value) => Mathf.Round(value * 100f), format);
	}

	public static Phrase FormatPercentCeiled(this Phrase phrase, string format = "F0")
	{
		return FormatPhraseWithPercent(phrase, (float value) => Mathf.Ceil(value * 100f), format);
	}

	public static Phrase FormatPercentFloored(this Phrase phrase, string format = "F0")
	{
		return FormatPhraseWithPercent(phrase, (float value) => Mathf.Floor(value * 100f), format);
	}

	public static Phrase FormatTicks<T>(this Phrase phrase, string format = null) where T : IFormattable
	{
		return FormatPhraseWithUnit<T>(phrase, format, TickUnitLocKey);
	}

	public static Phrase FormatHours<T>(this Phrase phrase, string format = null) where T : IFormattable
	{
		return FormatPhraseWithUnit<T>(phrase, format, HourUnitLocKey);
	}

	public static Phrase FormatHours<T>(this Phrase phrase, Func<T, string> formatter) where T : IFormattable
	{
		return FormatPhraseWithUnit(phrase, formatter, HourUnitLocKey);
	}

	public static Phrase FormatDays<T>(this Phrase phrase, string format = null) where T : IFormattable
	{
		return FormatPhraseWithUnit<T>(phrase, format, DayUnitLocKey);
	}

	public static Phrase FormatFlow<T>(this Phrase phrase, string format = null) where T : IFormattable
	{
		return FormatPhraseWithUnit<T>(phrase, format, FlowUnitLocKey);
	}

	public static Phrase FormatDistance<T>(this Phrase phrase, string format = null) where T : IFormattable
	{
		return FormatPhraseWithUnit<T>(phrase, format, DistanceUnitLocKey);
	}

	public static Phrase FormatAngle<T>(this Phrase phrase, string format = null) where T : IFormattable
	{
		return FormatPhraseWithUnit<T>(phrase, format, AngleUnitLocKey);
	}

	public static Phrase FormatKilogram<T>(this Phrase phrase, string format = null) where T : IFormattable
	{
		return FormatPhraseWithUnit<T>(phrase, format, KilogramUnitLocKey);
	}

	public static Phrase FormatPower<T>(this Phrase phrase, string format = null) where T : IFormattable
	{
		return phrase.FormatPower<T>(() => PowerUnitLocKey, format);
	}

	public static Phrase FormatPower<T>(this Phrase phrase, Func<string> unitLocKeyProvider, string format = null) where T : IFormattable
	{
		return FormatPhraseWithUnit<T>(phrase, format, unitLocKeyProvider?.Invoke() ?? PowerUnitLocKey);
	}

	public static Phrase FormatPowerCapacity<T>(this Phrase phrase, string format = null) where T : IFormattable
	{
		return FormatPhraseWithUnit<T>(phrase, format, PowerCapacityUnitLocKey);
	}

	public static Phrase FormatPowerCapacityPerMeter<T>(this Phrase phrase, string format = null) where T : IFormattable
	{
		return FormatPhraseWithUnit<T>(phrase, format, PowerCapacityPerMeterUnitLocKey);
	}

	private static Phrase FormatPhraseWithPercent(Phrase phrase, Func<float, float> valueFormatter, string format)
	{
		return phrase.Format((float value, ILoc loc) => loc.T(PercentUnitLocKey, valueFormatter(value).ToString(format, Culture)));
	}

	private static Phrase FormatPhraseWithUnit<T>(Phrase phrase, string format, string unitLocKey) where T : IFormattable
	{
		return FormatPhraseWithUnit(phrase, (T _) => format, unitLocKey);
	}

	private static Phrase FormatPhraseWithUnit<T>(Phrase phrase, Func<T, string> formatter, string unitLocKey) where T : IFormattable
	{
		return phrase.Format((T value, ILoc loc) => loc.T(unitLocKey, value.ToString(formatter(value), Culture)));
	}
}
