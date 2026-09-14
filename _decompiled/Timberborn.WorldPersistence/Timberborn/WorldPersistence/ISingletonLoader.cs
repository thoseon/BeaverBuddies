using Timberborn.Persistence;

namespace Timberborn.WorldPersistence;

public interface ISingletonLoader
{
	IObjectLoader GetSingleton(SingletonKey key);

	bool TryGetSingleton(SingletonKey key, out IObjectLoader objectLoader);
}
