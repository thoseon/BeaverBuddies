using Timberborn.EntitySystem;

namespace Timberborn.EntityNaming;

public class EntityNameChangedEvent
{
	public EntityComponent Entity { get; }

	public EntityNameChangedEvent(EntityComponent entity)
	{
		Entity = entity;
	}
}
