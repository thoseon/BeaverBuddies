using Timberborn.SingletonSystem;
using Timberborn.UndoSystem;

namespace Timberborn.EntityUndoSystem;

internal class ChangedEntityUndoable : IUndoable
{
	private readonly EventBus _eventBus;

	private readonly UndoableEntity _preChangeUndoableEntity;

	private readonly UndoableEntity _postChangeUndoableEntity;

	public ChangedEntityUndoable(EventBus eventBus, UndoableEntity preChangeUndoableEntity, UndoableEntity postChangeUndoableEntity)
	{
		_eventBus = eventBus;
		_preChangeUndoableEntity = preChangeUndoableEntity;
		_postChangeUndoableEntity = postChangeUndoableEntity;
	}

	public void Undo()
	{
		_preChangeUndoableEntity.Reload();
		_eventBus.Post(new UndoableEntityChangedEvent(_preChangeUndoableEntity.GetEntity()));
	}

	public void Redo()
	{
		_postChangeUndoableEntity.Reload();
		_eventBus.Post(new UndoableEntityChangedEvent(_postChangeUndoableEntity.GetEntity()));
	}
}
