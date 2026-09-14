using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Debugging;
using Timberborn.EntitySystem;

namespace Timberborn.BuildingsUI;

public class BuildingsModelToggler : IDevModule
{
	private readonly EntityComponentRegistry _entityComponentRegistry;

	private bool _buildingsHidden;

	public BuildingsModelToggler(EntityComponentRegistry entityComponentRegistry)
	{
		_entityComponentRegistry = entityComponentRegistry;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle models: Buildings", ToggleBuildingModels)).Build();
	}

	private void ToggleBuildingModels()
	{
		_buildingsHidden = !_buildingsHidden;
		foreach (Building item in _entityComponentRegistry.GetEnabled<Building>())
		{
			ToggleBuildingModel(item);
		}
	}

	private void ToggleBuildingModel(Building building)
	{
		bool active = !_buildingsHidden;
		BuildingModel component = building.GetComponent<BuildingModel>();
		if (component != null)
		{
			BlockObject component2 = component.GetComponent<BlockObject>();
			if (component2.IsFinished)
			{
				component.FinishedModel.SetActive(active);
			}
			else if (component2.IsUnfinished)
			{
				component.UnfinishedModel.SetActive(active);
			}
		}
	}
}
