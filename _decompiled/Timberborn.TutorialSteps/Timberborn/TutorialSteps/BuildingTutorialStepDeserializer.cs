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

internal class BuildingTutorialStepDeserializer : IStepDeserializer
{
	private readonly BuiltBuildingService _builtBuildingService;

	private readonly BuildingService _buildingService;

	private readonly ILoc _loc;

	private readonly ToolButtonService _toolButtonService;

	public BuildingTutorialStepDeserializer(BuiltBuildingService builtBuildingService, BuildingService buildingService, ILoc loc, ToolButtonService toolButtonService)
	{
		_builtBuildingService = builtBuildingService;
		_buildingService = buildingService;
		_loc = loc;
		_toolButtonService = toolButtonService;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is BuildingTutorialStepSpec buildingTutorialStepSpec)
		{
			tutorialStep = (buildingTutorialStepSpec.OnlyFinishedBuildings ? Create(buildingTutorialStepSpec.TemplateNames, buildingTutorialStepSpec.RequiredAmount, onlyFinishedBuildings: true, "Tutorial.Building") : Create(buildingTutorialStepSpec.TemplateNames, buildingTutorialStepSpec.RequiredAmount, onlyFinishedBuildings: false, "Tutorial.PlaceBuilding"));
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(IEnumerable<string> templateNames, int requiredAmount, bool onlyFinishedBuildings, string mainLocKey)
	{
		List<string> list = templateNames.ToList();
		LabeledEntitySpec spec = _buildingService.GetBuildingTemplate(list.First()).GetSpec<LabeledEntitySpec>();
		string localizedBuildingName = _loc.T(spec.DisplayNameLocKey);
		ImmutableArray<ToolButton> immutableArray = GetToolButtons(list).ToImmutableArray();
		ToolGroupButton toolGroupButton = _toolButtonService.GetToolGroupButton(immutableArray.First());
		return TutorialStep.Create(new BuildingTutorialStep(_builtBuildingService, _loc, list, onlyFinishedBuildings, requiredAmount, mainLocKey, localizedBuildingName), toolGroupButton, immutableArray);
	}

	private IEnumerable<ToolButton> GetToolButtons(IEnumerable<string> templateNames)
	{
		foreach (string templateName in templateNames)
		{
			yield return _toolButtonService.GetToolButton((BlockObjectTool tool) => tool.Template.GetSpec<TemplateSpec>().IsNamedExactly(templateName));
		}
	}
}
