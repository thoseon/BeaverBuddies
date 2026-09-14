using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.UndoSystem;

namespace Timberborn.EntityUndoSystem;

internal class EntityLifecycleUndoableRegistrar : IPostLoadableSingleton
{
	private readonly UndoableEntityFactory _undoableEntityFactory;

	private readonly IUndoRegistry _undoRegistry;

	private readonly EventBus _eventBus;

	public EntityLifecycleUndoableRegistrar(UndoableEntityFactory undoableEntityFactory, IUndoRegistry undoRegistry, EventBus eventBus)
	{
		_undoableEntityFactory = undoableEntityFactory;
		_undoRegistry = undoRegistry;
		_eventBus = eventBus;
	}

	public void PostLoad()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEntityCreated(EntityCreatedEvent entityCreatedEvent)
	{
		if (_undoRegistry.UndoAllowed && !_undoRegistry.IsProcessingStack)
		{
			RegisterCreatedEntity(entityCreatedEvent.Entity);
		}
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		if (_undoRegistry.UndoAllowed && !_undoRegistry.IsProcessingStack)
		{
			RegisterDeletedEntity(entityDeletedEvent.Entity);
		}
	}

	private void RegisterCreatedEntity(EntityComponent entity)
	{
		CreatedEntityUndoable undoable = new CreatedEntityUndoable(_undoableEntityFactory.CreateUninitialized(entity));
		_undoRegistry.RegisterStackedUndoable(undoable);
	}

	private void RegisterDeletedEntity(EntityComponent entity)
	{
		DeletedEntityUndoable undoable = new DeletedEntityUndoable(_undoableEntityFactory.CreateInitialized(entity));
		_undoRegistry.RegisterStackedUndoable(undoable);
	}
}
