using System.Collections.Generic;
using Timberborn.AlertPanelSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.StatusSystem;
using UnityEngine.UIElements;

namespace Timberborn.StatusSystemUI;

internal class StatusAlertFragment : IAlertFragment
{
	private readonly StatusAggregator _statusAggregator;

	private readonly EventBus _eventBus;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly StatusAlertFragmentRowFactory _statusAlertFragmentRowFactory;

	private AlertStatusSubjectSelector _alertStatusSubjectSelector;

	private readonly List<StatusAlertFragmentRow> _rows = new List<StatusAlertFragmentRow>();

	private VisualElement _root;

	public StatusAlertFragment(StatusAggregator statusAggregator, EventBus eventBus, EntitySelectionService entitySelectionService, StatusAlertFragmentRowFactory statusAlertFragmentRowFactory)
	{
		_statusAggregator = statusAggregator;
		_eventBus = eventBus;
		_entitySelectionService = entitySelectionService;
		_statusAlertFragmentRowFactory = statusAlertFragmentRowFactory;
	}

	public void InitializeAlertFragment(VisualElement root)
	{
		_root = new VisualElement
		{
			name = "StatusAlertFragment"
		};
		root.Add(_root);
		_alertStatusSubjectSelector = new AlertStatusSubjectSelector(_statusAggregator, _entitySelectionService);
		_eventBus.Register(this);
	}

	public void UpdateAlertFragment()
	{
		foreach (StatusAlertFragmentRow row in _rows)
		{
			int visibleStatusesCount = _statusAggregator.GetVisibleStatusesCount(row.AlertDescription);
			row.UpdateRowState(visibleStatusesCount);
		}
	}

	[OnEvent]
	public void OnStatusAlertAddedEvent(StatusAlertAddedEvent statusAlertAddedEvent)
	{
		StatusAlertFragmentRow statusAlertFragmentRow = _statusAlertFragmentRowFactory.Create(statusAlertAddedEvent.StatusAlert, statusAlertAddedEvent.StatusSprite, _alertStatusSubjectSelector);
		_rows.Add(statusAlertFragmentRow);
		_root.Add(statusAlertFragmentRow.Root);
	}
}
