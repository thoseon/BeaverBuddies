using Timberborn.CoreUI;
using Timberborn.TutorialSystem;
using Timberborn.WellbeingUI;

namespace Timberborn.TutorialSteps;

internal class OpenWellbeingPanelStep : ITutorialStep
{
	private readonly PanelStack _panelStack;

	private readonly PopulationWellbeingBox _populationWellbeingBox;

	private readonly string _description;

	private bool _wasAchieved;

	public OpenWellbeingPanelStep(PanelStack panelStack, PopulationWellbeingBox populationWellbeingBox, string description)
	{
		_panelStack = panelStack;
		_populationWellbeingBox = populationWellbeingBox;
		_description = description;
	}

	public string Description()
	{
		return _description;
	}

	public bool Achieved()
	{
		_wasAchieved |= _panelStack.IsPanelOnTop(_populationWellbeingBox);
		return _wasAchieved;
	}
}
