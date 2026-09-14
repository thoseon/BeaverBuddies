using Bindito.Core;
using Timberborn.Bots;
using Timberborn.BottomBarSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BotsUI;

[Context("Game")]
internal class BotsUIConfigurator : Configurator
{
	private class BottomBarModuleProvider : IProvider<BottomBarModule>
	{
		private readonly BotGeneratorButton _botGeneratorButton;

		public BottomBarModuleProvider(BotGeneratorButton botGeneratorButton)
		{
			_botGeneratorButton = botGeneratorButton;
		}

		public BottomBarModule Get()
		{
			BottomBarModule.Builder builder = new BottomBarModule.Builder();
			builder.AddLeftSectionElement(_botGeneratorButton, 80);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<BotEntityBadge>().AsTransient();
		Bind<BotSelectionSound>().AsTransient();
		Bind<BotGeneratorTool>().AsSingleton();
		Bind<BotGeneratorButton>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BotSpec, BotEntityBadge>();
		builder.AddDecorator<BotSelectionSoundSpec, BotSelectionSound>();
		return builder.Build();
	}
}
