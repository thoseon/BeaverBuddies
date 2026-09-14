using Timberborn.BaseComponentSystem;
using Timberborn.Growing;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Pollination;

internal class Pollinatee : BaseComponent, IAwakableComponent, IPersistentEntity
{
	private static readonly float DefaultLastPollinationTimestamp = -1f;

	private static readonly ComponentKey PollinateeKey = new ComponentKey("Pollinatee");

	private static readonly PropertyKey<float> LastPollinationTimestampKey = new PropertyKey<float>("LastPollinationTimestamp");

	private readonly IDayNightCycle _dayNightCycle;

	private Growable _growable;

	private float _lastPollinationTimestamp = DefaultLastPollinationTimestamp;

	public bool CanPollinate
	{
		get
		{
			if (_dayNightCycle.PartialDayNumber > _lastPollinationTimestamp + 1f)
			{
				return _growable.GrowthInProgress;
			}
			return false;
		}
	}

	public Pollinatee(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_growable = GetComponent<Growable>();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_lastPollinationTimestamp != DefaultLastPollinationTimestamp)
		{
			entitySaver.GetComponent(PollinateeKey).Set(LastPollinationTimestampKey, _lastPollinationTimestamp);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(PollinateeKey, out var objectLoader))
		{
			_lastPollinationTimestamp = objectLoader.Get(LastPollinationTimestampKey);
		}
	}

	public void Pollinate(float growthTimeReduction)
	{
		if (CanPollinate)
		{
			float num = 1f / (1f - growthTimeReduction) - 1f;
			_growable.IncreaseGrowthProgress(num / _growable.GrowthTimeInDays);
			_lastPollinationTimestamp = _dayNightCycle.PartialDayNumber;
		}
	}
}
