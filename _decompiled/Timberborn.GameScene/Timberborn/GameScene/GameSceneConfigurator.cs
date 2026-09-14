using Bindito.Core;
using Timberborn.MapStateSystem;
using Timberborn.TickSystem;
using Timberborn.UndoSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.GameScene;

[Context("Game")]
internal class GameSceneConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DateSalter>().AsSingleton();
		Bind<ITickingMode>().To<GameSceneTickingMode>().AsSingleton();
		Bind<ISerializedWorldSupplier>().To<GameSceneSerializedWorldSupplier>().AsSingleton();
		Bind<MapEditorMode>().ToInstance(MapEditorMode.NonMapEditorInstance());
		Bind<IUndoRegistry>().To<DummyUndoRegistry>().AsSingleton();
	}
}
