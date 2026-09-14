using Timberborn.BlueprintSystem;
using Timberborn.Localization;
using Timberborn.TimeSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class SetPauseStepDeserializer : IStepDeserializer
{
	private readonly SpeedManager _speedManager;

	private readonly ILoc _loc;

	public SetPauseStepDeserializer(SpeedManager speedManager, ILoc loc)
	{
		_speedManager = speedManager;
		_loc = loc;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is SetPauseStepSpec setPauseStepSpec)
		{
			tutorialStep = Create(setPauseStepSpec.Pause, setPauseStepSpec.OnlyOnce);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(bool pause, bool onlyOnce)
	{
		string description = (pause ? _loc.T("Tutorial.Basics.Pause") : _loc.T("Tutorial.Basics.Unpause"));
		return TutorialStep.Create(new SetPauseStep(_speedManager, description, pause, onlyOnce), "Speed0");
	}
}
