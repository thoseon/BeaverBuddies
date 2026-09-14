using System;
using System.Globalization;

namespace Timberborn.Persistence;

public class InvariantDateTimeSerializer : IValueSerializer<DateTime>
{
	public void Serialize(DateTime value, IValueSaver valueSaver)
	{
		valueSaver.AsString(value.ToString(DateTimeFormatInfo.InvariantInfo));
	}

	public Obsoletable<DateTime> Deserialize(IValueLoader valueLoader)
	{
		return DateTime.Parse(valueLoader.AsString(), DateTimeFormatInfo.InvariantInfo);
	}
}
