using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.TemplateCollectionSystem;

namespace Timberborn.TemplateSystem;

public class TemplateService
{
	private readonly TemplateCollectionService _templateCollectionService;

	public TemplateService(TemplateCollectionService templateCollectionService)
	{
		_templateCollectionService = templateCollectionService;
	}

	public IEnumerable<T> GetAll<T>()
	{
		List<T> list = new List<T>();
		foreach (Blueprint allTemplate in _templateCollectionService.AllTemplates)
		{
			allTemplate.GetSpecs(list);
		}
		return list;
	}

	public T GetSingle<T>()
	{
		List<T> list = GetAll<T>().ToList();
		if (list.Count == 1)
		{
			return list[0];
		}
		throw new InvalidOperationException("Blueprint with component " + typeof(T).Name + " not found or multiple templates/components found");
	}
}
