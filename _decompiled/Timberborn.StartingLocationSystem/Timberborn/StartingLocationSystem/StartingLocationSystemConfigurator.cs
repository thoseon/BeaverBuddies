using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.StartingLocationSystem;

[Context("Game")]
[Context("MapEditor")]
internal class StartingLocationSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<StartingLocation>().AsTransient();
		Bind<StartingLocationRenderer>().AsTransient();
		Bind<StartingLocationService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<StartingLocationSpec, StartingLocation>();
		builder.AddDecorator<StartingLocationSpec, StartingLocationRenderer>();
		return builder.Build();
	}
}
