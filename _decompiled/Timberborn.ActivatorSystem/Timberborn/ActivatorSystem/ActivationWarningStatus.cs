using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockingSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.ActivatorSystem;

public class ActivationWarningStatus : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly int CloseToActivationDayCount = 3;

	private static readonly int CloseToActivationHoursCount = 3;

	private static readonly float Step = 0.1f;

	private static readonly string StatusLocKey = "Status.TimedComponentActivator.Short";

	private readonly ILoc _loc;

	private StatusToggle _statusToggle;

	private ActivationWarningStatusSpec _activationWarningStatusSpec;

	private TimedComponentActivator _timedComponentActivator;

	private BlockableObject _blockableObject;

	public ActivationWarningStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_activationWarningStatusSpec = GetComponent<ActivationWarningStatusSpec>();
		LabeledEntitySpec component = GetComponent<LabeledEntitySpec>();
		_statusToggle = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon(_activationWarningStatusSpec.StatusSpriteName, _loc.T(_activationWarningStatusSpec.StatusLocKey), _loc.T(component.DisplayNameLocKey) + ": " + _loc.T(StatusLocKey));
		_timedComponentActivator = GetComponent<TimedComponentActivator>();
		if (!_timedComponentActivator.CountdownIsActive)
		{
			_timedComponentActivator.CountdownActivated += delegate
			{
				ActivateToggleIfPossible();
			};
		}
		if (!_timedComponentActivator.IsPastActivationTime)
		{
			_timedComponentActivator.Activated += delegate
			{
				_statusToggle.Deactivate();
			};
		}
		_blockableObject = GetComponent<BlockableObject>();
		if ((bool)_blockableObject)
		{
			_blockableObject.ObjectBlocked += OnObjectBlocked;
			_blockableObject.ObjectUnblocked += OnObjectUnblocked;
		}
	}

	public void InitializeEntity()
	{
		if (!_timedComponentActivator.IsPastActivationTime)
		{
			string warningSound = _activationWarningStatusSpec.WarningSound;
			GetComponent<StatusSubject>().RegisterDynamicStatus(_statusToggle, GetDaysLeftUntilActivation, GetStatusWarningType, warningSound);
			ActivateToggleIfPossible();
		}
	}

	public float GetDaysLeftUntilActivation()
	{
		float daysLeftUntilActivation = _timedComponentActivator.DaysLeftUntilActivation;
		if (!IsCloseToActivation())
		{
			return (float)Math.Ceiling(daysLeftUntilActivation);
		}
		float daysUntilActivation = _timedComponentActivator.DaysUntilActivation;
		return (float)Math.Ceiling((daysUntilActivation - daysUntilActivation * _timedComponentActivator.ActivationProgress) / Step) * Step;
	}

	public bool IsCloseToActivation()
	{
		return GetStatusWarningType() != StatusWarningType.None;
	}

	private StatusWarningType GetStatusWarningType()
	{
		if (_activationWarningStatusSpec.UseInfiniteWarning && _timedComponentActivator.DaysLeftUntilActivation * 24f <= (float)CloseToActivationHoursCount)
		{
			return StatusWarningType.Infinite;
		}
		if (!(_timedComponentActivator.DaysLeftUntilActivation <= (float)CloseToActivationDayCount))
		{
			return StatusWarningType.None;
		}
		return StatusWarningType.Short;
	}

	private void OnObjectUnblocked(object sender, EventArgs eventArgs)
	{
		ActivateToggleIfPossible();
	}

	private void OnObjectBlocked(object sender, EventArgs eventArgs)
	{
		_statusToggle.Deactivate();
	}

	private void ActivateToggleIfPossible()
	{
		if (_timedComponentActivator.CountdownIsActive)
		{
			BlockableObject blockableObject = _blockableObject;
			if (blockableObject == null || blockableObject.IsUnblocked)
			{
				_statusToggle.Activate();
			}
		}
	}
}
