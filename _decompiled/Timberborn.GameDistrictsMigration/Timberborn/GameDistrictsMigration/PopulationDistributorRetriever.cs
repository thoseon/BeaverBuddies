using Timberborn.BaseComponentSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.GameDistrictsMigration;

public class PopulationDistributorRetriever
{
	public PopulationDistributor GetPopulationDistributor<T>(BaseComponent component) where T : BaseComponent, IDistributorTemplate
	{
		string componentName = component.GetComponent<T>().ComponentName;
		return component.GetNamedComponent<PopulationDistributor>(componentName);
	}
}
