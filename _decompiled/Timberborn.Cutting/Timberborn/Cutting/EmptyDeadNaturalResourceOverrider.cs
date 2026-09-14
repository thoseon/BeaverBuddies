using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.GoodStackSystem;
using Timberborn.NaturalResourcesLifecycle;

namespace Timberborn.Cutting;

public class EmptyDeadNaturalResourceOverrider : BaseComponent, IAwakableComponent
{
	private Cuttable _cuttable;

	private GoodStack _goodStack;

	private BlockObject _blockObject;

	public void Awake()
	{
		_cuttable = GetComponent<Cuttable>();
		_goodStack = GetComponent<GoodStack>();
		_blockObject = GetComponent<BlockObject>();
		GetComponent<LivingNaturalResource>().Died += delegate
		{
			MakeOverridable();
		};
		GetComponent<LivingNaturalResource>().ReversedDeath += delegate
		{
			MakeNonOverridable();
		};
	}

	private void MakeNonOverridable()
	{
		_blockObject.MakeNonOverridable();
	}

	private void MakeOverridable()
	{
		if (!CuttableWithYield() && !GoodStackWithGood())
		{
			_blockObject.MakeOverridable();
		}
	}

	private bool CuttableWithYield()
	{
		if ((bool)_cuttable && _cuttable.Yielder.Yield.Amount > 0)
		{
			return !_cuttable.RemoveOnCut;
		}
		return false;
	}

	private bool GoodStackWithGood()
	{
		if ((bool)_goodStack)
		{
			return !_goodStack.Inventory.IsEmpty;
		}
		return false;
	}
}
