using Timberborn.BaseComponentSystem;

namespace Timberborn.TemplateSystem;

public class TemplateNameRetriever
{
	public string GetTemplateName(BaseComponent instance)
	{
		return GetTemplateName(instance.Name, instance.GetComponent<TemplateSpec>());
	}

	private static string GetTemplateName(string name, TemplateSpec persistentTemplate)
	{
		if (persistentTemplate == null)
		{
			throw new TemplateMappingException(name + " has no TemplateSpec component");
		}
		return persistentTemplate.TemplateName;
	}
}
