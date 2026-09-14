using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.TutorialSystem;
using Timberborn.WellbeingUI;

namespace Timberborn.TutorialSteps;

internal class OpenWellbeingPanelStepDeserializer : IStepDeserializer
{
	private static readonly string OpenWellbeingBoxKey = "OpenWellbeingBox";

	private static readonly string DescriptionLocKey = "Tutorial.Wellbeing.OpenWellbeingPanel";

	private readonly PanelStack _panelStack;

	private readonly BasicStatisticsPanel _basicStatisticsPanel;

	private readonly PopulationWellbeingBox _populationWellbeingBox;

	private readonly ILoc _loc;

	public OpenWellbeingPanelStepDeserializer(PanelStack panelStack, BasicStatisticsPanel basicStatisticsPanel, PopulationWellbeingBox populationWellbeingBox, ILoc loc)
	{
		_panelStack = panelStack;
		_basicStatisticsPanel = basicStatisticsPanel;
		_populationWellbeingBox = populationWellbeingBox;
		_loc = loc;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is OpenWellbeingPanelStepSpec)
		{
			tutorialStep = Create();
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create()
	{
		string description = _loc.T(DescriptionLocKey);
		return TutorialStep.Create(new OpenWellbeingPanelStep(_panelStack, _populationWellbeingBox, description), delegate(bool state)
		{
			_basicStatisticsPanel.ToggleWellbeingButtonHighlight(state);
		}, OpenWellbeingBoxKey);
	}
}
