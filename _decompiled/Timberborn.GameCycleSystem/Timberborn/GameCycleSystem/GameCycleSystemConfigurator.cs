using Bindito.Core;

namespace Timberborn.GameCycleSystem;

[Context("Game")]
[Context("MapEditor")]
internal class GameCycleSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GameCycleService>().AsSingleton();
	}
}
