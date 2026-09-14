using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.Reproduction;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ReproductionUI;

[Context("Game")]
internal class ReproductionUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly BreedingPodFragment _breedingPodFragment;

		public EntityPanelModuleProvider(BreedingPodFragment breedingPodFragment)
		{
			_breedingPodFragment = breedingPodFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_breedingPodFragment, 20);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<BreedingPodStatusInitializer>().AsTransient();
		Bind<BreedingPodDescriber>().AsTransient();
		Bind<BreedingPodFragment>().AsSingleton();
		Bind<BreedingPodBatchControlRowItemFactory>().AsSingleton();
		Bind<BreedingPodInventoryBatchControlRowItemFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BreedingPod, BreedingPodDescriber>();
		builder.AddDecorator<BreedingPod, BreedingPodStatusInitializer>();
		return builder.Build();
	}
}
