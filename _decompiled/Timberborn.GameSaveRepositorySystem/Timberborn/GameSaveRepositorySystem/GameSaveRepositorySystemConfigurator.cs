using Bindito.Core;

namespace Timberborn.GameSaveRepositorySystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class GameSaveRepositorySystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GameSaveRepository>().AsSingleton();
		Bind<GameSaveDeserializer>().AsSingleton();
	}
}
