using Bindito.Core;
using Timberborn.NaturalResourcesContamination;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NaturalResourcesContaminationUI;

[Context("Game")]
internal class NaturalResourcesContaminationUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ContaminatedNaturalResourceStatus>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ContaminatedNaturalResource, ContaminatedNaturalResourceStatus>();
		return builder.Build();
	}
}
