using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.TitleScreenUI;

internal class MacOsRosettaWarningInitializer : ILoadableSingleton
{
	private static readonly string RosettaOnLocKey = "MainMenu.RosettaOn";

	private static readonly string RosettaPerformanceLocKey = "MainMenu.RosettaPerformanceWarning";

	private readonly ILoc _loc;

	private readonly TitleScreenFooter _titleScreenFooter;

	public MacOsRosettaWarningInitializer(ILoc loc, TitleScreenFooter titleScreenFooter)
	{
		_loc = loc;
		_titleScreenFooter = titleScreenFooter;
	}

	public void Load()
	{
		VisualElement visualElement = _titleScreenFooter.Root.Q<VisualElement>("MacOsRosettaWarningContainer");
		if (ProcessorInfo.IsAppleCpu() && ProcessorInfo.IsIntelProcess())
		{
			visualElement.Q<Label>("MacOsRosettaWarning").text = GetWarningText();
		}
		else
		{
			visualElement.ToggleDisplayStyle(visible: false);
		}
	}

	private string GetWarningText()
	{
		return _loc.T(RosettaOnLocKey) + " " + _loc.T(RosettaPerformanceLocKey);
	}
}
