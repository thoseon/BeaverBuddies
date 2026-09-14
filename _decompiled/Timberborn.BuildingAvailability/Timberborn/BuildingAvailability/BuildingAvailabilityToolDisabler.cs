using Timberborn.BlockObjectTools;
using Timberborn.ToolSystem;

namespace Timberborn.BuildingAvailability;

internal class BuildingAvailabilityToolDisabler : IToolDisabler
{
	private readonly BuildingAvailabilityValidator _buildingAvailabilityValidator;

	public BuildingAvailabilityToolDisabler(BuildingAvailabilityValidator buildingAvailabilityValidator)
	{
		_buildingAvailabilityValidator = buildingAvailabilityValidator;
	}

	public bool IsEnabled(ITool tool)
	{
		if (tool is BlockObjectTool blockObjectTool)
		{
			return _buildingAvailabilityValidator.IsAvailableForPlacement(blockObjectTool.Template);
		}
		return true;
	}
}
