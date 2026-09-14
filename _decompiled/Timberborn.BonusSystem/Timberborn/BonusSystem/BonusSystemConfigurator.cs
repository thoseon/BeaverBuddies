using Bindito.Core;
using Timberborn.Characters;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BonusSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class BonusSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BonusManager>().AsTransient();
		Bind<BonusTypeSpecService>().AsSingleton();
		Bind<BonusDescriber>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Character, BonusManager>();
		return builder.Build();
	}
}
