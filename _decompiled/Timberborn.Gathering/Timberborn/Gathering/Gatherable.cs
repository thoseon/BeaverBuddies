using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.GoodStackSystem;
using Timberborn.Goods;
using Timberborn.WorldPersistence;
using Timberborn.Yielding;

namespace Timberborn.Gathering;

public class Gatherable : BaseComponent, IAwakableComponent, IDeletableEntity, IPostInitializableEntity
{
	private readonly GoodStackService<GathererFlag> _goodStackService;

	private readonly IGoodService _goodService;

	private GoodStack _goodStack;

	private GatherableSpec _gatherableSpec;

	private GatherableModel _gatherableModel;

	public Yielder Yielder { get; private set; }

	public float YieldGrowthTimeInDays => _gatherableSpec.YieldGrowthTimeInDays;

	public YielderSpec YielderSpec => _gatherableSpec.Yielder;

	public bool UsableWithCurrentFeatureToggles => _goodService.HasGood(YielderSpec.Yield.Id);

	public event EventHandler Gathered;

	public Gatherable(GoodStackService<GathererFlag> goodStackService, IGoodService goodService)
	{
		_goodStackService = goodStackService;
		_goodService = goodService;
	}

	public void Awake()
	{
		_goodStack = GetComponent<GoodStack>();
		_gatherableSpec = GetComponent<GatherableSpec>();
		_gatherableModel = GetComponent<GatherableModel>();
		Yielder = this.GetNamedComponent<Yielder>(YielderSpec.YielderComponentName);
	}

	public void PostInitializeEntity()
	{
		if (UsableWithCurrentFeatureToggles)
		{
			Yielder.YieldDecreased += delegate
			{
				OnYieldDecreased();
			};
			_goodStack.GoodStackDisabled += delegate
			{
				RemoveGoodStack();
			};
			AddGoodStack();
		}
	}

	public void DeleteEntity()
	{
		RemoveGoodStack();
	}

	public void UpdateModel()
	{
		_gatherableModel.UpdateMaterial(Yielder.IsYielding);
	}

	private void OnYieldDecreased()
	{
		if (Yielder.Yield.Amount > 0)
		{
			_goodStack.EnableGoodStack(Yielder.Yield);
			Yielder.RemoveRemainingYield();
		}
		AddGoodStack();
		Gathered?.Invoke(this, EventArgs.Empty);
	}

	private void AddGoodStack()
	{
		if (_goodStack.Enabled)
		{
			_goodStackService.Add(_goodStack);
		}
	}

	private void RemoveGoodStack()
	{
		_goodStackService.Remove(_goodStack);
	}
}
