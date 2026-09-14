using System.Collections.Generic;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.NaturalResourcesMoisture;

public class FloodableNaturalResourceService : ILoadableSingleton
{
	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly TemplateService _templateService;

	private readonly Dictionary<string, FloodableNaturalResourceSpec> _specs = new Dictionary<string, FloodableNaturalResourceSpec>();

	public FloodableNaturalResourceService(IThreadSafeWaterMap threadSafeWaterMap, TemplateService templateService)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
		_templateService = templateService;
	}

	public void Load()
	{
		foreach (FloodableNaturalResourceSpec item in _templateService.GetAll<FloodableNaturalResourceSpec>())
		{
			string templateName = item.GetSpec<TemplateSpec>().TemplateName;
			_specs[templateName] = item;
		}
	}

	public bool IsFloodableNaturalResource(string resourceName)
	{
		return _specs.ContainsKey(resourceName);
	}

	public bool ConditionsAreMet(string resourceName, Vector3Int coordinates)
	{
		int num = WaterDepth(coordinates);
		FloodableNaturalResourceSpec floodableNaturalResourceSpec = _specs[resourceName];
		if (num >= floodableNaturalResourceSpec.MinWaterHeight)
		{
			return num <= floodableNaturalResourceSpec.MaxWaterHeight;
		}
		return false;
	}

	private int WaterDepth(Vector3Int coordinates)
	{
		int num = _threadSafeWaterMap.CeiledWaterHeight(coordinates);
		if (num <= 0)
		{
			return 0;
		}
		return num - coordinates.z;
	}
}
