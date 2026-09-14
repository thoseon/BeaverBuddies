using Timberborn.BlockSystem;

namespace Timberborn.NaturalResources;

public class NaturalResourcePlantedEvent
{
	public BlockObjectSpec PlantedResource { get; }

	public NaturalResourcePlantedEvent(BlockObjectSpec plantedResource)
	{
		PlantedResource = plantedResource;
	}
}
