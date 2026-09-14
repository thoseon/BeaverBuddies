using Bindito.Core;
using Timberborn.BlockSystemNavigation;
using Timberborn.BuildingRange;
using Timberborn.Buildings;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BuildingsNavigation;

[Context("Game")]
internal class BuildingsNavigationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuildingCachingFlowField>().AsTransient();
		Bind<BuildingNavMesh>().AsTransient();
		Bind<BuildingRangeDrawer>().AsTransient();
		Bind<BuildingTerrainRange>().AsTransient();
		Bind<ConstructionSiteAccessible>().AsTransient();
		Bind<DistrictPathNavRangeDrawer>().AsTransient();
		Bind<DistrictPathNavRangeDrawerRegistrar>().AsTransient();
		Bind<PathDistrictRetriever>().AsTransient();
		Bind<PathMeshHider>().AsTransient();
		Bind<PathRangeDrawer>().AsTransient();
		Bind<BoundsNavRangeDrawingService>().AsSingleton();
		Bind<PathNavRangeDrawerInvalidator>().AsSingleton();
		Bind<PathMeshDrawerFactory>().AsSingleton();
		Bind<BoundsNavRangeCalculator>().AsSingleton();
		Bind<BoundsNavRangeDrawer>().AsSingleton();
		Bind<DistanceToColorConverter>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BuildingSpec, BuildingNavMesh>();
		builder.AddDecorator<BuildingAccessible, BuildingCachingFlowField>();
		builder.AddDecorator<BuildingSpec, ConstructionSiteAccessible>();
		builder.AddDecorator<DistrictBuilding, BuildingRangeDrawer>();
		builder.AddDecorator<BuildingWithTerrainRange, BuildingTerrainRange>();
		builder.AddDecorator<BlockObjectWithPathRangeSpec, PathRangeDrawer>();
		builder.AddDecorator<BlockObjectWithPathRangeSpec, PathDistrictRetriever>();
		builder.AddDecorator<DistrictCenter, DistrictPathNavRangeDrawer>();
		builder.AddDecorator<DistrictCenter, PathMeshHider>();
		builder.AddDecorator<DistrictPathNavRangeDrawer, DistrictPathNavRangeDrawerRegistrar>();
		return builder.Build();
	}
}
