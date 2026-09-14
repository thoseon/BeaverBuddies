using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Stockpiles;

namespace Timberborn.GameStockpiles;

internal class FixedStockpileDeletionBlocker : BaseComponent, IAwakableComponent, IBlockObjectDeletionBlocker
{
	private Stockpile _stockpile;

	public bool NoForcedDelete => false;

	public bool IsStackedDeletionBlocked => IsDeletionBlocked;

	public bool IsDeletionBlocked
	{
		get
		{
			if (_stockpile.Inventory.Enabled)
			{
				return !_stockpile.Inventory.IsEmpty;
			}
			return false;
		}
	}

	public string ReasonLocKey => "DeletionBlocker.StockpileNotEmpty";

	public void Awake()
	{
		_stockpile = GetComponent<Stockpile>();
	}
}
