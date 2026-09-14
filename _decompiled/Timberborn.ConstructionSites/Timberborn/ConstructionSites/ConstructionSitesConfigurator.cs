using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ConstructionSites;

[Context("Game")]
[Context("MapEditor")]
internal class ConstructionSitesConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuildBehavior>().AsTransient();
		Bind<BuildExecutor>().AsTransient();
		Bind<ConstructionSite>().AsTransient();
		Bind<ConstructionSiteProgressVisualizer>().AsTransient();
		Bind<Builder>().AsTransient();
		Bind<ConstructionJob>().AsTransient();
		Bind<ConstructionRegistrar>().AsTransient();
		Bind<ConstructionSiteBuildersLimiter>().AsTransient();
		Bind<ConstructionSiteModelUpdater>().AsTransient();
		Bind<ConstructionSitePrioritizableEnabler>().AsTransient();
		Bind<ConstructionSiteRecoverableGoodMultiplier>().AsTransient();
		Bind<ConstructionSiteReservations>().AsTransient();
		Bind<ConstructionSiteSlotManager>().AsTransient();
		Bind<DeleteOnFinishConstructionSite>().AsTransient();
		Bind<GroundedConstructionSite>().AsTransient();
		Bind<PhysicallySupportedConstructionSite>().AsTransient();
		Bind<PhysicallySupportedConstructionSiteUpdater>().AsTransient();
		Bind<ConstructionRegistry>().AsSingleton();
		Bind<ConstructionFactory>().AsSingleton();
		Bind<ConstructionSiteInventoryInitializer>().AsSingleton();
		Bind<ConstructionSiteBuildTimeCalculator>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<ConstructionSitesTemplateModuleProvider>().AsSingleton();
	}
}
