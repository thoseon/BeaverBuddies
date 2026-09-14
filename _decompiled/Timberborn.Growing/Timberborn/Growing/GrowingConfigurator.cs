using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Growing;

[Context("Game")]
[Context("MapEditor")]
internal class GrowingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Growable>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<GrowableSpec, Growable>();
		return builder.Build();
	}
}
