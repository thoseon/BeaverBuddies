using Bindito.Core;
using Timberborn.NaturalResources;
using Timberborn.SoilContaminationSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NaturalResourcesContamination;

[Context("Game")]
[Context("MapEditor")]
internal class NaturalResourcesContaminationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ContaminatedNaturalResource>().AsTransient();
		MultiBind<ISpawnValidator>().To<ContaminatedNaturalResourceSpawnValidator>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ContaminatedNaturalResourceSpec, ContaminatedNaturalResource>();
		builder.AddDecorator<ContaminatedNaturalResource, ContaminatedObject>();
		return builder.Build();
	}
}
