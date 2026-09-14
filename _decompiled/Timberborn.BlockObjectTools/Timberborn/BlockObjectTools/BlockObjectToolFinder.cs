using System;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.TemplateSystem;
using Timberborn.ToolButtonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.BlockObjectTools;

internal class BlockObjectToolFinder : IToolFinder
{
	private readonly ToolButtonService _toolButtonService;

	public BlockObjectToolFinder(ToolButtonService toolButtonService)
	{
		_toolButtonService = toolButtonService;
	}

	public bool TryFindTool(BaseComponent entity, out Action toolActivationAction)
	{
		string templateName = entity.GetComponent<TemplateSpec>().TemplateName;
		BlockObjectTool tool = (from toolButton in _toolButtonService.ToolButtons
			where toolButton.ToolEnabled
			select toolButton.Tool).OfType<BlockObjectTool>().SingleOrDefault(ToolMatchesTemplate(templateName));
		toolActivationAction = ((tool != null) ? ((Action)delegate
		{
			tool.ActivateWithDuplicationSource(entity);
		}) : null);
		return tool != null;
	}

	private static Func<BlockObjectTool, bool> ToolMatchesTemplate(string templateName)
	{
		return (BlockObjectTool tool) => tool.Template.GetSpec<TemplateSpec>().IsNamedExactly(templateName);
	}
}
