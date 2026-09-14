using Bindito.Core;
using Timberborn.Demolishing;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NaturalResources;

[Context("Game")]
[Context("MapEditor")]
internal class NaturalResourcesConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<NaturalResource>().AsTransient();
		Bind<CoordinatesOffsetter>().AsTransient();
		Bind<SpawnValidationService>().AsSingleton();
		Bind<NaturalResourceFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<NaturalResourceSpec, NaturalResource>();
		builder.AddDecorator<NaturalResourceSpec, Demolishable>();
		builder.AddDecorator<NaturalResourceSpec, CoordinatesOffsetter>();
		builder.AddDecorator<NaturalResourceSpec, LivingNaturalResource>();
		builder.AddDecorator<NaturalResourceSpec, DyingNaturalResource>();
		return builder.Build();
	}
}
