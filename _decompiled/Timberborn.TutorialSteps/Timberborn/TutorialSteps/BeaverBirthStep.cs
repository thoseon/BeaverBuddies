using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class BeaverBirthStep : ITutorialStep
{
	private readonly FirstbornService _firstbornService;

	private readonly string _description;

	public BeaverBirthStep(FirstbornService firstbornService, string description)
	{
		_firstbornService = firstbornService;
		_description = description;
	}

	public string Description()
	{
		return _description;
	}

	public bool Achieved()
	{
		return _firstbornService.FirstbornBorn;
	}
}
