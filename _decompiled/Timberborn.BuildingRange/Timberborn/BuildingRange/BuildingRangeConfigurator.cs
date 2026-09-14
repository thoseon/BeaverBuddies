using Bindito.Core;

namespace Timberborn.BuildingRange;

[Context("Game")]
[Context("MapEditor")]
internal class BuildingRangeConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuildingWithRoadSpillRange>().AsTransient();
		Bind<BuildingWithTerrainRange>().AsTransient();
	}
}
