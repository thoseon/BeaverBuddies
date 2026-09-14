using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.GameSaveRuntimeSystemUI;

[Context("Game")]
internal class GameSaveRuntimeSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SaveGameBox>().AsSingleton();
		Bind<SaveNameProvider>().AsSingleton();
		MultiBind<IDevModule>().To<GameSaverDevModule>().AsSingleton();
	}
}
