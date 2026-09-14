using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Buildings;
using Timberborn.BuildingsUI;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.TutorialSystem;
using UnityEngine;

namespace Timberborn.TutorialSteps;

internal class ChangePausedStateStepDeserializer : IStepDeserializer, ILoadableSingleton
{
	private readonly BuildingService _buildingService;

	private readonly BuiltBuildingService _builtBuildingService;

	private readonly ILoc _loc;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly PausableBuildingFragment _pausableBuildingFragment;

	private readonly Highlighter _highlighter;

	private readonly ISpecService _specService;

	private Color _tutorialBuildingHighlight;

	public ChangePausedStateStepDeserializer(BuildingService buildingService, BuiltBuildingService builtBuildingService, ILoc loc, EntitySelectionService entitySelectionService, PausableBuildingFragment pausableBuildingFragment, Highlighter highlighter, ISpecService specService)
	{
		_buildingService = buildingService;
		_builtBuildingService = builtBuildingService;
		_loc = loc;
		_entitySelectionService = entitySelectionService;
		_pausableBuildingFragment = pausableBuildingFragment;
		_highlighter = highlighter;
		_specService = specService;
	}

	public void Load()
	{
		_tutorialBuildingHighlight = _specService.GetSingleSpec<TutorialColorsSpec>().TutorialBuildingHighlight;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is ChangePausedStateStepSpec changePausedStateStepSpec)
		{
			tutorialStep = Create(changePausedStateStepSpec.ShouldBePaused, changePausedStateStepSpec.TemplateName);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(bool shouldBePaused, string templateName)
	{
		return TutorialStep.Create(new ChangePausedStateStep(_entitySelectionService, GetDescription(shouldBePaused, templateName), shouldBePaused, templateName), delegate(bool state)
		{
			Highlight(state, templateName);
		}, "ToggleBuildingPause");
	}

	private string GetDescription(bool shouldBePaused, string templateName)
	{
		LabeledEntitySpec spec = _buildingService.GetBuildingTemplate(templateName).GetSpec<LabeledEntitySpec>();
		string key = (shouldBePaused ? "Tutorial.PauseBuilding" : "Tutorial.UnpauseBuilding");
		return _loc.T(key, _loc.T(spec.DisplayNameLocKey));
	}

	private void Highlight(bool highlight, string templateName)
	{
		SelectableObject selectedObject = _entitySelectionService.SelectedObject;
		if ((bool)selectedObject)
		{
			TemplateSpec component = selectedObject.GetComponent<TemplateSpec>();
			if ((object)component != null && component.IsNamedExactly(templateName))
			{
				_pausableBuildingFragment.ToggleHighlight(highlight);
				_highlighter.UnhighlightAllPrimary();
				return;
			}
		}
		HighlightBuilding(highlight, templateName);
	}

	private void HighlightBuilding(bool highlight, string templateName)
	{
		if (highlight)
		{
			IReadOnlyList<Building> finishedBuildings = _builtBuildingService.GetFinishedBuildings(templateName);
			for (int i = 0; i < finishedBuildings.Count; i++)
			{
				_highlighter.HighlightPrimary(finishedBuildings[i], _tutorialBuildingHighlight);
			}
		}
		else
		{
			_highlighter.UnhighlightAllPrimary();
		}
	}
}
