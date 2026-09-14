using Timberborn.BaseComponentSystem;
using Timberborn.DeconstructionSystem;
using Timberborn.EntitySystem;
using Timberborn.InventorySystem;
using Timberborn.RecoverableGoodSystem;
using Timberborn.Stockpiles;

namespace Timberborn.GameStockpiles;

internal class FixedStockpileRemover : BaseComponent, IInitializableEntity, IRecoverableGoodMultiplier
{
	private readonly EntityService _entityService;

	private bool _isRemoved;

	public FixedStockpileRemover(EntityService entityService)
	{
		_entityService = entityService;
	}

	public void InitializeEntity()
	{
		if (GetComponent<FixedStockpile>().IsFixedGoodInvalid)
		{
			GetComponent<Deconstructible>().DisableDeconstruction();
			_isRemoved = true;
			_entityService.Delete(this);
		}
	}

	public float GetMultiplierForInventory(Inventory inventory)
	{
		return (!_isRemoved) ? 1 : 0;
	}
}
