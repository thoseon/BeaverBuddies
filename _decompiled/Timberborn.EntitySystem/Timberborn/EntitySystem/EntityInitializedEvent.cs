namespace Timberborn.EntitySystem;

public class EntityInitializedEvent
{
	public EntityComponent Entity { get; }

	public EntityInitializedEvent(EntityComponent entity)
	{
		Entity = entity;
	}
}
