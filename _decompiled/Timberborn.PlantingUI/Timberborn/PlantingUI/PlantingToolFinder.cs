using System;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.TemplateSystem;
using Timberborn.ToolButtonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.PlantingUI;

internal class PlantingToolFinder : IToolFinder
{
	private readonly ToolButtonService _toolButtonService;

	private readonly ToolService _toolService;

	public PlantingToolFinder(ToolButtonService toolButtonService, ToolService toolService)
	{
		_toolButtonService = toolButtonService;
		_toolService = toolService;
	}

	public bool TryFindTool(BaseComponent entity, out Action toolActivationAction)
	{
		string templateName = entity.GetComponent<TemplateSpec>().TemplateName;
		PlantingTool tool = (from toolButton in _toolButtonService.ToolButtons
			where toolButton.ToolEnabled
			select toolButton.Tool).OfType<PlantingTool>().SingleOrDefault(ToolMatchesPlantableSpecName(templateName));
		toolActivationAction = ((tool != null) ? ((Action)delegate
		{
			_toolService.SwitchTool(tool);
		}) : null);
		return tool != null;
	}

	private static Func<PlantingTool, bool> ToolMatchesPlantableSpecName(string templateName)
	{
		return (PlantingTool tool) => tool.PlantableSpec.TemplateName.Equals(templateName);
	}
}
