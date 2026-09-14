namespace Timberborn.EntitySystem;

public class EntityDeletedEvent
{
	public EntityComponent Entity { get; }

	public EntityDeletedEvent(EntityComponent entity)
	{
		Entity = entity;
	}
}
