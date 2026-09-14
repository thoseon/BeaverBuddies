using Bindito.Core;
using Timberborn.NeedApplication;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NeedApplicationUI;

[Context("Game")]
internal class NeedApplicationUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<NeedEffectsSpecDescriber>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<INeedEffectsSpec, NeedEffectsSpecDescriber>();
		return builder.Build();
	}
}
