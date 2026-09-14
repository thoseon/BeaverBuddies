using Timberborn.AlertPanelSystem;
using Timberborn.StatusSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.StatusSystemUI;

internal class StatusAlertFragmentRowFactory
{
	private readonly AlertPanelRowFactory _alertPanelRowFactory;

	public StatusAlertFragmentRowFactory(AlertPanelRowFactory alertPanelRowFactory)
	{
		_alertPanelRowFactory = alertPanelRowFactory;
	}

	public StatusAlertFragmentRow Create(string alert, Sprite sprite, AlertStatusSubjectSelector subjectSelector, string warningSound = null)
	{
		VisualElement visualElement = _alertPanelRowFactory.Create(sprite);
		visualElement.Q<Image>("Icon").sprite = sprite;
		Button statusButton = visualElement.Q<Button>("Button");
		StatusAlertFragmentRow statusAlertFragmentRow = new StatusAlertFragmentRow(subjectSelector, alert, warningSound, visualElement, statusButton);
		statusAlertFragmentRow.Initialize();
		return statusAlertFragmentRow;
	}
}
