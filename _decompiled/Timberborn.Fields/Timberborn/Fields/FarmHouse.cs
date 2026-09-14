using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.Planting;
using Timberborn.WorldPersistence;

namespace Timberborn.Fields;

public class FarmHouse : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<FarmHouse>, IDuplicable, IFinishedStateListener, IPlantingSpotValidator
{
	private static readonly ComponentKey FarmHouseKey = new ComponentKey("FarmHouse");

	private static readonly PropertyKey<bool> PlantingPrioritizedKey = new PropertyKey<bool>("PlantingPrioritized");

	public bool PlantingPrioritized { get; private set; } = true;

	public void Awake()
	{
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public void PrioritizePlanting()
	{
		PlantingPrioritized = true;
	}

	public void UnprioritizePlanting()
	{
		PlantingPrioritized = false;
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(FarmHouseKey).Set(PlantingPrioritizedKey, PlantingPrioritized);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(FarmHouseKey);
		PlantingPrioritized = component.Get(PlantingPrioritizedKey);
	}

	public void DuplicateFrom(FarmHouse source)
	{
		PlantingPrioritized = source.PlantingPrioritized;
	}

	public bool Validate(PlantingSpot spot)
	{
		return !spot.PlantingBlocker;
	}
}
