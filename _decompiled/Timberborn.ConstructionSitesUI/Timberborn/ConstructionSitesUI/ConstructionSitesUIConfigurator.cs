using Bindito.Core;
using Timberborn.ConstructionSites;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ConstructionSitesUI;

[Context("Game")]
internal class ConstructionSitesUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly ConstructionSiteDebugFragment _constructionSiteDebugFragment;

		private readonly ConstructionSiteFragment _constructionSiteFragment;

		public EntityPanelModuleProvider(ConstructionSiteDebugFragment constructionSiteDebugFragment, ConstructionSiteFragment constructionSiteFragment)
		{
			_constructionSiteDebugFragment = constructionSiteDebugFragment;
			_constructionSiteFragment = constructionSiteFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddBottomFragment(_constructionSiteFragment, 500);
			builder.AddDiagnosticFragment(_constructionSiteDebugFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<ConstructionSiteDescriber>().AsTransient();
		Bind<ConstructionSiteDebugFragment>().AsSingleton();
		Bind<ConstructionSiteFragment>().AsSingleton();
		Bind<ConstructionSiteFragmentInventory>().AsSingleton();
		Bind<ConstructionSitePanelDescriptionUpdater>().AsSingleton();
		Bind<ConstructionSitePriorityBatchControlRowItemFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ConstructionSite, ConstructionSiteDescriber>();
		return builder.Build();
	}
}
