using Bindito.Core;
using Bindito.Unity;

namespace Timberborn.Physics;

[Context("Game")]
[Context("MapEditor")]
internal class PhysicsConfigurator : Configurator
{
	protected override void Configure()
	{
		MultiBind<ISceneInitializer>().To<InstantiatingSceneInitializer<TransformSyncServiceUnityAdapter>>().AsSingleton();
	}
}
