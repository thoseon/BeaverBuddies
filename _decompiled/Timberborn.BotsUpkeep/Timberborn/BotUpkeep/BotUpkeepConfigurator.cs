using Bindito.Core;
using Timberborn.Particles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BotUpkeep;

[Context("Game")]
internal class BotUpkeepConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BotManufactoryAnimationController>().AsTransient();
		Bind<BotManufactory>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BotManufactoryAnimationControllerSpec, BotManufactoryAnimationController>();
		builder.AddDecorator<BotManufactoryAnimationController, ParticlesCache>();
		builder.AddDecorator<BotManufactorySpec, BotManufactory>();
		return builder.Build();
	}
}
