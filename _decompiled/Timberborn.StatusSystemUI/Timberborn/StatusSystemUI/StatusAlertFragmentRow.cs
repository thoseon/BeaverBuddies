using Timberborn.CoreUI;
using Timberborn.StatusSystem;
using UnityEngine.UIElements;

namespace Timberborn.StatusSystemUI;

internal class StatusAlertFragmentRow
{
	private static readonly string BlinkingClass = "alert-panel-row--blink";

	private readonly AlertStatusSubjectSelector _alertStatusSubjectSelector;

	private readonly Button _statusButton;

	private int _previousCount;

	private string _previousValue;

	private bool _highlightActive;

	public string AlertDescription { get; }

	public string WarningSound { get; }

	public VisualElement Root { get; }

	public StatusAlertFragmentRow(AlertStatusSubjectSelector alertStatusSubjectSelector, string alertDescription, string warningSound, VisualElement root, Button statusButton)
	{
		_alertStatusSubjectSelector = alertStatusSubjectSelector;
		AlertDescription = alertDescription;
		WarningSound = warningSound;
		Root = root;
		_statusButton = statusButton;
	}

	public void Initialize()
	{
		_statusButton.RegisterCallback<ClickEvent>(SelectNext);
	}

	public void UpdateRowState(int count, string value = null)
	{
		if (_previousCount != count || _previousValue != value)
		{
			_statusButton.text = GetAlertText(count, value);
			Root.ToggleDisplayStyle(count > 0);
			_previousCount = count;
			_previousValue = value;
		}
	}

	public void ToggleHighlight()
	{
		ChangeHighlightState(!_highlightActive);
	}

	public void DisableHighlight()
	{
		ChangeHighlightState(active: false);
	}

	private void SelectNext(ClickEvent evt)
	{
		_alertStatusSubjectSelector.SelectNextSubject(AlertDescription);
	}

	private string GetAlertText(int count, string value)
	{
		string text = ((value != null) ? string.Format(AlertDescription, value) : AlertDescription);
		string text2;
		if (count <= 1)
		{
			text2 = text;
			if (text2 == null)
			{
				return "";
			}
		}
		else
		{
			text2 = $"{text} ({count})";
		}
		return text2;
	}

	private void ChangeHighlightState(bool active)
	{
		_highlightActive = active;
		Root.EnableInClassList(BlinkingClass, active);
	}
}
