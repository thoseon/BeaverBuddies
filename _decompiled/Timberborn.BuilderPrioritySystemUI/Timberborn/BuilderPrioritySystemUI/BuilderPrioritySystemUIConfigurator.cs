using Bindito.Core;
using Timberborn.BottomBarSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BuilderPrioritySystemUI;

[Context("Game")]
internal class BuilderPrioritySystemUIConfigurator : Configurator
{
	private class BottomBarModuleProvider : IProvider<BottomBarModule>
	{
		private readonly BuilderPrioritiesButton _builderPrioritiesButton;

		public BottomBarModuleProvider(BuilderPrioritiesButton builderPrioritiesButton)
		{
			_builderPrioritiesButton = builderPrioritiesButton;
		}

		public BottomBarModule Get()
		{
			BottomBarModule.Builder builder = new BottomBarModule.Builder();
			builder.AddLeftSectionElement(_builderPrioritiesButton, 60);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<BuilderPrioritizableHighlightUpdater>().AsTransient();
		Bind<BuilderPrioritiesButton>().AsSingleton();
		Bind<BuilderPrioritiesButtonFactory>().AsSingleton();
		Bind<BuilderPrioritizableHighlighter>().AsSingleton();
		Bind<BuilderPrioritySpriteLoader>().AsSingleton();
		Bind<BuilderPriorityToggleGroupFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BuilderPrioritizable, BuilderPrioritizableHighlightUpdater>();
		return builder.Build();
	}
}
