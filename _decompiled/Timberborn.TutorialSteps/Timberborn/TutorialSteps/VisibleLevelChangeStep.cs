using System;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class VisibleLevelChangeStep : ITutorialStep
{
	private readonly VisibleLevelChangeService _visibleLevelChangeService;

	private readonly string _description;

	private readonly VisibleLevelChangeType _visibleLevelChangeType;

	public VisibleLevelChangeStep(VisibleLevelChangeService visibleLevelChangeService, string description, VisibleLevelChangeType visibleLevelChangeType)
	{
		_visibleLevelChangeService = visibleLevelChangeService;
		_description = description;
		_visibleLevelChangeType = visibleLevelChangeType;
	}

	public string Description()
	{
		return _description;
	}

	public bool Achieved()
	{
		return _visibleLevelChangeType switch
		{
			VisibleLevelChangeType.Decrease => _visibleLevelChangeService.WasAtZero, 
			VisibleLevelChangeType.Increase => _visibleLevelChangeService.LevelsIncreasedSinceZero > 0, 
			VisibleLevelChangeType.Reset => _visibleLevelChangeService.WasAtZero && _visibleLevelChangeService.IsAtMax, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
