using System;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.StatusSystem;

public class StatusInstance
{
	private readonly IDayNightCycle _dayNightCycle;

	private readonly float _showDelayInDays;

	private float _lastInactiveTimestamp;

	public string StatusDescription { get; }

	public string AlertDescription { get; }

	public bool IsPriorityStatus { get; }

	public bool IsNotifying { get; }

	public bool ShowFloatingIcon { get; }

	public StatusSubject StatusSubject { get; }

	public Sprite IconLarge { get; }

	public Sprite IconSmall { get; }

	public Func<float> StatusValueGetter { get; }

	public Func<StatusWarningType> StatusWarningTypeGetter { get; }

	public string WarningSound { get; }

	public bool IsActive { get; private set; }

	public bool ShowAlert => !string.IsNullOrEmpty(AlertDescription);

	private bool IsOverriden
	{
		get
		{
			if (!IsPriorityStatus)
			{
				return StatusSubject.InPriorityMode;
			}
			return false;
		}
	}

	public StatusInstance(string statusDescription, string alertDescription, bool isPriorityStatus, bool isNotifying, bool showFloatingIcon, StatusSubject statusSubject, Sprite iconLarge, Sprite iconSmall, Func<float> statusValueGetter, Func<StatusWarningType> statusWarningTypeGetter, string warningSound, IDayNightCycle dayNightCycle, float showDelayInHours)
	{
		StatusDescription = statusDescription;
		AlertDescription = alertDescription;
		IsPriorityStatus = isPriorityStatus;
		IsNotifying = isNotifying;
		ShowFloatingIcon = showFloatingIcon;
		StatusSubject = statusSubject;
		IconLarge = iconLarge;
		IconSmall = iconSmall;
		StatusValueGetter = statusValueGetter;
		StatusWarningTypeGetter = statusWarningTypeGetter;
		WarningSound = warningSound;
		_dayNightCycle = dayNightCycle;
		_showDelayInDays = showDelayInHours / 24f;
	}

	public void Activate()
	{
		if (!IsActive)
		{
			_lastInactiveTimestamp = _dayNightCycle.PartialDayNumber;
		}
		IsActive = true;
	}

	public void Deactivate()
	{
		IsActive = false;
	}

	public bool IsVisible()
	{
		bool flag = IsActive && !IsOverriden;
		if ((_showDelayInDays > 0f) & flag)
		{
			return _dayNightCycle.PartialDayNumber - _lastInactiveTimestamp > _showDelayInDays;
		}
		return flag;
	}
}
