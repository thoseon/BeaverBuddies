using Timberborn.Persistence;
using Timberborn.WorldSerialization;

namespace Timberborn.WorldPersistence;

public class EntityLoader : IEntityLoader
{
	private readonly SerializedEntity _serializedEntity;

	public EntityLoader(SerializedEntity serializedEntity)
	{
		_serializedEntity = serializedEntity;
	}

	public IObjectLoader GetComponent(ComponentKey key)
	{
		return new ObjectLoader(_serializedEntity.GetComponent(key.Name).Value);
	}

	public IObjectLoader GetComponent(ComponentKey key, string suffix)
	{
		return GetComponent(key.AddSuffix(suffix));
	}

	public bool HasComponent(ComponentKey key)
	{
		return _serializedEntity.HasComponent(key.Name);
	}

	public bool HasComponent(ComponentKey key, string suffix)
	{
		return HasComponent(key.AddSuffix(suffix));
	}

	public bool TryGetComponent(ComponentKey key, out IObjectLoader objectLoader)
	{
		if (HasComponent(key))
		{
			objectLoader = GetComponent(key);
			return true;
		}
		objectLoader = null;
		return false;
	}

	public bool TryGetComponent(ComponentKey key, string suffix, out IObjectLoader objectLoader)
	{
		if (HasComponent(key, suffix))
		{
			objectLoader = GetComponent(key, suffix);
			return true;
		}
		objectLoader = null;
		return false;
	}
}
