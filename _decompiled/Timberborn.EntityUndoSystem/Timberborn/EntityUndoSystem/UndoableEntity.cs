using System;
using Timberborn.EntitySystem;
using Timberborn.TemplateSystem;
using Timberborn.WorldPersistence;
using Timberborn.WorldSerialization;

namespace Timberborn.EntityUndoSystem;

public class UndoableEntity : IEquatable<UndoableEntity>
{
	private readonly EntityService _entityService;

	private readonly EntityRegistry _entityRegistry;

	private readonly TemplateNameMapper _templateNameMapper;

	private readonly UndoableEntitiesLoader _undoableEntitiesLoader;

	private readonly Guid _guid;

	private SerializedEntity _serializedEntity;

	public UndoableEntity(EntityService entityService, EntityRegistry entityRegistry, TemplateNameMapper templateNameMapper, UndoableEntitiesLoader undoableEntitiesLoader, Guid guid)
	{
		_entityService = entityService;
		_entityRegistry = entityRegistry;
		_templateNameMapper = templateNameMapper;
		_undoableEntitiesLoader = undoableEntitiesLoader;
		_guid = guid;
	}

	public void InitializeUndoableState()
	{
		if (_serializedEntity == null)
		{
			_serializedEntity = SerializeEntity(GetEntity());
		}
	}

	public void Delete()
	{
		if (TryGetEntity(out var entity))
		{
			_entityService.Delete(entity);
		}
	}

	public void Create()
	{
		if (_serializedEntity == null)
		{
			throw new InvalidOperationException("Cannot create entity without serialized data. Guid: " + _guid.ToString());
		}
		InstantiateEntity(_serializedEntity);
	}

	public void Reload()
	{
		_undoableEntitiesLoader.Reload(new InstantiatedSerializedEntity(GetEntity(), _serializedEntity));
	}

	public EntityComponent GetEntity()
	{
		if (TryGetEntity(out var entity))
		{
			return entity;
		}
		throw new InvalidOperationException("Entity with Guid " + _guid.ToString() + " not found.");
	}

	public bool Equals(UndoableEntity other)
	{
		if (other == null)
		{
			return false;
		}
		if (_guid != other._guid)
		{
			return false;
		}
		if (_serializedEntity == null || other._serializedEntity == null)
		{
			throw new InvalidOperationException("Cannot compare an uninitialized UndoableEntity. Guid: " + _guid.ToString());
		}
		return _serializedEntity.Equals(other._serializedEntity);
	}

	private bool TryGetEntity(out EntityComponent entity)
	{
		entity = _entityRegistry.GetEntity(_guid);
		return entity != null;
	}

	private static SerializedEntity SerializeEntity(EntityComponent entity)
	{
		string templateName = entity.GetComponent<TemplateSpec>().TemplateName;
		SerializedEntity serializedEntity = new SerializedEntity(entity.EntityId, templateName);
		SaveEntity(entity, serializedEntity);
		return serializedEntity;
	}

	private static void SaveEntity(EntityComponent entity, SerializedEntity output)
	{
		foreach (object allComponent in entity.AllComponents)
		{
			if (allComponent is IPersistentEntity persistentEntity)
			{
				persistentEntity.Save(new EntitySaver(output));
			}
		}
	}

	private void InstantiateEntity(SerializedEntity serializedEntity)
	{
		string templateName = serializedEntity.TemplateName;
		EntitySetup.Builder entitySetupBuilder = new EntitySetup.Builder(_templateNameMapper.GetTemplate(templateName).Blueprint).DisableInitialization().SetId(serializedEntity.Id);
		InstantiatedSerializedEntity entity = new InstantiatedSerializedEntity(_entityService.Instantiate(entitySetupBuilder), serializedEntity);
		_undoableEntitiesLoader.AddEntityForLoad(entity);
	}
}
