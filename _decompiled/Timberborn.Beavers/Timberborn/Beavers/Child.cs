using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.LifeSystem;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Beavers;

public class Child : TickableComponent, IAwakableComponent, IPreInitializableEntity, IPersistentEntity
{
	private static readonly string BonusId = "GrowthSpeed";

	private static readonly ComponentKey ChildKey = new ComponentKey("Child");

	private static readonly PropertyKey<float> GrowthProgressKey = new PropertyKey<float>("GrowthProgress");

	private readonly BeaverFactory _beaverFactory;

	private readonly LifeService _lifeService;

	private readonly IDayNightCycle _dayNightCycle;

	private Character _character;

	private BonusManager _bonusManager;

	private bool _grownUp;

	public float GrowthProgress { get; private set; }

	public bool IsNewborn => _character.Age == 0;

	public Child(BeaverFactory beaverFactory, LifeService lifeService, IDayNightCycle dayNightCycle)
	{
		_beaverFactory = beaverFactory;
		_lifeService = lifeService;
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_character = GetComponent<Character>();
		_bonusManager = GetComponent<BonusManager>();
	}

	public void PreInitializeEntity()
	{
		if (TryGetComponent<ChildInit>(out var component))
		{
			GrowthProgress = component.GrowthProgress;
		}
	}

	public override void Tick()
	{
		UpdateGrowthProgress();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(ChildKey).Set(GrowthProgressKey, GrowthProgress);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ChildKey);
		GrowthProgress = component.Get(GrowthProgressKey);
	}

	public bool GrowUpIfItIsTime()
	{
		if (!_grownUp && GrowthProgress >= 1f)
		{
			GrowUp();
		}
		return _grownUp;
	}

	private void UpdateGrowthProgress()
	{
		GrowthProgress = Math.Min(GrowthProgress + GrowthProgressPerTick(), 1f);
	}

	private float GrowthProgressPerTick()
	{
		return _lifeService.CalculateGrowthProgress(_dayNightCycle.FixedDeltaTimeInHours) * _bonusManager.Multiplier(BonusId);
	}

	private void GrowUp()
	{
		base.GameObject.SetActive(value: false);
		_beaverFactory.CreateAdultFromChild(this);
		_grownUp = true;
	}
}
