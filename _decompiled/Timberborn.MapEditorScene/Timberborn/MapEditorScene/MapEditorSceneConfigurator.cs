using Bindito.Core;
using Timberborn.Demolishing;
using Timberborn.MapStateSystem;
using Timberborn.Navigation;
using Timberborn.WorldPersistence;

namespace Timberborn.MapEditorScene;

[Context("MapEditor")]
internal class MapEditorSceneConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Accessible>().AsTransient();
		Bind<Demolishable>().AsTransient();
		Bind<AccessibleDemolishableReacher>().AsTransient();
		Bind<MapEditorCameraCenterer>().AsSingleton();
		Bind<ISerializedWorldSupplier>().To<MapEditorSerializedWorldSupplier>().AsSingleton();
		Bind<INavMeshService>().To<DummyNavMeshService>().AsSingleton();
		Bind<INavigationService>().To<DummyNavigationService>().AsSingleton();
		Bind<INavigationCachingService>().To<DummyNavigationCachingService>().AsSingleton();
		Bind<INavigationDebuggingService>().To<DummyNavigationDebuggingService>().AsSingleton();
		Bind<INavMeshListenerEntityRegistry>().To<DummyNavMeshListenerEntityRegistry>().AsSingleton();
		Bind<MapEditorMode>().ToInstance(MapEditorMode.MapEditorInstance());
	}
}
