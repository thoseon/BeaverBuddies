using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;
using Timberborn.SelectionSystem;

namespace Timberborn.StatusSystem;

public class AlertStatusSubjectSelector
{
	private readonly IStatusAggregator _statusAggregator;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly Dictionary<string, StatusInstance> _previousSelectedStatuses = new Dictionary<string, StatusInstance>();

	public AlertStatusSubjectSelector(IStatusAggregator statusAggregator, EntitySelectionService entitySelectionService)
	{
		_statusAggregator = statusAggregator;
		_entitySelectionService = entitySelectionService;
	}

	public void SelectNextSubject(string alertDescription)
	{
		ImmutableArray<StatusInstance> visibleStatuses = _statusAggregator.GetVisibleStatuses(alertDescription);
		if (visibleStatuses.Length > 0)
		{
			StatusInstance orAdd = _previousSelectedStatuses.GetOrAdd(alertDescription, () => (StatusInstance)null);
			int num = visibleStatuses.IndexOf(orAdd);
			StatusInstance statusInstance = (ShouldShowFirst(num, visibleStatuses) ? visibleStatuses[0] : visibleStatuses[num + 1]);
			_previousSelectedStatuses[alertDescription] = statusInstance;
			_entitySelectionService.SelectAndFocusOn(statusInstance.StatusSubject);
		}
	}

	private static bool ShouldShowFirst(int previousIndex, ImmutableArray<StatusInstance> statuses)
	{
		if (previousIndex != -1)
		{
			return previousIndex == statuses.Length - 1;
		}
		return true;
	}
}
