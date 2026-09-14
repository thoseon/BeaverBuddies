using Timberborn.Persistence;
using Timberborn.WorldSerialization;

namespace Timberborn.WorldPersistence;

internal class SingletonSaver : ISingletonSaver
{
	private readonly SerializedWorld _serializedWorld;

	public SingletonSaver(SerializedWorld serializedWorld)
	{
		_serializedWorld = serializedWorld;
	}

	public IObjectSaver GetSingleton(SingletonKey key)
	{
		return new ObjectSaver(_serializedWorld.GetOrAddSingleton(key.Name));
	}
}
