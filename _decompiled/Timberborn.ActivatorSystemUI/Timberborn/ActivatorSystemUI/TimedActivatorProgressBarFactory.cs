using System;
using Timberborn.CoreUI;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.ActivatorSystemUI;

internal class TimedActivatorProgressBarFactory
{
	private readonly ILoc _loc;

	public TimedActivatorProgressBarFactory(ILoc loc)
	{
		_loc = loc;
	}

	public TimedActivatorProgressBar Create(VisualElement root, Func<float> progressGetter, Func<string> daysLeftGetter, Func<bool> countdownActiveGetter)
	{
		Label label = root.Q<Label>("Text");
		Timberborn.CoreUI.ProgressBar progressBar = root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		return new TimedActivatorProgressBar(_loc, label, progressBar, progressGetter, daysLeftGetter, countdownActiveGetter);
	}
}
