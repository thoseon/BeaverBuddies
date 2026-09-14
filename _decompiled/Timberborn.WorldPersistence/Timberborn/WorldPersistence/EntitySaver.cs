using Timberborn.Persistence;
using Timberborn.WorldSerialization;

namespace Timberborn.WorldPersistence;

public class EntitySaver : IEntitySaver
{
	private readonly SerializedEntity _serializedEntity;

	public EntitySaver(SerializedEntity serializedEntity)
	{
		_serializedEntity = serializedEntity;
	}

	public IObjectSaver GetComponent(ComponentKey componentKey)
	{
		return new ObjectSaver(_serializedEntity.GetOrAddComponent(componentKey.Name).Value);
	}

	public IObjectSaver GetComponent(ComponentKey componentKey, string suffix)
	{
		return GetComponent(componentKey.AddSuffix(suffix));
	}
}
