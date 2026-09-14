using Timberborn.Forestry;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class MarkTreesTutorialStep : ITutorialStep
{
	private readonly TreeCuttingArea _treeCuttingArea;

	private readonly string _description;

	public MarkTreesTutorialStep(TreeCuttingArea treeCuttingArea, string description)
	{
		_treeCuttingArea = treeCuttingArea;
		_description = description;
	}

	public string Description()
	{
		return _description;
	}

	public bool Achieved()
	{
		return _treeCuttingArea.AnyYielderSelected;
	}
}
