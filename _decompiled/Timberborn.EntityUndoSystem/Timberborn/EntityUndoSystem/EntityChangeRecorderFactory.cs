using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.UndoSystem;

namespace Timberborn.EntityUndoSystem;

public class EntityChangeRecorderFactory
{
	private readonly EventBus _eventBus;

	private readonly IUndoRegistry _undoRegistry;

	private readonly UndoableEntityFactory _undoableEntityFactory;

	public EntityChangeRecorderFactory(EventBus eventBus, IUndoRegistry undoRegistry, UndoableEntityFactory undoableEntityFactory)
	{
		_eventBus = eventBus;
		_undoRegistry = undoRegistry;
		_undoableEntityFactory = undoableEntityFactory;
	}

	public EntityChangeRecorder CreateChangeRecorder(BaseComponent baseComponent)
	{
		if (_undoRegistry.UndoAllowed)
		{
			EntityComponent component = baseComponent.GetComponent<EntityComponent>();
			UndoableEntity preChangeUndoableEntity = _undoableEntityFactory.CreateInitialized(component);
			return new EntityChangeRecorder(_eventBus, _undoRegistry, _undoableEntityFactory, preChangeUndoableEntity);
		}
		return new EntityChangeRecorder(_eventBus, _undoRegistry, _undoableEntityFactory, null);
	}
}
