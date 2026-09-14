using System;
using Timberborn.BlueprintSystem;
using Timberborn.LevelVisibilitySystemUI;
using Timberborn.Localization;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class VisibleLevelChangeStepDeserializer : IStepDeserializer
{
	private readonly ILoc _loc;

	private readonly VisibleLevelChangeService _visibleLevelChangeService;

	private readonly ILevelVisibilityPanel _levelVisibilityPanel;

	public VisibleLevelChangeStepDeserializer(ILoc loc, VisibleLevelChangeService visibleLevelChangeService, ILevelVisibilityPanel levelVisibilityPanel)
	{
		_loc = loc;
		_visibleLevelChangeService = visibleLevelChangeService;
		_levelVisibilityPanel = levelVisibilityPanel;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is VisibleLevelChangeStepSpec visibleLevelChangeStepSpec)
		{
			tutorialStep = Create(visibleLevelChangeStepSpec.VisibleLevelChangeType, visibleLevelChangeStepSpec.ShowKeybindings);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(VisibleLevelChangeType visibleLevelChangeType, bool showKeybindings)
	{
		return TutorialStep.Create(new VisibleLevelChangeStep(_visibleLevelChangeService, _loc.T(GetLocKey(visibleLevelChangeType)), visibleLevelChangeType), keyBinding: showKeybindings ? GetKeybindingLocKey(visibleLevelChangeType) : null, highlight: delegate(bool state)
		{
			_levelVisibilityPanel.TogglePanelHighlight(state);
		});
	}

	private static string GetLocKey(VisibleLevelChangeType visibleLevelChangeType)
	{
		return visibleLevelChangeType switch
		{
			VisibleLevelChangeType.Decrease => "Tutorial.LayerTool.Decrease", 
			VisibleLevelChangeType.Increase => "Tutorial.LayerTool.Increase", 
			VisibleLevelChangeType.Reset => "Tutorial.LayerTool.Reset", 
			_ => throw new ArgumentOutOfRangeException("visibleLevelChangeType", visibleLevelChangeType, null), 
		};
	}

	private static string GetKeybindingLocKey(VisibleLevelChangeType visibleLevelChangeType)
	{
		return visibleLevelChangeType switch
		{
			VisibleLevelChangeType.Decrease => "LowerVisibleLayer", 
			VisibleLevelChangeType.Increase => "RaiseVisibleLayer", 
			VisibleLevelChangeType.Reset => null, 
			_ => throw new ArgumentOutOfRangeException("visibleLevelChangeType", visibleLevelChangeType, null), 
		};
	}
}
