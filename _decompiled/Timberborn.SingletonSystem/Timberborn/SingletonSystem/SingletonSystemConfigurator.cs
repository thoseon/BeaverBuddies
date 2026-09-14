using Bindito.Core;
using Bindito.Unity;

namespace Timberborn.SingletonSystem;

[Context("Bootstrapper")]
[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class SingletonSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<EventBus>().AsSingleton();
		Bind<ISingletonRepository>().To<SingletonRepository>().AsSingleton();
		Bind<SingletonLifecycleService>().AsSingleton();
		SingletonListener singletonListener = new SingletonListener();
		Bind<SingletonListener>().ToInstance(singletonListener);
		AddInjectionListener(singletonListener);
		AddProvisionListener(singletonListener);
		MultiBind<ISceneInitializer>().To<InstantiatingSceneInitializer<SingletonLifecycleUnityAdapter>>().AsSingleton();
	}
}
