using System.Collections.Generic;
using Timberborn.EntitySystem;

namespace Timberborn.WorldPersistence;

public interface IEntityBatchLoader
{
	void BatchLoadEntities(IEnumerable<EntityComponent> entities);
}
