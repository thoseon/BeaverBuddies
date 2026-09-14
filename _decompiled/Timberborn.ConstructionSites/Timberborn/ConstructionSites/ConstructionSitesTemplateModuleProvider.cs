using Bindito.Core;
using Timberborn.BlockSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.Buildings;
using Timberborn.SlotSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.ConstructionSites;

internal class ConstructionSitesTemplateModuleProvider : IProvider<TemplateModule>
{
	private readonly ConstructionSiteInventoryInitializer _constructionSiteInventoryInitializer;

	public ConstructionSitesTemplateModuleProvider(ConstructionSiteInventoryInitializer constructionSiteInventoryInitializer)
	{
		_constructionSiteInventoryInitializer = constructionSiteInventoryInitializer;
	}

	public TemplateModule Get()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDedicatedDecorator(_constructionSiteInventoryInitializer);
		builder.AddDecorator<ConstructionRegistrar, ConstructionSite>();
		builder.AddDecorator<ConstructionSite, ConstructionSiteModelUpdater>();
		builder.AddDecorator<ConstructionSite, GroundedConstructionSite>();
		builder.AddDecorator<ConstructionSite, ConstructionJob>();
		builder.AddDecorator<ConstructionSite, ConstructionSitePrioritizableEnabler>();
		builder.AddDecorator<ConstructionSite, ConstructionSiteRecoverableGoodMultiplier>();
		builder.AddDecorator<ConstructionSite, ConstructionSiteReservations>();
		builder.AddDecorator<ConstructionSitePrioritizableEnabler, BuilderPrioritizable>();
		builder.AddDecorator<ConstructionSiteSlotManagerSpec, ConstructionSiteSlotManager>();
		builder.AddDecorator<ConstructionSiteSlotManager, SlotManager>();
		builder.AddDecorator<BlockObject, PhysicallySupportedConstructionSiteUpdater>();
		builder.AddDecorator<ConstructionSiteBuildersLimiterSpec, ConstructionSiteBuildersLimiter>();
		builder.AddDecorator<ConstructionSiteProgressVisualizerSpec, ConstructionSiteProgressVisualizer>();
		builder.AddDecorator<Worker, Builder>();
		builder.AddDecorator<Worker, BuildBehavior>();
		builder.AddDecorator<BuildingSpec, ConstructionRegistrar>();
		return builder.Build();
	}
}
