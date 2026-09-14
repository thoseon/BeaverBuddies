using Bindito.Core;

namespace Timberborn.GameWonderCompletion;

[Context("Game")]
internal class GameWonderCompletionConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapNameService>().AsSingleton();
		Bind<GameWonderCompletionService>().AsSingleton();
		Bind<WonderCompletionCountdownStarter>().AsSingleton();
		Bind<GameWonderCompletionRestorer>().AsSingleton();
	}
}
