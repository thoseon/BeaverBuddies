using Bindito.Core;
using Timberborn.Attractions;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.AttractionsUI;

[Context("Game")]
internal class AttractionsUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly AttractionFragment _attractionFragment;

		private readonly AttractionLoadRateFragment _attractionLoadRateFragment;

		public EntityPanelModuleProvider(AttractionFragment attractionFragment, AttractionLoadRateFragment attractionLoadRateFragment)
		{
			_attractionFragment = attractionFragment;
			_attractionLoadRateFragment = attractionLoadRateFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_attractionLoadRateFragment);
			builder.AddTopFragment(_attractionFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<AttractionDescriber>().AsTransient();
		Bind<AttractionFragment>().AsSingleton();
		Bind<AttractionLoadRateFragment>().AsSingleton();
		Bind<AttractionBatchControlRowItemFactory>().AsSingleton();
		Bind<AttractionLoadRateBatchControlRowItemFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Attraction, AttractionDescriber>();
		return builder.Build();
	}
}
