using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.Planting;
using Timberborn.WorldPersistence;

namespace Timberborn.Forestry;

public class Forester : BaseComponent, IPersistentEntity, IDuplicable<Forester>, IDuplicable, IPlantingSpotValidator
{
	private static readonly ComponentKey ForesterKey = new ComponentKey("Forester");

	private static readonly PropertyKey<bool> ReplantDeadTreesKey = new PropertyKey<bool>("ReplantDeadTrees");

	private readonly TreeCuttingArea _treeCuttingArea;

	public bool ReplantDeadTrees { get; private set; }

	public Forester(TreeCuttingArea treeCuttingArea)
	{
		_treeCuttingArea = treeCuttingArea;
	}

	public void SetReplantDeadTrees(bool replantDeadTrees)
	{
		ReplantDeadTrees = replantDeadTrees;
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(ForesterKey).Set(ReplantDeadTreesKey, ReplantDeadTrees);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ForesterKey);
		ReplantDeadTrees = component.Get(ReplantDeadTreesKey);
	}

	public void DuplicateFrom(Forester source)
	{
		SetReplantDeadTrees(source.ReplantDeadTrees);
	}

	public bool Validate(PlantingSpot spot)
	{
		BlockObject plantingBlocker = spot.PlantingBlocker;
		if ((bool)plantingBlocker)
		{
			if (ReplantDeadTrees)
			{
				TreeComponent component = plantingBlocker.GetComponent<TreeComponent>();
				if (component != null && component.CanBeReplaced)
				{
					return !_treeCuttingArea.IsInCuttingArea(spot.Coordinates);
				}
			}
			return false;
		}
		return true;
	}
}
