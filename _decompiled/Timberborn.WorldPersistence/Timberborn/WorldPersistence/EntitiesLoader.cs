using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Timberborn.WorldPersistence;

public class EntitiesLoader
{
	private readonly ImmutableArray<IEntityBatchLoader> _entityBatchLoaders;

	public EntitiesLoader(IEnumerable<IEntityBatchLoader> entityBatchLoaders)
	{
		_entityBatchLoaders = entityBatchLoaders.ToImmutableArray();
	}

	public void LoadAndInitialize(ICollection<InstantiatedSerializedEntity> entities)
	{
		Load(entities);
		BatchLoad(entities);
		PreInitialize(entities);
		Initialize(entities);
		PostInitialize(entities);
	}

	public void Load(ICollection<InstantiatedSerializedEntity> entities)
	{
		foreach (InstantiatedSerializedEntity entity in entities)
		{
			Load(entity);
		}
	}

	public void PostLoad(ICollection<InstantiatedSerializedEntity> entities)
	{
		foreach (InstantiatedSerializedEntity entity in entities)
		{
			entity.Entity.PostLoad();
		}
	}

	private void BatchLoad(ICollection<InstantiatedSerializedEntity> entities)
	{
		foreach (IEntityBatchLoader entityBatchLoader in _entityBatchLoaders)
		{
			entityBatchLoader.BatchLoadEntities(entities.Select((InstantiatedSerializedEntity entity) => entity.Entity));
		}
	}

	private static void PreInitialize(IEnumerable<InstantiatedSerializedEntity> entities)
	{
		foreach (InstantiatedSerializedEntity entity in entities)
		{
			entity.Entity.PreInitialize();
		}
	}

	private static void Initialize(IEnumerable<InstantiatedSerializedEntity> entities)
	{
		foreach (InstantiatedSerializedEntity entity in entities)
		{
			entity.Entity.Initialize();
		}
	}

	private static void PostInitialize(IEnumerable<InstantiatedSerializedEntity> entities)
	{
		foreach (InstantiatedSerializedEntity entity in entities)
		{
			entity.Entity.PostInitialize();
		}
	}

	private static void Load(InstantiatedSerializedEntity serializedEntity)
	{
		foreach (IPersistentEntity item in serializedEntity.Entity.GetComponentsAllocating<IPersistentEntity>())
		{
			EntityLoader entityLoader = new EntityLoader(serializedEntity.SerializedEntity);
			item.Load(entityLoader);
		}
	}
}
