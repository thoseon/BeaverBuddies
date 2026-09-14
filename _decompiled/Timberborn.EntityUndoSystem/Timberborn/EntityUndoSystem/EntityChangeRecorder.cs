using System;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.UndoSystem;

namespace Timberborn.EntityUndoSystem;

public class EntityChangeRecorder : IDisposable
{
	private readonly EventBus _eventBus;

	private readonly IUndoRegistry _undoRegistry;

	private readonly UndoableEntityFactory _undoableEntityFactory;

	private readonly UndoableEntity _preChangeUndoableEntity;

	public EntityChangeRecorder(EventBus eventBus, IUndoRegistry undoRegistry, UndoableEntityFactory undoableEntityFactory, UndoableEntity preChangeUndoableEntity)
	{
		_eventBus = eventBus;
		_undoRegistry = undoRegistry;
		_undoableEntityFactory = undoableEntityFactory;
		_preChangeUndoableEntity = preChangeUndoableEntity;
	}

	public void Dispose()
	{
		if (_undoRegistry.UndoAllowed)
		{
			EntityComponent entity = _preChangeUndoableEntity.GetEntity();
			UndoableEntity undoableEntity = _undoableEntityFactory.CreateInitialized(entity);
			if (!_preChangeUndoableEntity.Equals(undoableEntity))
			{
				ChangedEntityUndoable undoable = new ChangedEntityUndoable(_eventBus, _preChangeUndoableEntity, undoableEntity);
				_undoRegistry.RegisterSingleUndoable(undoable);
			}
		}
	}
}
