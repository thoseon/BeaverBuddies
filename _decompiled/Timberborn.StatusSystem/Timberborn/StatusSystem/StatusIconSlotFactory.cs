namespace Timberborn.StatusSystem;

internal class StatusIconSlotFactory
{
	public StatusSlot CreateBounded(SlotConstraints slotConstraints, TopBoundForLayer topBoundForLayer, float statusZCoordinate, byte baseZ)
	{
		if ((slotConstraints.IsOccupied && !slotConstraints.InvalidInConstructionMode) || statusZCoordinate < topBoundForLayer.NormalModeTopBound)
		{
			return StatusSlot.CreateValidAboveMaxVisibilityLevel(statusZCoordinate, baseZ);
		}
		if ((!slotConstraints.InvalidInConstructionMode && !(statusZCoordinate < topBoundForLayer.ConstructionModeTopBound)) || slotConstraints.ForceValidInConstructionMode)
		{
			return StatusSlot.CreateAlwaysValid(statusZCoordinate);
		}
		return StatusSlot.CreateInvalidInConstructionMode(statusZCoordinate, baseZ);
	}

	public StatusSlot CreateUnbounded(SlotConstraints slotConstraints, float statusZCoordinate, byte baseZ)
	{
		if (slotConstraints.IsOccupied && !slotConstraints.InvalidInConstructionMode)
		{
			return StatusSlot.CreateValidAboveMaxVisibilityLevel(statusZCoordinate, baseZ);
		}
		if (!slotConstraints.InvalidInConstructionMode)
		{
			return StatusSlot.CreateAlwaysValid(statusZCoordinate);
		}
		return StatusSlot.CreateInvalidInConstructionMode(statusZCoordinate, baseZ);
	}
}
