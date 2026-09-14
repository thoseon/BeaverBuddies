using Bindito.Core;

namespace Timberborn.TutorialSystemUI;

[Context("Game")]
internal class TutorialSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TutorialPanels>().AsSingleton();
		Bind<TutorialPanelFactory>().AsSingleton();
		Bind<DisableTutorialButtonInitializer>().AsSingleton();
		Bind<TutorialPanelBlinker>().AsSingleton();
		Bind<TutorialStepViewFactory>().AsSingleton();
	}
}
