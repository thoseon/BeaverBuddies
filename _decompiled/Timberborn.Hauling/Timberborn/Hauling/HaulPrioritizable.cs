using Timberborn.BaseComponentSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.Hauling;

public class HaulPrioritizable : BaseComponent, IPersistentEntity, IDuplicable<HaulPrioritizable>, IDuplicable
{
	private static readonly ComponentKey HaulPrioritizableKey = new ComponentKey("HaulPrioritizable");

	private static readonly PropertyKey<bool> PrioritizedKey = new PropertyKey<bool>("Prioritized");

	public bool Prioritized { get; set; }

	public void Save(IEntitySaver entitySaver)
	{
		if (Prioritized)
		{
			entitySaver.GetComponent(HaulPrioritizableKey).Set(PrioritizedKey, Prioritized);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(HaulPrioritizableKey, out var objectLoader))
		{
			Prioritized = objectLoader.Get(PrioritizedKey);
		}
	}

	public void DuplicateFrom(HaulPrioritizable source)
	{
		Prioritized = source.Prioritized;
	}
}
