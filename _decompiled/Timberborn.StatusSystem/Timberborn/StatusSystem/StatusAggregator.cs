using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.SingletonSystem;

namespace Timberborn.StatusSystem;

public class StatusAggregator : IUpdatableSingleton, IStatusAggregator
{
	private readonly EventBus _eventBus;

	private readonly List<StatusInstance> _statuses = new List<StatusInstance>();

	private readonly Dictionary<string, List<StatusInstance>> _visibleStatuses = new Dictionary<string, List<StatusInstance>>();

	public StatusAggregator(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void UpdateSingleton()
	{
		UpdateVisibleStatuses();
	}

	public void AddStatus(StatusInstance statusInstance)
	{
		if (statusInstance.ShowAlert)
		{
			_statuses.Add(statusInstance);
			string alertDescription = statusInstance.AlertDescription;
			if (!_visibleStatuses.ContainsKey(alertDescription))
			{
				_visibleStatuses[alertDescription] = new List<StatusInstance>();
				_eventBus.Post(new StatusAlertAddedEvent(alertDescription, statusInstance.IconSmall));
			}
		}
	}

	public void RemoveStatuses(StatusSubject statusSubject)
	{
		_statuses.RemoveAll((StatusInstance instance) => instance.StatusSubject == statusSubject);
		UpdateVisibleStatuses();
	}

	public ImmutableArray<StatusInstance> GetVisibleStatuses(string alertDescription)
	{
		return _visibleStatuses[alertDescription].ToImmutableArray();
	}

	public int GetVisibleStatusesCount(string alertDescription)
	{
		return _visibleStatuses[alertDescription].Count;
	}

	private void UpdateVisibleStatuses()
	{
		foreach (string key in _visibleStatuses.Keys)
		{
			_visibleStatuses[key].Clear();
		}
		foreach (StatusInstance status in _statuses)
		{
			if (IsVisible(status))
			{
				_visibleStatuses[status.AlertDescription].Add(status);
			}
		}
	}

	private static bool IsVisible(StatusInstance statusInstance)
	{
		if (statusInstance.IsActive)
		{
			return statusInstance.IsVisible();
		}
		return false;
	}
}
