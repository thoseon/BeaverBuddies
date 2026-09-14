using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.DeteriorationSystem;

[Context("Game")]
internal class DeteriorationSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Deteriorable>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DeteriorableSpec, Deteriorable>();
		return builder.Build();
	}
}
