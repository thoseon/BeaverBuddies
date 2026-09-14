using Timberborn.BlueprintSystem;
using Timberborn.Localization;
using Timberborn.TutorialSystem;
using Timberborn.WorkSystem;
using Timberborn.WorkSystemUI;

namespace Timberborn.TutorialSteps;

internal class SetWorkingHoursStepDeserializer : IStepDeserializer
{
	private readonly WorkingHoursManager _workingHoursManager;

	private readonly WorkingHoursPanel _workingHoursPanel;

	private readonly ILoc _loc;

	public SetWorkingHoursStepDeserializer(WorkingHoursManager workingHoursManager, WorkingHoursPanel workingHoursPanel, ILoc loc)
	{
		_workingHoursManager = workingHoursManager;
		_workingHoursPanel = workingHoursPanel;
		_loc = loc;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is SetWorkingHoursStepSpec setWorkingHoursStepSpec)
		{
			tutorialStep = Create(setWorkingHoursStepSpec.TargetWorkingHours);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(int targetWorkingHours)
	{
		string description = _loc.T("Tutorial.SetWorkingHours", targetWorkingHours);
		return TutorialStep.Create(new SetWorkingHoursStep(_workingHoursManager, targetWorkingHours, description), delegate(bool state)
		{
			_workingHoursPanel.TogglePanelHighlight(state);
		});
	}
}
