using Bindito.Core;
using Bindito.Unity;

namespace Timberborn.Autosaving;

[Context("Game")]
internal class AutosavingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Autosaver>().AsSingleton();
		Bind<AutosaveNameService>().AsSingleton();
		MultiBind<ISceneInitializer>().To<InstantiatingSceneInitializer<AutosaverUnityAdapter>>().AsSingleton();
	}
}
