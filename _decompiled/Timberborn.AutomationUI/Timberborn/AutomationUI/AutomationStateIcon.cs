using System;
using Timberborn.Automation;
using Timberborn.Illumination;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.AutomationUI;

public class AutomationStateIcon
{
	private static readonly string StateOnClass = "automation-state-icon--on";

	private static readonly string StateUnfinishedClass = "automation-state-icon--unfinished";

	private readonly Func<Automator> _automatorGetter;

	private readonly Image _icon;

	public AutomationStateIcon(Func<Automator> automatorGetter, Image icon)
	{
		_automatorGetter = automatorGetter;
		_icon = icon;
	}

	public void Update()
	{
		Automator automator = _automatorGetter();
		if (automator != null)
		{
			_icon.visible = true;
			_icon.EnableInClassList(StateOnClass, automator.UnfinishedState == AutomatorState.On);
			_icon.EnableInClassList(StateUnfinishedClass, !automator.Enabled);
			_icon.style.unityBackgroundImageTintColor = GetColor(automator);
		}
		else
		{
			_icon.visible = false;
		}
	}

	private Color GetColor(Automator automator)
	{
		return automator.GetComponent<CustomizableIlluminator>().IconColor;
	}
}
