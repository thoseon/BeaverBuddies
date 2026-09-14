using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.LifeSystem;

public class LifeProgressor : TickableComponent, IAwakableComponent, IPreInitializableEntity, IPersistentEntity
{
	private static readonly string LifeExpectancyBonusId = "LifeExpectancy";

	private static readonly ComponentKey LifeProgressorKey = new ComponentKey("LifeProgressor");

	private static readonly PropertyKey<float> LifeProgressKey = new PropertyKey<float>("LifeProgress");

	private readonly LifeService _lifeService;

	private BonusManager _bonusManager;

	private ILongevity _longevity;

	public float LifeProgress { get; private set; }

	public bool ShouldDie => LifeProgress > _longevity.ExpectedLongevity;

	public LifeProgressor(LifeService lifeService)
	{
		_lifeService = lifeService;
	}

	public void Awake()
	{
		_bonusManager = GetComponent<BonusManager>();
		_longevity = GetComponent<ILongevity>();
	}

	public void PreInitializeEntity()
	{
		if (TryGetComponent<CharacterInit>(out var component))
		{
			LifeProgress = component.LifeProgress;
		}
	}

	public override void Tick()
	{
		IncreaseLifeProgress();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(LifeProgressorKey).Set(LifeProgressKey, LifeProgress);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(LifeProgressorKey);
		LifeProgress = component.Get(LifeProgressKey);
	}

	private void IncreaseLifeProgress()
	{
		LifeProgress += _lifeService.LifeProgressIncreasePerTick / _bonusManager.Multiplier(LifeExpectancyBonusId);
	}
}
