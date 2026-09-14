namespace Timberborn.BlockSystem;

public interface IBlockOccupancyService
{
	bool OccupantPresentOnArea(BlockObject blockObject, float minDistanceFromArea);
}
