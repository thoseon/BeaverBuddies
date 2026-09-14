using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.LinkedBuildingSystem;

[Context("Game")]
internal class LinkedBuildingSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<LinkedBuilding>().AsTransient();
		Bind<LinkedConstructionSite>().AsTransient();
		Bind<LinkedConstructionSiteReachability>().AsTransient();
		Bind<LinkedConstructionSiteRecoverableGoodMultiplier>().AsTransient();
		Bind<LinkedPausableConstructionSite>().AsTransient();
		Bind<LinkedInventories>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<LinkedBuildingSpec, LinkedBuilding>();
		builder.AddDecorator<LinkedBuilding, LinkedConstructionSite>();
		builder.AddDecorator<LinkedConstructionSite, LinkedConstructionSiteReachability>();
		builder.AddDecorator<LinkedConstructionSite, LinkedPausableConstructionSite>();
		builder.AddDecorator<LinkedConstructionSite, LinkedInventories>();
		builder.AddDecorator<LinkedConstructionSite, LinkedConstructionSiteRecoverableGoodMultiplier>();
		return builder.Build();
	}
}
