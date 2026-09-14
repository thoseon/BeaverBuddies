using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.Stockpiles;

namespace Timberborn.StockpilesUI;

internal class StockpileDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string CapacityLocKey = "Inventory.Capacity";

	private readonly ILoc _loc;

	private Stockpile _stockpile;

	public StockpileDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_stockpile = GetComponent<Stockpile>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (!_stockpile.Enabled)
		{
			string content = $"{SpecialStrings.RowStarter}{_loc.T(CapacityLocKey)} {_stockpile.MaxCapacity}";
			yield return EntityDescription.CreateTextSection(content, 70);
		}
	}
}
