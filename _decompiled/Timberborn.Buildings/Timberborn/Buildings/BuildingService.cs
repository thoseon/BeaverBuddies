using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;

namespace Timberborn.Buildings;

public class BuildingService : ILoadableSingleton
{
	private readonly TemplateService _templateService;

	private readonly List<BuildingSpec> _buildings = new List<BuildingSpec>();

	public ReadOnlyList<BuildingSpec> Buildings => _buildings.AsReadOnlyList();

	public BuildingService(TemplateService templateService)
	{
		_templateService = templateService;
	}

	public void Load()
	{
		_buildings.AddRange(_templateService.GetAll<BuildingSpec>());
	}

	public string GetTemplateName(BuildingSpec buildingSpec)
	{
		return buildingSpec.GetSpec<TemplateSpec>().TemplateName;
	}

	public BuildingSpec GetBuildingTemplate(string templateName)
	{
		return _buildings.SingleOrDefault((BuildingSpec building) => IsBuildingNamedExactly(templateName, building)) ?? throw new ArgumentException("Building not found: " + templateName + ".");
	}

	private static bool IsBuildingNamedExactly(string templateName, BuildingSpec buildingSpec)
	{
		return buildingSpec.GetSpec<TemplateSpec>().IsNamedExactly(templateName);
	}
}
