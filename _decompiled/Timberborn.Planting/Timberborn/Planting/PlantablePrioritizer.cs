using System;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.Planting;

public class PlantablePrioritizer : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<PlantablePrioritizer>, IDuplicable, IFinishedStateListener
{
	private static readonly ComponentKey PlantablePrioritizerKey = new ComponentKey("PlantablePrioritizer");

	private static readonly PropertyKey<string> PrioritizedPlantableKey = new PropertyKey<string>("PrioritizedPlantable");

	private PlanterBuilding _planterBuilding;

	public PlantableSpec PrioritizedPlantableSpec { get; private set; }

	public event EventHandler PrioritizedPlantableChanged;

	public void Awake()
	{
		_planterBuilding = GetComponent<PlanterBuilding>();
		DisableComponent();
	}

	public void PrioritizePlantable(PlantableSpec plantableSpec)
	{
		PrioritizedPlantableSpec = plantableSpec;
		PrioritizedPlantableChanged?.Invoke(this, EventArgs.Empty);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (PrioritizedPlantableSpec != null)
		{
			entitySaver.GetComponent(PlantablePrioritizerKey).Set(PrioritizedPlantableKey, PrioritizedPlantableSpec.TemplateName);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(PlantablePrioritizerKey, out var objectLoader))
		{
			string templateName = objectLoader.Get(PrioritizedPlantableKey);
			PrioritizedPlantableSpec = _planterBuilding.AllowedPlantables.SingleOrDefault((PlantableSpec plantable) => plantable.TemplateName == templateName);
		}
	}

	public void DuplicateFrom(PlantablePrioritizer source)
	{
		PlantableSpec prioritizedPlantableSpec = source.PrioritizedPlantableSpec;
		if (prioritizedPlantableSpec == null || _planterBuilding.AllowedPlantables.Contains(prioritizedPlantableSpec))
		{
			PrioritizePlantable(prioritizedPlantableSpec);
		}
	}
}
