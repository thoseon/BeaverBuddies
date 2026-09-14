using Timberborn.Persistence;

namespace Timberborn.WorldPersistence;

public interface ISingletonSaver
{
	IObjectSaver GetSingleton(SingletonKey key);
}
