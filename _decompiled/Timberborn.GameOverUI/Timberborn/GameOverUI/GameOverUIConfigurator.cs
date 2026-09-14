using Bindito.Core;

namespace Timberborn.GameOverUI;

[Context("Game")]
internal class GameOverUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GameOverBox>().AsSingleton();
	}
}
