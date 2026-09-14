using System.Collections.Generic;
using System.Linq;
using Timberborn.BottomBarSystem;
using Timberborn.Fields;
using Timberborn.NaturalResources;
using Timberborn.Planting;
using Timberborn.PlantingUI;
using Timberborn.TemplateSystem;
using Timberborn.ToolButtonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.FieldsUI;

internal class FieldsButton : IBottomBarElementsProvider
{
	private static readonly string ToolGroupId = "Fields";

	private readonly ToolGroupButtonFactory _toolGroupButtonFactory;

	private readonly TemplateService _templateService;

	private readonly PlantingToolButtonFactory _plantingToolButtonFactory;

	private readonly ToolGroupService _toolGroupService;

	public FieldsButton(ToolGroupButtonFactory toolGroupButtonFactory, TemplateService templateService, PlantingToolButtonFactory plantingToolButtonFactory, ToolGroupService toolGroupService)
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
		foreach (PlantableSpec item in from template in (from plantable in _templateService.GetAll<PlantableSpec>()
				where plantable.GetSpec<NaturalResourceSpec>().UsableWithCurrentFeatureToggles
				orderby plantable.GetSpec<NaturalResourceSpec>().Order
				select plantable).ToList()
			where template.HasSpec<CropSpec>()
			select template)
		{
			ITool tool = CreateTool(item, toolGroupButton);
			_toolGroupService.AssignToGroup(toolGroup, tool);
		}
		ToolButton toolButton = _plantingToolButtonFactory.CreateCancelTool(toolGroupButton.ToolButtonsElement);
		toolGroupButton.AddTool(toolButton);
		_toolGroupService.AssignToGroup(toolGroup, toolButton.Tool);
		yield return BottomBarElement.CreateMultiLevel(toolGroupButton.Root, toolGroupButton.ToolButtonsElement);
	}

	private ITool CreateTool(PlantableSpec plantableSpec, ToolGroupButton toolGroupButton)
	{
		ToolButton toolButton = _plantingToolButtonFactory.CreatePlantingTool(plantableSpec, toolGroupButton.ToolButtonsElement);
		toolGroupButton.AddTool(toolButton);
		return toolButton.Tool;
	}
}
