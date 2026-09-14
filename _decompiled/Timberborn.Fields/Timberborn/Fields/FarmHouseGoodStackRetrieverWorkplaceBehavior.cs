using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Buildings;
using Timberborn.GoodStackSystem;
using Timberborn.InventorySystem;
using Timberborn.Navigation;
using Timberborn.SimpleOutputBuildings;
using Timberborn.WorkSystem;

namespace Timberborn.Fields;

internal class FarmHouseGoodStackRetrieverWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private readonly GoodStackService<FarmHouse> _goodStackService;

	private BuildingAccessible _buildingAccessible;

	private Inventory _inventory;

	public FarmHouseGoodStackRetrieverWorkplaceBehavior(GoodStackService<FarmHouse> goodStackService)
	{
		_goodStackService = goodStackService;
	}

	public void Awake()
	{
		_buildingAccessible = GetComponent<BuildingAccessible>();
		_inventory = GetComponent<SimpleOutputInventory>().Inventory;
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		Accessible accessible = _buildingAccessible.Accessible;
		GoodStackRetrieverBehavior component = agent.GetComponent<GoodStackRetrieverBehavior>();
		Decision decision = component.StartRetrieving(_goodStackService, accessible, _inventory);
		if (!decision.ShouldReleaseNow)
		{
			return Decision.TransferNow(component, in decision);
		}
		return Decision.ReleaseNow();
	}
}
