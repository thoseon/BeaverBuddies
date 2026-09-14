using System;

namespace Timberborn.StatusSystem;

public class StatusToggle
{
	public StatusSpecification StatusSpecification { get; }

	public bool IsPriorityStatus { get; }

	public bool IsNotifying { get; private set; }

	public bool IsActive { get; private set; }

	public event EventHandler<EventArgs> StatusToggled;

	private StatusToggle(StatusSpecification statusSpecification, bool isPriorityStatus)
	{
		StatusSpecification = statusSpecification;
		IsPriorityStatus = isPriorityStatus;
	}

	public static StatusToggle CreateNormalStatus(string spriteName, string description)
	{
		return new StatusToggle(StatusSpecification.Create(spriteName, description), isPriorityStatus: false);
	}

	public static StatusToggle CreatePriorityStatus(string spriteName, string description)
	{
		return new StatusToggle(StatusSpecification.Create(spriteName, description), isPriorityStatus: true);
	}

	public static StatusToggle CreatePriorityStatusWithFloatingIcon(string spriteName, string description, float delayInHours = 0f)
	{
		return new StatusToggle(StatusSpecification.CreateWithIcon(spriteName, description, delayInHours), isPriorityStatus: true);
	}

	public static StatusToggle CreatePriorityStatusWithAlertAndFloatingIcon(string spriteName, string statusDescription, string alertDescription, float delayInHours = 0f)
	{
		return new StatusToggle(StatusSpecification.CreateWithAlertAndIcon(spriteName, statusDescription, alertDescription, delayInHours), isPriorityStatus: true);
	}

	public static StatusToggle CreateNormalStatusWithFloatingIcon(string spriteName, string description, float delayInHours = 0f)
	{
		return new StatusToggle(StatusSpecification.CreateWithIcon(spriteName, description, delayInHours), isPriorityStatus: false);
	}

	public static StatusToggle CreateNormalStatusWithAlert(string spriteName, string statusDescription, string alertDescription, float delayInHours = 0f)
	{
		return new StatusToggle(StatusSpecification.CreateWithAlert(spriteName, statusDescription, alertDescription, delayInHours), isPriorityStatus: false);
	}

	public static StatusToggle CreateNormalStatusWithAlertAndFloatingIcon(string spriteName, string statusDescription, string alertDescription, float delayInHours = 0f)
	{
		return new StatusToggle(StatusSpecification.CreateWithAlertAndIcon(spriteName, statusDescription, alertDescription, delayInHours), isPriorityStatus: false);
	}

	public void Activate()
	{
		Toggle(isActive: true);
	}

	public void Deactivate()
	{
		Toggle(isActive: false);
	}

	public void Toggle(bool isActive)
	{
		if (IsActive != isActive)
		{
			IsActive = isActive;
			StatusToggled?.Invoke(this, EventArgs.Empty);
		}
	}

	public StatusToggle AsNotifying()
	{
		IsNotifying = true;
		return this;
	}
}
