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

internal class PowerBuildingsTutorialStepDeserializer : IStepDeserializer
{
	private readonly BuiltBuildingService _builtBuildingService;

	private readonly BuildingService _buildingService;

	private readonly ILoc _loc;

	private readonly ToolButtonService _toolButtonService;

	public PowerBuildingsTutorialStepDeserializer(BuiltBuildingService builtBuildingService, BuildingService buildingService, ILoc loc, ToolButtonService toolButtonService)
	{
		_builtBuildingService = builtBuildingService;
		_buildingService = buildingService;
		_loc = loc;
		_toolButtonService = toolButtonService;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is PowerBuildingsTutorialStepSpec powerBuildingsTutorialStepSpec)
		{
			tutorialStep = Create(powerBuildingsTutorialStepSpec.TemplateName, powerBuildingsTutorialStepSpec.RequiredAmount);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(string templateName, int requiredAmount)
	{
		LabeledEntitySpec spec = _buildingService.GetBuildingTemplate(templateName).GetSpec<LabeledEntitySpec>();
		string localizedBuildingName = _loc.T(spec.DisplayNameLocKey);
		ImmutableArray<ToolButton> immutableArray = GetToolButtons("PowerShaft.Folktails", "VerticalPowerShaft.Folktails").ToImmutableArray();
		ToolGroupButton toolGroupButton = _toolButtonService.GetToolGroupButton(immutableArray.First());
		return TutorialStep.Create(new PowerBuildingsTutorialStep(_builtBuildingService, _loc, templateName, requiredAmount, localizedBuildingName), toolGroupButton, immutableArray);
	}

	private IEnumerable<ToolButton> GetToolButtons(params string[] templateNames)
	{
		foreach (string templateName in templateNames)
		{
			yield return _toolButtonService.GetToolButton((BlockObjectTool tool) => tool.Template.GetSpec<TemplateSpec>().IsNamed(templateName));
		}
	}
}
