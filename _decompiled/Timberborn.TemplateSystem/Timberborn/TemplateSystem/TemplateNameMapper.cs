using System.Collections.Generic;
using System.Linq;
using Timberborn.SingletonSystem;

namespace Timberborn.TemplateSystem;

public class TemplateNameMapper : ILoadableSingleton
{
	private readonly TemplateService _templateService;

	private Dictionary<string, TemplateSpec> _templates;

	public TemplateNameMapper(TemplateService templateService)
	{
		_templateService = templateService;
	}

	public void Load()
	{
		_templates = new Dictionary<string, TemplateSpec>();
		List<TemplateSpec> list = _templateService.GetAll<TemplateSpec>().ToList();
		foreach (TemplateSpec item in list)
		{
			TryAddTemplate(item.TemplateName, item, throwIfDuplicated: true);
		}
		foreach (TemplateSpec item2 in list)
		{
			foreach (string backwardCompatibleTemplateName in item2.BackwardCompatibleTemplateNames)
			{
				TryAddTemplate(backwardCompatibleTemplateName, item2, throwIfDuplicated: false);
			}
		}
	}

	public TemplateSpec GetTemplate(string templateName)
	{
		if (_templates.TryGetValue(templateName, out var value))
		{
			return value;
		}
		throw new TemplateMappingException("No template found with name '" + templateName + "'");
	}

	public bool TryGetTemplate(string templateName, out TemplateSpec templateSpec)
	{
		return _templates.TryGetValue(templateName, out templateSpec);
	}

	private void TryAddTemplate(string templateName, TemplateSpec templateSpec, bool throwIfDuplicated)
	{
		if (!_templates.TryAdd(templateName, templateSpec) & throwIfDuplicated)
		{
			TemplateSpec templateSpec2 = _templates[templateName];
			throw new TemplateMappingException("Duplicate template name " + templateName + " in objects: " + templateSpec.Blueprint.Name + ", " + templateSpec2.Blueprint.Name + ".");
		}
	}
}
