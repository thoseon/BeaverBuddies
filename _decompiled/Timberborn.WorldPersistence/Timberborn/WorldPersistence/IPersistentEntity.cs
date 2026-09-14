namespace Timberborn.WorldPersistence;

public interface IPersistentEntity
{
	void Save(IEntitySaver entitySaver);

	void Load(IEntityLoader entityLoader);
}
