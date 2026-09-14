using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;
using Timberborn.DropdownSystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BatchControl;

public class TimeRangeDropdownProvider : IExtendedDropdownProvider, IDropdownProvider
{
	private static readonly string AllTimeLocKey = "BatchControl.TimeRange.AllTime";

	private static readonly string DaysLocKey = "BatchControl.TimeRange.Days";

	private readonly EventBus _eventBus;

	private readonly ILoc _loc;

	private int _selectedIndex;

	public IReadOnlyList<string> Items { get; }

	public int SelectedTimeRange => int.Parse(Items[_selectedIndex]);

	public bool AllTimeSelected => SelectedTimeRange == -1;

	public TimeRangeDropdownProvider(EventBus eventBus, ILoc loc, IEnumerable<string> items, int selectedIndex)
	{
		_eventBus = eventBus;
		_loc = loc;
		Items = items.ToImmutableArray();
		_selectedIndex = selectedIndex;
	}

	public string GetValue()
	{
		return Items[_selectedIndex];
	}

	public void SetValue(string value)
	{
		_selectedIndex = Items.IndexOf(value);
		_eventBus.Post(new BatchControlBoxTimeRangeChangedEvent());
	}

	public string FormatDisplayText(string value, bool selected)
	{
		if (value == "-1")
		{
			return _loc.T(AllTimeLocKey);
		}
		return _loc.T(DaysLocKey, value);
	}

	public Sprite GetIcon(string value)
	{
		return null;
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		return ImmutableArray<string>.Empty;
	}
}
