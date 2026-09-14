using Timberborn.Persistence;

namespace Timberborn.WorldPersistence;

public interface IEntitySaver
{
	IObjectSaver GetComponent(ComponentKey key);

	IObjectSaver GetComponent(ComponentKey key, string suffix);
}
