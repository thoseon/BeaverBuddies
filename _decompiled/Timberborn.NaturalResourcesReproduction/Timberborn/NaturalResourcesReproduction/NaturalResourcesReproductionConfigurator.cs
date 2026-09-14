using Bindito.Core;
using Timberborn.Debugging;
using Timberborn.NaturalResources;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NaturalResourcesReproduction;

[Context("Game")]
[Context("MapEditor")]
internal class NaturalResourcesReproductionConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DyingNaturalResourceReproducible>().AsTransient();
		Bind<LivingReproducible>().AsTransient();
		Bind<Reproducible>().AsTransient();
		Bind<NaturalResourceReproducer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<IDevModule>().To<PotentialSpotsToggler>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<NaturalResourceSpec, Reproducible>();
		builder.AddDecorator<DyingNaturalResource, DyingNaturalResourceReproducible>();
		builder.AddDecorator<LivingNaturalResource, LivingReproducible>();
		return builder.Build();
	}
}
