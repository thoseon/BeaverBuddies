using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;

namespace Timberborn.StatusSystem;

public class StatusSubject : BaseComponent, IInitializableEntity, IDeletableEntity
{
	private readonly StatusInstanceFactory _statusInstanceFactory;

	private readonly DynamicStatusAggregator _dynamicStatusAggregator;

	private readonly StatusAggregator _statusAggregator;

	private readonly NotifyingStatusMonitor _notifyingStatusMonitor;

	private readonly List<StatusInstance> _activePriorityStatuses = new List<StatusInstance>();

	private readonly List<StatusInstance> _activeNormalStatuses = new List<StatusInstance>();

	public ReadOnlyList<StatusInstance> ActiveStatuses
	{
		get
		{
			if (!InPriorityMode)
			{
				return _activeNormalStatuses.AsReadOnlyList();
			}
			return _activePriorityStatuses.AsReadOnlyList();
		}
	}

	public bool InPriorityMode => !_activePriorityStatuses.IsEmpty();

	public event EventHandler<StatusInstance> StatusToggled;

	internal StatusSubject(StatusInstanceFactory statusInstanceFactory, DynamicStatusAggregator dynamicStatusAggregator, StatusAggregator statusAggregator, NotifyingStatusMonitor notifyingStatusMonitor)
	{
		_statusInstanceFactory = statusInstanceFactory;
		_dynamicStatusAggregator = dynamicStatusAggregator;
		_statusAggregator = statusAggregator;
		_notifyingStatusMonitor = notifyingStatusMonitor;
	}

	public void RegisterStatuses(IEnumerable<StatusToggle> statusToggles)
	{
		foreach (StatusToggle statusToggle in statusToggles)
		{
			RegisterStatus(statusToggle);
		}
	}

	public void RegisterStatus(StatusToggle statusToggle)
	{
		StatusInstance statusInstance = _statusInstanceFactory.CreateStatus(this, statusToggle);
		_statusAggregator.AddStatus(statusInstance);
		UpdateStatus(statusToggle, statusInstance);
	}

	public void RegisterDynamicStatus(StatusToggle statusToggle, Func<float> statusGroupOrderingGetter, Func<StatusWarningType> statusWarningTypeGetter, string warningSound)
	{
		StatusInstance statusInstance = _statusInstanceFactory.CreateDynamicStatus(this, statusToggle, statusGroupOrderingGetter, statusWarningTypeGetter, warningSound);
		_dynamicStatusAggregator.AddStatus(statusInstance);
		UpdateStatus(statusToggle, statusInstance);
	}

	public void InitializeEntity()
	{
		_notifyingStatusMonitor.AddSubject(this);
	}

	public void DeleteEntity()
	{
		_notifyingStatusMonitor.RemoveSubject(this);
		_statusAggregator.RemoveStatuses(this);
		_dynamicStatusAggregator.RemoveStatuses(this);
	}

	private void UpdateStatus(StatusToggle statusToggle, StatusInstance statusInstance)
	{
		UpdateStatus(statusInstance, statusToggle.IsActive);
		statusToggle.StatusToggled += delegate
		{
			UpdateStatus(statusInstance, statusToggle.IsActive);
		};
	}

	private void UpdateStatus(StatusInstance statusInstance, bool isActive)
	{
		List<StatusInstance> list = (statusInstance.IsPriorityStatus ? _activePriorityStatuses : _activeNormalStatuses);
		if (isActive)
		{
			statusInstance.Activate();
			if (!list.Contains(statusInstance))
			{
				list.Add(statusInstance);
			}
		}
		else
		{
			statusInstance.Deactivate();
			list.Remove(statusInstance);
		}
		StatusToggled?.Invoke(this, statusInstance);
	}
}
