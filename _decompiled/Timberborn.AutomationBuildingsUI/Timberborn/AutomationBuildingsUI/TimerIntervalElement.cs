using System;
using Timberborn.AutomationBuildings;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class TimerIntervalElement
{
	private static readonly string IntervalTypeLocKey = "Building.Timer.IntervalType.";

	private readonly EnumDropdownProviderFactory _enumDropdownProviderFactory;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private VisualElement _root;

	private FloatField _timeField;

	private Dropdown _typeDropdown;

	private EnumDropdownProvider<IntervalType> _intervalTypeDropdownProvider;

	private TimerInterval _timerInterval;

	public TimerIntervalElement(EnumDropdownProviderFactory enumDropdownProviderFactory, DropdownItemsSetter dropdownItemsSetter)
	{
		_enumDropdownProviderFactory = enumDropdownProviderFactory;
		_dropdownItemsSetter = dropdownItemsSetter;
	}

	public void Initialize(VisualElement root)
	{
		_root = root;
		_timeField = _root.Q<FloatField>("TimeField");
		_timeField.isDelayed = true;
		_timeField.RegisterValueChangedCallback(SetTime);
		_typeDropdown = _root.Q<Dropdown>("IntervalTypeDropdown");
		_intervalTypeDropdownProvider = _enumDropdownProviderFactory.CreateLocalized(() => _timerInterval.Type, SetIntervalType, IntervalTypeLocKey);
	}

	public void Show(TimerInterval timerInterval)
	{
		_timerInterval = timerInterval;
		_dropdownItemsSetter.SetItems(_typeDropdown, _intervalTypeDropdownProvider);
	}

	public void Update()
	{
		if (!_timeField.IsFocused())
		{
			_timeField.SetValueWithoutNotify(_timerInterval.GetTypeTime());
		}
	}

	public void Clear()
	{
		_timerInterval = null;
		_typeDropdown.ClearItems();
	}

	public void SetDisplayStyle(bool visible)
	{
		_root.ToggleDisplayStyle(visible);
	}

	private void SetTime(ChangeEvent<float> time)
	{
		if (time.newValue >= 0f)
		{
			SetTimeInterval(time.newValue, _timerInterval.Type);
		}
		else
		{
			_timeField.SetValueWithoutNotify(_timerInterval.GetTypeTime());
		}
	}

	private void SetIntervalType(IntervalType intervalType)
	{
		SetTimeInterval(_timeField.value, intervalType);
	}

	private void SetTimeInterval(float time, IntervalType intervalType)
	{
		switch (intervalType)
		{
		case IntervalType.Ticks:
			_timerInterval.SetTicks((int)Math.Round(time));
			break;
		case IntervalType.Hours:
			_timerInterval.SetHours(time);
			break;
		case IntervalType.Days:
			_timerInterval.SetDays(time);
			break;
		default:
			throw new ArgumentOutOfRangeException("Type", _timerInterval.Type, null);
		}
		_timeField.SetValueWithoutNotify(_timerInterval.GetTypeTime());
	}
}
