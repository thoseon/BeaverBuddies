using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.StockpilesUI;
using Timberborn.TemplateSystem;
using Timberborn.TutorialSystem;
using UnityEngine;

namespace Timberborn.TutorialSteps;

internal class SelectStockpileGoodTutorialStepDeserializer : IStepDeserializer, ILoadableSingleton
{
	private readonly BuiltBuildingService _builtBuildingService;

	private readonly BuildingService _buildingService;

	private readonly ILoc _loc;

	private readonly IGoodService _goodService;

	private readonly StockpileInventoryFragment _stockpileInventoryFragment;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly Highlighter _highlighter;

	private readonly ISpecService _specService;

	private Color _tutorialBuildingHighlight;

	public SelectStockpileGoodTutorialStepDeserializer(BuiltBuildingService builtBuildingService, BuildingService buildingService, ILoc loc, IGoodService goodService, StockpileInventoryFragment stockpileInventoryFragment, EntitySelectionService entitySelectionService, Highlighter highlighter, ISpecService specService)
	{
		_builtBuildingService = builtBuildingService;
		_buildingService = buildingService;
		_loc = loc;
		_goodService = goodService;
		_stockpileInventoryFragment = stockpileInventoryFragment;
		_entitySelectionService = entitySelectionService;
		_highlighter = highlighter;
		_specService = specService;
	}

	public void Load()
	{
		_tutorialBuildingHighlight = _specService.GetSingleSpec<TutorialColorsSpec>().TutorialBuildingHighlight;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is SelectStockpileGoodTutorialStepSpec selectStockpileGoodTutorialStepSpec)
		{
			tutorialStep = Create(selectStockpileGoodTutorialStepSpec.TemplateName, selectStockpileGoodTutorialStepSpec.RequiredAmount, selectStockpileGoodTutorialStepSpec.GoodId);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(string templateName, int requiredAmount, string goodId)
	{
		LabeledEntitySpec spec = _buildingService.GetBuildingTemplate(templateName).GetSpec<LabeledEntitySpec>();
		string localizedBuildingName = _loc.T(spec.DisplayNameLocKey);
		GoodSpec good = _goodService.GetGood(goodId);
		return TutorialStep.Create(new SelectStockpileGoodTutorialStep(_builtBuildingService, _loc, templateName, good, requiredAmount, "Tutorial.SelectGood", localizedBuildingName), delegate(bool state)
		{
			Highlight(state, templateName, goodId);
		});
	}

	private void Highlight(bool highlight, string templateName, string goodId)
	{
		SelectableObject selectedObject = _entitySelectionService.SelectedObject;
		if ((bool)selectedObject)
		{
			TemplateSpec component = selectedObject.GetComponent<TemplateSpec>();
			if ((object)component != null && component.IsNamedExactly(templateName))
			{
				_stockpileInventoryFragment.ToggleButtonHighlight(highlight);
				_highlighter.UnhighlightAllPrimary();
				return;
			}
		}
		HighlightBuilding(highlight, templateName, goodId);
	}

	private void HighlightBuilding(bool highlight, string templateName, string goodId)
	{
		if (highlight)
		{
			IReadOnlyList<Building> finishedBuildings = _builtBuildingService.GetFinishedBuildings(templateName);
			for (int i = 0; i < finishedBuildings.Count; i++)
			{
				SingleGoodAllower component = finishedBuildings[i].GetComponent<SingleGoodAllower>();
				if (component.AllowedGood != goodId)
				{
					_highlighter.HighlightPrimary(component, _tutorialBuildingHighlight);
				}
			}
		}
		else
		{
			_highlighter.UnhighlightAllPrimary();
		}
	}
}
