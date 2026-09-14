using System;
using System.Globalization;
using Timberborn.Localization;

namespace Timberborn.UIFormatters;

public static class NumberFormatter
{
	private static readonly string KiloLocKey = "Unit.Compact.Kilo.NumberAndUnit";

	private static readonly string MegaLocKey = "Unit.Compact.Mega.NumberAndUnit";

	private static readonly NumberFormatInfo NumberFormatInfo = new NumberFormatInfo
	{
		NumberGroupSeparator = " "
	};

	public static Phrase FormatCompact(this Phrase phrase)
	{
		return phrase.Format<int>(FormatCompact);
	}

	public static string FormatFullNumber(int number)
	{
		return number.ToString("N0", NumberFormatInfo);
	}

	public static string CeilToTenthsPlace(double value)
	{
		double num = Math.Ceiling(10.0 * value) / 10.0;
		return $"{num:0.0}";
	}

	internal static string FormatCompact(int value, ILoc loc)
	{
		if (value < 1000000)
		{
			if (value >= 1000)
			{
				if (value < 10000)
				{
					return ToCompactString(Math.Floor((float)value / 100f) / 10.0, 1, KiloLocKey, loc);
				}
				return ToCompactString(Math.Floor((float)value / 1000f), 0, KiloLocKey, loc);
			}
			return $"{value:F0}";
		}
		if (value < 10000000)
		{
			return ToCompactString(Math.Floor((float)value / 100000f) / 10.0, 1, MegaLocKey, loc);
		}
		return ToCompactString(Math.Floor((float)value / 1000000f), 0, MegaLocKey, loc);
	}

	private static string ToCompactString(double value, int precision, string locKey, ILoc loc)
	{
		return loc.T(locKey, value.ToString($"F{precision}", CultureInfo.InvariantCulture) ?? "");
	}
}
