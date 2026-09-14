using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.InventorySystem;
using Timberborn.WorkSystem;

namespace Timberborn.Emptying;

public class EmptyInventoriesWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private Inventories _inventories;

	private Emptiable _emptiable;

	public void Awake()
	{
		_inventories = GetComponent<Inventories>();
		_emptiable = GetComponent<Emptiable>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (_emptiable.IsMarkedForEmptying)
		{
			EmptyingStarter component = agent.GetComponent<EmptyingStarter>();
			foreach (Inventory enabledInventory in _inventories.EnabledInventories)
			{
				if (component.StartEmptying(enabledInventory))
				{
					return Decision.ReleaseNextTick();
				}
			}
		}
		return Decision.ReleaseNow();
	}
}
