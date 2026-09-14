using Bindito.Core;

namespace Timberborn.CoreUI;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class CoreUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DialogBoxShower>().AsSingleton();
		Bind<InputBoxShower>().AsSingleton();
		Bind<IntegerSliderFactory>().AsSingleton();
		Bind<RowShader>().AsSingleton();
		Bind<UISettings>().AsSingleton();
		Bind<VisualElementInitializer>().AsSingleton();
		Bind<VisualElementLoader>().AsSingleton();
		Bind<ScrollBarInitializationService>().AsSingleton();
		Bind<AlternateClickableFactory>().AsSingleton();
		Bind<HyperlinkInitializer>().AsSingleton();
		Bind<DelayedButtonEnabler>().AsSingleton();
		Bind<Underlay>().AsSingleton();
		Bind<PanelStack>().AsSingleton();
		Bind<UIScaler>().AsSingleton();
		Bind<RadioToggleFactory>().AsSingleton();
		MultiBind<IVisualElementInitializer>().To<VisualElementLocalizer>().AsSingleton();
		MultiBind<IVisualElementInitializer>().To<ButtonClickabilityInitializer>().AsSingleton();
		MultiBind<IVisualElementInitializer>().To<UISoundInitializer>().AsSingleton();
		MultiBind<IVisualElementInitializer>().To<ScrollBarInitializer>().AsSingleton();
		MultiBind<IVisualElementInitializer>().To<TextElementInitializer>().AsSingleton();
	}
}
