using Timberborn.WorldPersistence;

namespace Timberborn.BehaviorSystem;

public interface IExecutor
{
	ExecutorStatus Tick(float deltaTimeInHours);

	void Save(IEntitySaver entitySaver);

	void Load(IEntityLoader entityLoader);
}
