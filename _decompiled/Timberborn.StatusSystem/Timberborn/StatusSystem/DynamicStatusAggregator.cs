using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.SingletonSystem;

namespace Timberborn.StatusSystem;

public class DynamicStatusAggregator : IUpdatableSingleton, IStatusAggregator
{
	private readonly EventBus _eventBus;

	private readonly Dictionary<string, List<StatusInstance>> _allStatuses = new Dictionary<string, List<StatusInstance>>();

	private readonly Dictionary<string, List<StatusInstance>> _visibleStatuses = new Dictionary<string, List<StatusInstance>>();

	public DynamicStatusAggregator(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void AddStatus(StatusInstance statusInstance)
	{
		if (statusInstance.ShowAlert)
		{
			string alertDescription = statusInstance.AlertDescription;
			if (!_visibleStatuses.ContainsKey(alertDescription))
			{
				_visibleStatuses[alertDescription] = new List<StatusInstance>();
				_allStatuses[alertDescription] = new List<StatusInstance>();
				_eventBus.Post(new DynamicStatusAlertAddedEvent(statusInstance));
			}
			_allStatuses[alertDescription].Add(statusInstance);
			UpdateVisibleStatuses(alertDescription);
		}
	}

	public void RemoveStatuses(StatusSubject statusSubject)
	{
		foreach (string key in _allStatuses.Keys)
		{
			_allStatuses[key].RemoveAll((StatusInstance instance) => instance.StatusSubject == statusSubject);
			UpdateVisibleStatuses(key);
		}
	}

	public void UpdateSingleton()
	{
		foreach (string key in _allStatuses.Keys)
		{
			UpdateVisibleStatuses(key);
		}
	}

	public ImmutableArray<StatusInstance> GetVisibleStatuses(string alertDescription)
	{
		return _visibleStatuses[alertDescription].ToImmutableArray();
	}

	public bool TryGetStatusData(string alertDescription, out StatusData statusData)
	{
		if (_visibleStatuses.TryGetValue(alertDescription, out var value) && value.Count > 0)
		{
			StatusInstance statusInstance = value[0];
			statusData = new StatusData(value.Count, statusInstance.StatusValueGetter(), statusInstance.StatusWarningTypeGetter());
			return true;
		}
		statusData = default(StatusData);
		return false;
	}

	private void UpdateVisibleStatuses(string key)
	{
		List<StatusInstance> list = _visibleStatuses[key];
		list.Clear();
		List<StatusInstance> list2 = _allStatuses[key];
		float num = float.MaxValue;
		foreach (StatusInstance item in list2)
		{
			if (!IsVisible(item))
			{
				continue;
			}
			float num2 = item.StatusValueGetter();
			if (num2 <= num)
			{
				if (num2 < num)
				{
					list.Clear();
					num = num2;
				}
				list.Add(item);
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
