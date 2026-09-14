using Bindito.Core;

namespace Timberborn.GameSceneLoading;

[Context("MainMenu")]
[Context("Game")]
internal class GameSceneLoadingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GameSceneLoader>().AsTransient();
	}
}
