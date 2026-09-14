using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;
using Timberborn.Goods;

namespace Timberborn.Yielding;

public class YieldRemovalSuccessValidator : BaseComponent, IAwakableComponent
{
	private readonly YieldRemovalChanceBonusService _yieldRemovalChanceBonusService;

	private BonusManager _bonusManager;

	private YielderRemover _yielderRemover;

	private RemoveYieldExecutor _removeYieldExecutor;

	public YieldRemovalSuccessValidator(YieldRemovalChanceBonusService yieldRemovalChanceBonusService)
	{
		_yieldRemovalChanceBonusService = yieldRemovalChanceBonusService;
	}

	public void Awake()
	{
		_bonusManager = GetComponent<BonusManager>();
		_yielderRemover = GetComponent<YielderRemover>();
	}

	public bool ValidateYieldSuccess()
	{
		GoodAmount yield = _yielderRemover.ReservedYielder.Yield;
		return _yieldRemovalChanceBonusService.CheckYieldRemovalSuccess(_bonusManager, yield.GoodId);
	}
}
