using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DwellingSystem;
using Timberborn.EnterableSystem;
using Timberborn.NeedSystem;

namespace Timberborn.Reproduction;

internal class ProcreationHouse : BaseComponent, IAwakableComponent
{
	private static readonly float DailySpawningChance = 0.1875f;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly NewbornSpawner _newbornSpawner;

	private Enterable _enterable;

	private Dwelling _dwelling;

	public ProcreationHouse(IRandomNumberGenerator randomNumberGenerator, NewbornSpawner newbornSpawner)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_newbornSpawner = newbornSpawner;
	}

	public void Awake()
	{
		_enterable = GetComponent<Enterable>();
		_dwelling = GetComponent<Dwelling>();
		_enterable.EntererAdded += delegate(object _, EntererAddedEventArgs e)
		{
			ProcreateAmongDwellers(e.Enterer);
		};
	}

	private void ProcreateAmongDwellers(Enterer enterer)
	{
		if (HasFreeChildSlot() && CanProcreate(enterer) && HasDwellerToProcreateWith(enterer) && CheckProcreationChance())
		{
			_newbornSpawner.SpawnChild(_dwelling);
		}
	}

	private bool HasFreeChildSlot()
	{
		if (_dwelling.HasFreeChildSlots)
		{
			return _dwelling.UnderpopulatedByChildren;
		}
		return false;
	}

	private static bool CanProcreate(BaseComponent entity)
	{
		Procreator component = entity.GetComponent<Procreator>();
		if (component != null && component.Enabled)
		{
			return !entity.GetComponent<NeedManager>().AnyNeedIsInCriticalState();
		}
		return false;
	}

	private bool HasDwellerToProcreateWith(Enterer enterer)
	{
		foreach (Dweller adultDweller in _dwelling.AdultDwellers)
		{
			if (adultDweller.GameObject != enterer.GameObject && CanProcreate(adultDweller))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckProcreationChance()
	{
		return _randomNumberGenerator.CheckProbability(DailySpawningChance);
	}
}
