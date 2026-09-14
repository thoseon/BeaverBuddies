using Timberborn.BaseComponentSystem;
using Timberborn.Cutting;
using Timberborn.EntitySystem;
using Timberborn.GoodStackSystem;

namespace Timberborn.Forestry;

public class LumberjackGoodStackAdder : BaseComponent, IAwakableComponent, IPostInitializableEntity, IDeletableEntity
{
	private readonly GoodStackService<LumberjackFlagSpec> _goodStackService;

	private Cuttable _cuttable;

	private GoodStack _goodStack;

	public LumberjackGoodStackAdder(GoodStackService<LumberjackFlagSpec> goodStackService)
	{
		_goodStackService = goodStackService;
	}

	public void Awake()
	{
		Cuttable component = GetComponent<Cuttable>();
		if (component != null)
		{
			_cuttable = component;
			_goodStack = GetComponent<GoodStack>();
		}
	}

	public void PostInitializeEntity()
	{
		if ((bool)_cuttable)
		{
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
}
