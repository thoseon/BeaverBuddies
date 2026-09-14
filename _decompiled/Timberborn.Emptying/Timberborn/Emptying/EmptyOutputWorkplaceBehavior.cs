using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.InventorySystem;
using Timberborn.WorkSystem;

namespace Timberborn.Emptying;

public class EmptyOutputWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private Inventories _inventories;

	public void Awake()
	{
		_inventories = GetComponent<Inventories>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		EmptyingStarter component = agent.GetComponent<EmptyingStarter>();
		foreach (Inventory enabledInventory in _inventories.EnabledInventories)
		{
			if (enabledInventory.OutputGoods.Count > 0 && component.StartEmptying(enabledInventory))
			{
				return Decision.ReleaseNextTick();
			}
		}
		return Decision.ReleaseNow();
	}
}
