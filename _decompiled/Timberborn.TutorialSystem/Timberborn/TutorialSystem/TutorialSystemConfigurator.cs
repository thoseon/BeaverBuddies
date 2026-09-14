using Bindito.Core;

namespace Timberborn.TutorialSystem;

[Context("Game")]
internal class TutorialSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TutorialService>().AsSingleton();
		Bind<ITutorialService>().ToExisting<TutorialService>();
		Bind<ITutorialTriggers>().To<TutorialTriggers>().AsSingleton();
		Bind<TutorialStageService>().AsSingleton();
	}
}
