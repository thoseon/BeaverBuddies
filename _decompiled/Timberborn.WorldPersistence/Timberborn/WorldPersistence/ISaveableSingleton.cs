using Timberborn.SingletonSystem;

namespace Timberborn.WorldPersistence;

[Singleton]
public interface ISaveableSingleton
{
	void Save(ISingletonSaver singletonSaver);
}
