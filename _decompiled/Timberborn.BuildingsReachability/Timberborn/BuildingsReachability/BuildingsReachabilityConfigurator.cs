using Bindito.Core;
using Timberborn.Buildings;
using Timberborn.ConstructionSites;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BuildingsReachability;

[Context("Game")]
internal class BuildingsReachabilityConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<EntityReachabilityStatus>().AsTransient();
		Bind<BlockableEntranceBuilding>().AsTransient();
		Bind<ConstructionSiteEntranceBlockedPreviewValidator>().AsTransient();
		Bind<ReachableConstructionSite>().AsTransient();
		Bind<UnconnectedBuildingStatus>().AsTransient();
		Bind<UnconnectedBuildingBlocker>().AsTransient();
		Bind<ReachabilityPreviewValidator>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ConstructionSite, ReachableConstructionSite>();
		builder.AddDecorator<ConstructionSite, ReachabilityPreviewValidator>();
		builder.AddDecorator<ConstructionSite, ConstructionSiteEntranceBlockedPreviewValidator>();
		builder.AddDecorator<BuildingAccessible, UnconnectedBuildingStatus>();
		builder.AddDecorator<BuildingSpec, BlockableEntranceBuilding>();
		builder.AddDecorator<IUnreachableEntity, EntityReachabilityStatus>();
		builder.AddDecorator<UnconnectedBuildingBlockerSpec, UnconnectedBuildingBlocker>();
		return builder.Build();
	}
}
