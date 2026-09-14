namespace Timberborn.EntitySystem;

public class EntityCreatedEvent
{
	public EntityComponent Entity { get; }

	public EntityCreatedEvent(EntityComponent entity)
	{
		Entity = entity;
	}
}
