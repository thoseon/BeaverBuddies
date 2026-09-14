using System.Collections.Generic;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;

namespace Timberborn.NaturalResourcesMoisture;

public class AridNaturalResourceService : ILoadableSingleton
{
	private readonly TemplateService _templateService;

	private readonly HashSet<string> _resourceNames = new HashSet<string>();

	public AridNaturalResourceService(TemplateService templateService)
	{
		_templateService = templateService;
	}

	public void Load()
	{
		foreach (AridNaturalResourceSpec item in _templateService.GetAll<AridNaturalResourceSpec>())
		{
			_resourceNames.Add(item.GetSpec<TemplateSpec>().TemplateName);
		}
	}

	public bool IsAridResource(string resourceName)
	{
		return _resourceNames.Contains(resourceName);
	}
}
