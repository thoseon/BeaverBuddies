using Timberborn.TemplateInstantiation;

namespace Timberborn.GameDistrictsMigration;

internal class DistributorTemplateInitializer : IDedicatedDecoratorInitializer<IDistributorTemplate, PopulationDistributor>
{
	public void Initialize(IDistributorTemplate subject, PopulationDistributor decorator)
	{
		decorator.Initialize(subject);
	}
}
