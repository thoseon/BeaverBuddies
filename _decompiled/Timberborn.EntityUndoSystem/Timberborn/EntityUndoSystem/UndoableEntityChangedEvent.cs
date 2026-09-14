using Timberborn.EntitySystem;

namespace Timberborn.EntityUndoSystem;

public class UndoableEntityChangedEvent
{
	public EntityComponent Entity { get; }

	public UndoableEntityChangedEvent(EntityComponent entity)
	{
		Entity = entity;
	}
}
