using Bindito.Core;
using Timberborn.UndoSystem;

namespace Timberborn.EntityUndoSystem;

[Context("Game")]
[Context("MapEditor")]
internal class EntityUndoSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<EntityLifecycleUndoableRegistrar>().AsSingleton();
		Bind<UndoableEntitiesLoader>().AsSingleton();
		Bind<EntityChangeRecorderFactory>().AsSingleton();
		Bind<UndoableEntityFactory>().AsSingleton();
		MultiBind<IUndoPostprocessor>().ToExisting<UndoableEntitiesLoader>();
	}
}
