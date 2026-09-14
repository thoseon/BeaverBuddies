using System;
using System.Collections.Generic;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.BatchControl;

public class TimeRangeDropdownProviderFactory
{
	public static readonly int MinRange = 10;

	private static readonly List<int> Values = new List<int> { 10, 20, 50, 100, 250, -1 };

	private readonly EventBus _eventBus;

	private readonly ILoc _loc;

	public TimeRangeDropdownProviderFactory(EventBus eventBus, ILoc loc)
	{
		_eventBus = eventBus;
		_loc = loc;
	}

	public TimeRangeDropdownProvider Create(int defaultValue)
	{
		if (Values.Contains(defaultValue))
		{
			return new TimeRangeDropdownProvider(_eventBus, _loc, Values.ConvertAll((int v) => v.ToString()), Values.IndexOf(defaultValue));
		}
		throw new ArgumentException($"Default value {defaultValue} is not in the list of " + "allowed values: " + string.Join(", ", Values));
	}
}
