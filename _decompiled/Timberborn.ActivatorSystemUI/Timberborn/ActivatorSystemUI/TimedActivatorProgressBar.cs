using System;
using Timberborn.CoreUI;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.ActivatorSystemUI;

internal class TimedActivatorProgressBar
{
	private static readonly string HazardousActivatorUssClass = "progress-bar--red";

	private readonly ILoc _loc;

	private readonly Label _label;

	private readonly Timberborn.CoreUI.ProgressBar _progressBar;

	private readonly Func<float> _progressGetter;

	private readonly Func<string> _daysLeftGetter;

	private readonly Func<bool> _countdownActiveGetter;

	private string _progressActiveLabelLocKey;

	private string _progressNotActiveLabelLocKey;

	public TimedActivatorProgressBar(ILoc loc, Label label, Timberborn.CoreUI.ProgressBar progressBar, Func<float> progressGetter, Func<string> daysLeftGetter, Func<bool> countdownActiveGetter)
	{
		_loc = loc;
		_label = label;
		_progressBar = progressBar;
		_progressGetter = progressGetter;
		_daysLeftGetter = daysLeftGetter;
		_countdownActiveGetter = countdownActiveGetter;
	}

	public void Initialize(string progressActiveLabelLocKey, string progressNotActiveLabelLocKey, bool isHazardousActivator)
	{
		_progressActiveLabelLocKey = progressActiveLabelLocKey;
		_progressNotActiveLabelLocKey = progressNotActiveLabelLocKey;
		_progressBar.EnableInClassList(HazardousActivatorUssClass, isHazardousActivator);
	}

	public void UpdateState()
	{
		if (_countdownActiveGetter())
		{
			_progressBar.SetProgress(_progressGetter());
			_label.text = _loc.T(_progressActiveLabelLocKey, _daysLeftGetter());
		}
		else
		{
			_label.text = _loc.T(_progressNotActiveLabelLocKey);
			_progressBar.SetProgress(0f);
		}
	}
}
