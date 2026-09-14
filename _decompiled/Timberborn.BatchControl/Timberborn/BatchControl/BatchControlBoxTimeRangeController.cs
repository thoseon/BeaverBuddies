using System;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.TimeSystem;
using UnityEngine.UIElements;

namespace Timberborn.BatchControl;

public class BatchControlBoxTimeRangeController
{
	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly IDayNightCycle _dayNightCycle;

	private Dropdown _dropdown;

	private TimeRangeDropdownProvider _timeRangeDropdownProvider;

	public bool AllTimeSelected => _timeRangeDropdownProvider.AllTimeSelected;

	internal BatchControlBoxTimeRangeController(DropdownItemsSetter dropdownItemsSetter, IDayNightCycle dayNightCycle)
	{
		_dropdownItemsSetter = dropdownItemsSetter;
		_dayNightCycle = dayNightCycle;
	}

	public void Initialize(VisualElement root)
	{
		_dropdown = root.Q<Dropdown>("TimeRangeDropdown");
	}

	public void SetTimeRangeDropdownProvider(TimeRangeDropdownProvider timeRangeDropdownProvider)
	{
		_timeRangeDropdownProvider = timeRangeDropdownProvider;
		if (timeRangeDropdownProvider != null)
		{
			_dropdownItemsSetter.SetItems(_dropdown, timeRangeDropdownProvider);
			_dropdown.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_dropdown.ClearItems();
			_dropdown.ToggleDisplayStyle(visible: false);
		}
	}

	public int GetDaysTimeRange()
	{
		if (_timeRangeDropdownProvider.AllTimeSelected)
		{
			return Math.Max(TimeRangeDropdownProviderFactory.MinRange, _dayNightCycle.DayNumber);
		}
		return _timeRangeDropdownProvider.SelectedTimeRange;
	}
}
