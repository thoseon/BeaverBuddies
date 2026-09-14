using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.NaturalResources;
using Timberborn.TemplateSystem;

namespace Timberborn.Planting;

public class PlanterBuilding : BaseComponent, IAwakableComponent
{
	private readonly TemplateService _templateService;

	private FrozenSet<string> _allowedPlantables;

	public ImmutableArray<PlantableSpec> AllowedPlantables { get; private set; }

	public PlanterBuilding(TemplateService templateService)
	{
		_templateService = templateService;
	}

	public void Awake()
	{
		AllowedPlantables = GetAllowedPlantables().ToImmutableArray();
		_allowedPlantables = AllowedPlantables.Select((PlantableSpec plantable) => plantable.TemplateName).ToFrozenSet();
	}

	public bool CanPlant(string plantable)
	{
		return _allowedPlantables.Contains(plantable);
	}

	private IEnumerable<PlantableSpec> GetAllowedPlantables()
	{
		PlanterBuildingSpec planterBuildingSpec = GetComponent<PlanterBuildingSpec>();
		return from plantable in _templateService.GetAll<PlantableSpec>()
			where planterBuildingSpec.PlantableResourceGroup == plantable.ResourceGroup && plantable.GetSpec<NaturalResourceSpec>().UsableWithCurrentFeatureToggles
			orderby plantable.GetSpec<NaturalResourceSpec>().Order
			select plantable;
	}
}
