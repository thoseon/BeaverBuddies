using Timberborn.NaturalResourcesMoisture;
using Timberborn.SoilContaminationSystem;
using Timberborn.SoilMoistureSystem;
using UnityEngine;

namespace Timberborn.Planting;

internal class PlantingSoilValidator
{
	private readonly ISoilMoistureService _soilMoistureService;

	private readonly ISoilContaminationService _soilContaminationService;

	private readonly AridNaturalResourceService _aridNaturalResourceService;

	public PlantingSoilValidator(ISoilMoistureService soilMoistureService, ISoilContaminationService soilContaminationService, AridNaturalResourceService aridNaturalResourceService)
	{
		_soilMoistureService = soilMoistureService;
		_soilContaminationService = soilContaminationService;
		_aridNaturalResourceService = aridNaturalResourceService;
	}

	public bool Validate(PlantingSpot plantingSpot)
	{
		Vector3Int coordinates = plantingSpot.Coordinates;
		bool flag = _aridNaturalResourceService.IsAridResource(plantingSpot.ResourceToPlant);
		if (_soilMoistureService.SoilIsMoist(coordinates) == flag)
		{
			return false;
		}
		return !_soilContaminationService.SoilIsContaminated(coordinates);
	}
}
