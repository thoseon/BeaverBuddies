using Bindito.Core;
using Timberborn.Characters;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Bots;

[Context("Game")]
internal class BotsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BotIlluminationController>().AsTransient();
		Bind<BotLongevity>().AsTransient();
		Bind<Bot>().AsTransient();
		Bind<BotFactory>().AsSingleton();
		Bind<BotPopulation>().AsSingleton();
		Bind<BotColors>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BotSpec, Bot>();
		builder.AddDecorator<BotSpec, Character>();
		builder.AddDecorator<BotSpec, BotIlluminationController>();
		builder.AddDecorator<BotSpec, BotLongevity>();
		return builder.Build();
	}
}
