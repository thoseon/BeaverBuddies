using Timberborn.BaseComponentSystem;
using Timberborn.Cutting;
using Timberborn.EntitySystem;
using Timberborn.GoodStackSystem;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.Yielding;

namespace Timberborn.Fields;

public class Crop : BaseComponent, IAwakableComponent, IPostInitializableEntity, IDeletableEntity
{
	private readonly GoodStackService<FarmHouse> _goodStackService;

	private GoodStack _goodStack;

	private Cuttable _cuttable;

	public Yielder Yielder => _cuttable.Yielder;

	public Crop(GoodStackService<FarmHouse> goodStackService)
	{
		_goodStackService = goodStackService;
	}

	public void Awake()
	{
		_cuttable = GetComponent<Cuttable>();
		_goodStack = GetComponent<GoodStack>();
	}

	public void PostInitializeEntity()
	{
		LivingNaturalResource component = GetComponent<LivingNaturalResource>();
		if (component.IsDead)
		{
			Disable();
		}
		else
		{
			component.Died += delegate
			{
				Disable();
			};
		}
		_cuttable.WasCut += delegate
		{
			AddGoodStack();
		};
		_goodStack.GoodStackDisabled += delegate
		{
			RemoveGoodStack();
		};
		AddGoodStack();
	}

	public void DeleteEntity()
	{
		RemoveGoodStack();
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

	private void Disable()
	{
		_cuttable.Yielder.Disable();
	}
}
