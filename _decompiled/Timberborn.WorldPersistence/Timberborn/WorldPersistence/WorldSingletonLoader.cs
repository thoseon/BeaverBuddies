using Timberborn.Persistence;

namespace Timberborn.WorldPersistence;

internal class WorldSingletonLoader : ISingletonLoader
{
	private readonly ISerializedWorldSupplier _serializedWorldSupplier;

	public WorldSingletonLoader(ISerializedWorldSupplier serializedWorldSupplier)
	{
		_serializedWorldSupplier = serializedWorldSupplier;
	}

	public IObjectLoader GetSingleton(SingletonKey key)
	{
		return new ObjectLoader(_serializedWorldSupplier.Get().GetSingleton(key.Name));
	}

	public bool TryGetSingleton(SingletonKey key, out IObjectLoader objectLoader)
	{
		if (HasSingleton(key))
		{
			objectLoader = GetSingleton(key);
			return true;
		}
		objectLoader = null;
		return false;
	}

	private bool HasSingleton(SingletonKey key)
	{
		return _serializedWorldSupplier.Get().HasSingleton(key.Name);
	}
}
