using Timberborn.BlueprintSystem;
using Timberborn.Localization;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class BeaverBirthStepDeserializer : IStepDeserializer
{
	private static readonly string BeaverBirthLocKey = "Tutorial.MoreBeavers.BeaverBirth";

	private readonly FirstbornService _firstbornService;

	private readonly ILoc _loc;

	public BeaverBirthStepDeserializer(FirstbornService firstbornService, ILoc loc)
	{
		_firstbornService = firstbornService;
		_loc = loc;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is BeaverBirthStepSpec)
		{
			tutorialStep = Create();
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create()
	{
		return TutorialStep.Create(new BeaverBirthStep(_firstbornService, _loc.T(BeaverBirthLocKey)));
	}
}
