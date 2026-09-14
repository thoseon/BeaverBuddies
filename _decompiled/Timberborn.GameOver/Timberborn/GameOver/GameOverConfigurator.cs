using Bindito.Core;

namespace Timberborn.GameOver;

[Context("Game")]
internal class GameOverConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IGameOverChecker>().To<GameOverChecker>().AsSingleton();
		Bind<GameOverDisabler>().AsSingleton();
	}
}
