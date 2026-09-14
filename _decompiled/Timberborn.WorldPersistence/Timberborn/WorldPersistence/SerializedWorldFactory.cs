using System;
using System.Collections.Generic;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.Versioning;
using Timberborn.WorldSerialization;

namespace Timberborn.WorldPersistence;

public class SerializedWorldFactory
{
	private readonly TemplateNameRetriever _templateNameRetriever;

	private readonly EntityRegistry _entityRegistry;

	private readonly ISingletonRepository _singletonRepository;

	public SerializedWorldFactory(TemplateNameRetriever templateNameRetriever, EntityRegistry entityRegistry, ISingletonRepository singletonRepository)
	{
		_templateNameRetriever = templateNameRetriever;
		_entityRegistry = entityRegistry;
		_singletonRepository = singletonRepository;
	}

	public SerializedWorld Create()
	{
		SerializedWorld serializedWorld = new SerializedWorld(GameVersions.CurrentVersion);
		SaveEntities(serializedWorld);
		SaveSingletons(serializedWorld);
		return serializedWorld;
	}

	public SerializedWorld Create(IEnumerable<ISaveableSingleton> singletons)
	{
		SerializedWorld serializedWorld = new SerializedWorld(GameVersions.CurrentVersion);
		SaveSingletons(serializedWorld, singletons);
		return serializedWorld;
	}

	private void SaveEntities(SerializedWorld serializedWorld)
	{
		foreach (EntityComponent entity in _entityRegistry.Entities)
		{
			string templateName = _templateNameRetriever.GetTemplateName(entity);
			SerializedEntity serializedEntity = new SerializedEntity(entity.GetComponent<EntityComponent>().EntityId, templateName);
			SaveEntity(entity, serializedEntity);
			if (serializedEntity.HasComponents())
			{
				serializedWorld.AddEntity(serializedEntity);
			}
		}
	}

	private void SaveEntity(EntityComponent entity, SerializedEntity output)
	{
		foreach (object allComponent in entity.AllComponents)
		{
			if (allComponent is ISaveableSingleton)
			{
				throw new ArgumentException("An entity must not implement ISaveableSingleton: " + allComponent.GetType().Name);
			}
			if (allComponent is IPersistentEntity persistentEntity)
			{
				persistentEntity.Save(new EntitySaver(output));
			}
		}
	}

	private void SaveSingletons(SerializedWorld serializedWorld)
	{
		SaveSingletons(serializedWorld, _singletonRepository.GetSingletons<ISaveableSingleton>());
	}

	private void SaveSingletons(SerializedWorld serializedWorld, IEnumerable<ISaveableSingleton> singletons)
	{
		SingletonSaver singletonSaver = new SingletonSaver(serializedWorld);
		foreach (ISaveableSingleton singleton in singletons)
		{
			singleton.Save(singletonSaver);
		}
	}
}
