using System.Collections.Generic;
using System.Linq;
using Timberborn.BottomBarSystem;
using Timberborn.Forestry;
using Timberborn.NaturalResources;
using Timberborn.Planting;
using Timberborn.PlantingUI;
using Timberborn.TemplateSystem;
using Timberborn.ToolButtonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.ForestryUI;

internal class ForestryButton : IBottomBarElementsProvider
{
	private static readonly string ToolGroupId = "Forestry";

	private readonly ToolGroupButtonFactory _toolGroupButtonFactory;

	private readonly TemplateService _templateService;

	private readonly PlantingToolButtonFactory _plantingToolButtonFactory;

	private readonly ToolGroupService _toolGroupService;

	public ForestryButton(ToolGroupButtonFactory toolGroupButtonFactory, TemplateService templateService, PlantingToolButtonFactory plantingToolButtonFactory, ToolGroupService toolGroupService)
	{
		_toolGroupButtonFactory = toolGroupButtonFactory;
		_templateService = templateService;
		_plantingToolButtonFactory = plantingToolButtonFactory;
		_toolGroupService = toolGroupService;
	}

	public IEnumerable<BottomBarElement> GetElements()
	{
		ToolGroupSpec toolGroup = _toolGroupService.GetGroup(ToolGroupId);
		ToolGroupButton toolGroupButton = _toolGroupButtonFactory.CreateBlue(toolGroup);
		List<PlantableSpec> source = (from plantable in _templateService.GetAll<PlantableSpec>()
			where plantable.GetSpec<NaturalResourceSpec>().UsableWithCurrentFeatureToggles
			orderby plantable.GetSpec<NaturalResourceSpec>().Order
			select plantable).ToList();
		IEnumerable<PlantableSpec> first = source.Where((PlantableSpec template) => template.HasSpec<BushSpec>());
		IEnumerable<PlantableSpec> second = source.Where((PlantableSpec template) => template.HasSpec<TreeComponentSpec>());
		foreach (PlantableSpec item in first.Concat(second))
		{
			ITool tool = CreateTool(item, toolGroupButton);
			_toolGroupService.AssignToGroup(toolGroup, tool);
		}
		ToolButton toolButton = _plantingToolButtonFactory.CreateCancelTool(toolGroupButton.ToolButtonsElement);
		_toolGroupService.AssignToGroup(toolGroup, toolButton.Tool);
		toolGroupButton.AddTool(toolButton);
		yield return BottomBarElement.CreateMultiLevel(toolGroupButton.Root, toolGroupButton.ToolButtonsElement);
	}

	private ITool CreateTool(PlantableSpec plantableSpec, ToolGroupButton toolGroupButton)
	{
		ToolButton toolButton = _plantingToolButtonFactory.CreatePlantingTool(plantableSpec, toolGroupButton.ToolButtonsElement);
		toolGroupButton.AddTool(toolButton);
		return toolButton.Tool;
	}
}
