using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.InventorySystem;
using Timberborn.RecoverableGoodSystem;

namespace Timberborn.ConstructionSites;

internal class ConstructionSiteRecoverableGoodMultiplier : BaseComponent, IAwakableComponent, IRecoverableGoodMultiplier
{
	private readonly GoodRecoveryRateService _goodRecoveryRateService;

	private BlockObject _blockObject;

	private ConstructionSite _constructionSite;

	public ConstructionSiteRecoverableGoodMultiplier(GoodRecoveryRateService goodRecoveryRateService)
	{
		_goodRecoveryRateService = goodRecoveryRateService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_constructionSite = GetComponent<ConstructionSite>();
	}

	public float GetMultiplierForInventory(Inventory inventory)
	{
		if (_blockObject.IsFinished && inventory == _constructionSite.Inventory)
		{
			return _goodRecoveryRateService.DemolishableRecoveryRate;
		}
		return 1f;
	}
}
