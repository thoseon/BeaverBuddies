using Timberborn.EntitySystem;
using Timberborn.WorldSerialization;

namespace Timberborn.WorldPersistence;

public class InstantiatedSerializedEntity
{
	public EntityComponent Entity { get; }

	public SerializedEntity SerializedEntity { get; }

	public InstantiatedSerializedEntity(EntityComponent entity, SerializedEntity serializedEntity)
	{
		Entity = entity;
		SerializedEntity = serializedEntity;
	}
}
