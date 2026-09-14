using Bindito.Core;

namespace Timberborn.SceneLoading;

[Context("Bootstrapper")]
public class SceneLoadingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<LoadingScreen>().AsSingleton().AsExported();
		Bind<CoroutineStarter>().AsSingleton().AsExported();
		Bind<ISceneLoader>().To<SceneLoader>().AsSingleton().AsExported();
	}
}
