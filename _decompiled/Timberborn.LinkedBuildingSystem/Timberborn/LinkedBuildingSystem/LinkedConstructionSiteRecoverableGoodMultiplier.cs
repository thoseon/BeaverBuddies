using Timberborn.BaseComponentSystem;
using Timberborn.ConstructionSites;
using Timberborn.InventorySystem;
using Timberborn.RecoverableGoodSystem;

namespace Timberborn.LinkedBuildingSystem;

internal class LinkedConstructionSiteRecoverableGoodMultiplier : BaseComponent, IAwakableComponent, IRecoverableGoodMultiplier
{
	private ConstructionSite _constructionSite;

	public void Awake()
	{
		_constructionSite = GetComponent<ConstructionSite>();
	}

	public float GetMultiplierForInventory(Inventory inventory)
	{
		if (inventory != _constructionSite.Inventory)
		{
			return 1f;
		}
		return 0.5f;
	}
}
