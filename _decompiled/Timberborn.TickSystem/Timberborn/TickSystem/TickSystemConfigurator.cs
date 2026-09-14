using Bindito.Core;
using Bindito.Unity;

namespace Timberborn.TickSystem;

[Context("Game")]
[Context("MapEditor")]
internal class TickSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Ticker>().AsSingleton();
		Bind<ITickService>().To<TickService>().AsSingleton();
		Bind<ITickableBucketService>().To<TickableBucketService>().AsSingleton();
		Bind<ITickableSingletonService>().To<TickableSingletonService>().AsSingleton();
		Bind<TickableEntityLifecycleManager>().AsSingleton();
		Bind<TickOnlyArrayService>().AsSingleton();
		MultiBind<ISceneInitializer>().To<InstantiatingSceneInitializer<TickerUnityAdapter>>().AsSingleton();
	}
}
