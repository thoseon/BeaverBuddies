using Timberborn.BaseComponentSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Emptying;

namespace Timberborn.StockpilePrioritySystem;

public class StockpilePriority : BaseComponent, IAwakableComponent, IDuplicable<StockpilePriority>, IDuplicable
{
	private Emptiable _emptiable;

	private GoodObtainer _goodObtainer;

	private GoodSupplier _goodSupplier;

	public bool IsAcceptActive
	{
		get
		{
			if (!_emptiable.IsMarkedForEmptying && !_goodObtainer.IsObtaining)
			{
				return !_goodSupplier.IsSupplying;
			}
			return false;
		}
	}

	public bool IsEmptyActive => _emptiable.IsMarkedForEmptying;

	public bool IsObtainActive => _goodObtainer.IsObtaining;

	public bool IsSupplyActive => _goodSupplier.IsSupplying;

	public void Awake()
	{
		_emptiable = GetComponent<Emptiable>();
		_goodObtainer = GetComponent<GoodObtainer>();
		_goodSupplier = GetComponent<GoodSupplier>();
	}

	public void Accept()
	{
		Reset();
	}

	public void Empty()
	{
		Reset();
		_emptiable.MarkForEmptyingWithStatus();
	}

	public void Obtain()
	{
		Reset();
		_goodObtainer.EnableObtaining();
	}

	public void Supply()
	{
		Reset();
		_goodSupplier.EnableSupplying();
	}

	public void DuplicateFrom(StockpilePriority source)
	{
		if (source.IsAcceptActive)
		{
			Accept();
		}
		else if (source.IsEmptyActive)
		{
			Empty();
		}
		else if (source.IsObtainActive)
		{
			Obtain();
		}
		else if (source.IsSupplyActive)
		{
			Supply();
		}
	}

	private void Reset()
	{
		_emptiable.UnmarkForEmptying();
		_goodObtainer.DisableObtaining();
		_goodSupplier.DisableSupplying();
	}
}
