using Timberborn.BaseComponentSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.Attractions;

public class AttractionAttender : BaseComponent, IPersistentEntity
{
	private static readonly ComponentKey AttractionAttenderKey = new ComponentKey("AttractionAttender");

	private static readonly PropertyKey<bool> FirstVisitKey = new PropertyKey<bool>("FirstVisit");

	public bool FirstVisit { get; set; }

	public void Save(IEntitySaver entitySaver)
	{
		if (FirstVisit)
		{
			entitySaver.GetComponent(AttractionAttenderKey).Set(FirstVisitKey, FirstVisit);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(AttractionAttenderKey, out var objectLoader))
		{
			FirstVisit = objectLoader.Get(FirstVisitKey);
		}
	}
}
