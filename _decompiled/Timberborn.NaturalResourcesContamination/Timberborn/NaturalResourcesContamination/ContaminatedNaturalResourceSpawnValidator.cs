using Timberborn.NaturalResources;
using Timberborn.SoilContaminationSystem;
using UnityEngine;

namespace Timberborn.NaturalResourcesContamination;

internal class ContaminatedNaturalResourceSpawnValidator : ISpawnValidator
{
	private readonly ISoilContaminationService _soilContaminationService;

	public ContaminatedNaturalResourceSpawnValidator(ISoilContaminationService soilContaminationService)
	{
		_soilContaminationService = soilContaminationService;
	}

	public bool CanSpawn(Vector3Int coordinates, string resourceTemplateName)
	{
		return !_soilContaminationService.SoilIsContaminated(coordinates);
	}
}
