using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;
using Timberborn.EntitySystem;
using Timberborn.Goods;

namespace Timberborn.Carrying;

public class Overburdenable : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly IGoodService _goodService;

	private BonusManager _bonusManager;

	private GoodCarrier _goodCarrier;

	private OverburdenableSpec _overburdenableSpec;

	private bool _isOverburdened;

	public Overburdenable(IGoodService goodService)
	{
		_goodService = goodService;
	}

	public void Awake()
	{
		_bonusManager = GetComponent<BonusManager>();
		_goodCarrier = GetComponent<GoodCarrier>();
		_overburdenableSpec = GetComponent<OverburdenableSpec>();
	}

	public void InitializeEntity()
	{
		_goodCarrier.CarriedGoodsChanged += OnCarriedGoodsChanged;
		CheckIfOverburdened(_goodCarrier.CarriedGood.GoodAmount);
	}

	private void OnCarriedGoodsChanged(object sender, CarriedGoodsChangedEventArgs e)
	{
		CheckIfOverburdened(e.CarriedGood.GoodAmount);
	}

	private void CheckIfOverburdened(GoodAmount goodAmount)
	{
		if (!_isOverburdened && _goodCarrier.IsCarrying && IsCarryingTooMuch(goodAmount))
		{
			AddOverburdenedBonuses();
		}
		else if (_isOverburdened && !_goodCarrier.IsCarrying)
		{
			RemoveOverburdenedBonuses();
		}
	}

	private bool IsCarryingTooMuch(GoodAmount goodAmount)
	{
		int weight = _goodService.GetGood(goodAmount.GoodId).Weight;
		return goodAmount.Amount * weight > _goodCarrier.LiftingCapacity;
	}

	private void AddOverburdenedBonuses()
	{
		_bonusManager.AddBonuses(_overburdenableSpec.OverburdenedBonuses);
		_isOverburdened = true;
	}

	private void RemoveOverburdenedBonuses()
	{
		_bonusManager.RemoveBonuses(_overburdenableSpec.OverburdenedBonuses);
		_isOverburdened = false;
	}
}
