using Timberborn.Persistence;

namespace Timberborn.WorldPersistence;

public interface IEntityLoader
{
	IObjectLoader GetComponent(ComponentKey key);

	IObjectLoader GetComponent(ComponentKey key, string suffix);

	bool HasComponent(ComponentKey key);

	bool HasComponent(ComponentKey key, string suffix);

	bool TryGetComponent(ComponentKey key, out IObjectLoader objectLoader);

	bool TryGetComponent(ComponentKey key, string suffix, out IObjectLoader objectLoader);
}
