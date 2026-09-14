using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlockObjectTools;
using Timberborn.BlueprintSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.TemplateSystem;
using Timberborn.ToolButtonSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class ConnectBuildingsTutorialStepDeserializer : IStepDeserializer
{
	private readonly BuiltBuildingService _builtBuildingService;

	private readonly BuildingService _buildingService;

	private readonly ILoc _loc;

	private readonly ToolButtonService _toolButtonService;

	public ConnectBuildingsTutorialStepDeserializer(BuiltBuildingService builtBuildingService, BuildingService buildingService, ILoc loc, ToolButtonService toolButtonService)
	{
		_builtBuildingService = builtBuildingService;
		_buildingService = buildingService;
		_loc = loc;
		_toolButtonService = toolButtonService;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is ConnectBuildingsTutorialStepSpec connectBuildingsTutorialStepSpec)
		{
			tutorialStep = Create(connectBuildingsTutorialStepSpec.TemplateName, connectBuildingsTutorialStepSpec.RequiredAmount, connectBuildingsTutorialStepSpec.CountUnfinishedBuildings, connectBuildingsTutorialStepSpec.HighlightableBuildingIds);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(string templateName, int requiredAmount, bool countUnfinishedBuildings, ImmutableArray<string> highlightableBuildingIds)
	{
		LabeledEntitySpec spec = _buildingService.GetBuildingTemplate(templateName).GetSpec<LabeledEntitySpec>();
		string localizedBuildingName = _loc.T(spec.DisplayNameLocKey);
		ImmutableArray<ToolButton> immutableArray = GetToolButtons(highlightableBuildingIds).ToImmutableArray();
		ToolGroupButton toolGroupButton = _toolButtonService.GetToolGroupButton(immutableArray.First());
		return TutorialStep.Create(new ConnectBuildingsTutorialStep(_builtBuildingService, _loc, templateName, requiredAmount, localizedBuildingName, countUnfinishedBuildings), toolGroupButton, immutableArray);
	}

	private IEnumerable<ToolButton> GetToolButtons(ImmutableArray<string> templateNames)
	{
		foreach (string templateName in templateNames)
		{
			yield return _toolButtonService.GetToolButton((BlockObjectTool tool) => tool.Template.GetSpec<TemplateSpec>().IsNamed(templateName));
		}
	}
}
