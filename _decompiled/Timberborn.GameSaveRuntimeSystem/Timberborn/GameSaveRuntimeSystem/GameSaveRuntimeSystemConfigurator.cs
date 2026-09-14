using Bindito.Core;
using Bindito.Unity;

namespace Timberborn.GameSaveRuntimeSystem;

[Context("Game")]
internal class GameSaveRuntimeSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GameSaver>().AsSingleton();
		Bind<GameLoader>().AsSingleton();
		MultiBind<ISceneInitializer>().To<InstantiatingSceneInitializer<GameSaverUnityAdapter>>().AsSingleton();
	}
}
