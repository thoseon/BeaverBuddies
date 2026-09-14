using System.Collections.Generic;
using Timberborn.AlertPanelSystem;
using Timberborn.GameSound;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.StatusSystem;
using UnityEngine.UIElements;

namespace Timberborn.StatusSystemUI;

internal class DynamicStatusAlertFragment : IAlertFragment
{
	private readonly DynamicStatusAggregator _dynamicStatusAggregator;

	private readonly GameUISoundController _gameUISoundController;

	private readonly StatusAlertRowBlinker _statusAlertRowBlinker;

	private readonly EventBus _eventBus;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly StatusAlertFragmentRowFactory _statusAlertFragmentRowFactory;

	private AlertStatusSubjectSelector _alertStatusSubjectSelector;

	private readonly Dictionary<string, StatusAlertFragmentRow> _rows = new Dictionary<string, StatusAlertFragmentRow>();

	private readonly Dictionary<string, float> _blinkingRow = new Dictionary<string, float>();

	private readonly Dictionary<string, float> _infiniteBlinkingRow = new Dictionary<string, float>();

	private VisualElement _root;

	public DynamicStatusAlertFragment(DynamicStatusAggregator dynamicStatusAggregator, GameUISoundController gameUISoundController, StatusAlertRowBlinker statusAlertRowBlinker, EventBus eventBus, EntitySelectionService entitySelectionService, StatusAlertFragmentRowFactory statusAlertFragmentRowFactory)
	{
		_dynamicStatusAggregator = dynamicStatusAggregator;
		_gameUISoundController = gameUISoundController;
		_statusAlertRowBlinker = statusAlertRowBlinker;
		_eventBus = eventBus;
		_entitySelectionService = entitySelectionService;
		_statusAlertFragmentRowFactory = statusAlertFragmentRowFactory;
	}

	public void InitializeAlertFragment(VisualElement root)
	{
		_root = new VisualElement
		{
			name = "DynamicStatusAlertFragment"
		};
		root.Add(_root);
		_alertStatusSubjectSelector = new AlertStatusSubjectSelector(_dynamicStatusAggregator, _entitySelectionService);
		_eventBus.Register(this);
	}

	public void UpdateAlertFragment()
	{
		foreach (StatusAlertFragmentRow value in _rows.Values)
		{
			string alertDescription = value.AlertDescription;
			if (_dynamicStatusAggregator.TryGetStatusData(alertDescription, out var statusData))
			{
				string text = ((statusData.StatusWarningType != StatusWarningType.None) ? "F1" : "F0");
				value.UpdateRowState(statusData.Count, statusData.Value.ToString(text));
				UpdateBlinkingState(statusData, alertDescription, value);
			}
			else
			{
				value.UpdateRowState(0);
				_statusAlertRowBlinker.StopBlinking(value);
			}
		}
	}

	[OnEvent]
	public void OnStatusAlertAddedEvent(DynamicStatusAlertAddedEvent statusAlertAddedEvent)
	{
		StatusInstance statusInstance = statusAlertAddedEvent.StatusInstance;
		StatusAlertFragmentRow statusAlertFragmentRow = _statusAlertFragmentRowFactory.Create(statusInstance.AlertDescription, statusInstance.IconSmall, _alertStatusSubjectSelector, statusInstance.WarningSound);
		_rows.Add(statusInstance.AlertDescription, statusAlertFragmentRow);
		_root.Add(statusAlertFragmentRow.Root);
	}

	[OnEvent]
	public void OnPinnedStatusChanged(NotifyingStatusChangedEvent notifyingStatusChangedEvent)
	{
		if (_rows.TryGetValue(notifyingStatusChangedEvent.StatusAlert, out var value))
		{
			_statusAlertRowBlinker.StartShortBlinking(value);
		}
	}

	private void UpdateBlinkingState(StatusData statusData, string alertDescription, StatusAlertFragmentRow row)
	{
		switch (statusData.StatusWarningType)
		{
		case StatusWarningType.Short:
			if (ShouldStartBlinking(alertDescription, statusData.Value, _blinkingRow))
			{
				PlaySound(row.WarningSound);
				_statusAlertRowBlinker.StartShortBlinking(row);
			}
			_blinkingRow[alertDescription] = statusData.Value;
			break;
		case StatusWarningType.Infinite:
			if (ShouldStartBlinking(alertDescription, statusData.Value, _infiniteBlinkingRow))
			{
				PlaySound(row.WarningSound);
				_statusAlertRowBlinker.StartInfiniteBlinking(row);
			}
			_infiniteBlinkingRow[alertDescription] = statusData.Value;
			break;
		case StatusWarningType.None:
			_statusAlertRowBlinker.StopBlinking(row);
			break;
		}
	}

	private static bool ShouldStartBlinking(string alertDescription, float value, Dictionary<string, float> blinkingRow)
	{
		if (blinkingRow.TryGetValue(alertDescription, out var value2))
		{
			return value > value2;
		}
		return true;
	}

	private void PlaySound(string warningSound)
	{
		if (!string.IsNullOrEmpty(warningSound))
		{
			_gameUISoundController.PlaySound2D(warningSound);
		}
	}
}
