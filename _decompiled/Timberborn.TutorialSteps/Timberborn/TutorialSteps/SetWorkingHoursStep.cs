using System;
using Timberborn.TutorialSystem;
using Timberborn.WorkSystem;

namespace Timberborn.TutorialSteps;

internal class SetWorkingHoursStep : ITutorialStep
{
	private readonly WorkingHoursManager _workingHoursManager;

	private readonly int _targetWorkingHours;

	private readonly string _description;

	public SetWorkingHoursStep(WorkingHoursManager workingHoursManager, int targetWorkingHours, string description)
	{
		_workingHoursManager = workingHoursManager;
		_targetWorkingHours = targetWorkingHours;
		_description = description;
	}

	public string Description()
	{
		return _description;
	}

	public bool Achieved()
	{
		return Math.Abs(_workingHoursManager.EndHours - (float)_targetWorkingHours) < 0.01f;
	}
}
