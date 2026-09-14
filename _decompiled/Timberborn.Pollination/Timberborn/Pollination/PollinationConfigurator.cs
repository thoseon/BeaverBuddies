using Bindito.Core;
using Timberborn.Fields;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Pollination;

[Context("Game")]
internal class PollinationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Hive>().AsTransient();
		Bind<Pollinatee>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvidePollinationModule).AsSingleton();
	}

	private static TemplateModule ProvidePollinationModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Crop, Pollinatee>();
		builder.AddDecorator<HiveSpec, Hive>();
		return builder.Build();
	}
}
