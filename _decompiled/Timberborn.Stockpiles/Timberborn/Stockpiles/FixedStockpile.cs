using Timberborn.BaseComponentSystem;
using Timberborn.Goods;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.Stockpiles;

public class FixedStockpile : BaseComponent, IPersistentEntity
{
	private static readonly ComponentKey FixedStockpileKey = new ComponentKey("FixedStockpile");

	private static readonly PropertyKey<string> FixedGoodIdKey = new PropertyKey<string>("FixedGoodId");

	private readonly IGoodService _goodService;

	private string _fixedGoodId = string.Empty;

	public bool IsFixedGoodInvalid
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(_fixedGoodId))
			{
				return !_goodService.HasGood(_fixedGoodId);
			}
			return false;
		}
	}

	public FixedStockpile(IGoodService goodService)
	{
		_goodService = goodService;
	}

	public void SetFixedGood(string goodId)
	{
		_fixedGoodId = goodId;
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(FixedStockpileKey).Set(FixedGoodIdKey, _fixedGoodId);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(FixedStockpileKey);
		_fixedGoodId = component.Get(FixedGoodIdKey);
	}
}
