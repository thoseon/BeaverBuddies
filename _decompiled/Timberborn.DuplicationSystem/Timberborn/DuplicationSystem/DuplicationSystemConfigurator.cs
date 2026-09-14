using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.DuplicationSystem;

[Context("Game")]
[Context("MapEditor")]
internal class DuplicationSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DuplicationBlocker>().AsTransient();
		Bind<DuplicableInitializer>().AsTransient();
		Bind<Duplicator>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<IDuplicable, DuplicableInitializer>();
		return builder.Build();
	}
}
