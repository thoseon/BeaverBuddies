using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.Localization;
using Timberborn.MortalSystem;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.DeteriorationSystem;

public class Deteriorable : TickableComponent, IAwakableComponent, IPersistentEntity
{
	private static readonly string BotDeathLocKey = "Bot.DeathMessage";

	private static readonly ComponentKey DeteriorableKey = new ComponentKey("Deteriorable");

	private static readonly PropertyKey<float> CurrentDeteriorationKey = new PropertyKey<float>("CurrentDeterioration");

	private readonly IDayNightCycle _dayNightCycle;

	private readonly ILoc _loc;

	private Mortal _mortal;

	private DeteriorableSpec _deteriorableSpec;

	private float _currentDeterioration;

	private float _fixedDeltaTimeInDays;

	public float DeteriorationProgress => _currentDeterioration / (float)_deteriorableSpec.DeteriorationInDays;

	public Deteriorable(IDayNightCycle dayNightCycle, ILoc loc)
	{
		_dayNightCycle = dayNightCycle;
		_loc = loc;
	}

	public void Awake()
	{
		_mortal = GetComponent<Mortal>();
		_deteriorableSpec = GetComponent<DeteriorableSpec>();
		SetDeteriorationToMaximum();
		_fixedDeltaTimeInDays = _dayNightCycle.FixedDeltaTimeInHours / 24f;
	}

	public override void Tick()
	{
		if (_currentDeterioration > 0f)
		{
			_currentDeterioration -= _fixedDeltaTimeInDays;
			return;
		}
		string deathMessage = _loc.T(BotDeathLocKey, _mortal.GetComponent<Character>().FirstName);
		_mortal.DiePubliclyAsSoonAsPossible(deathMessage);
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(DeteriorableKey).Set(CurrentDeteriorationKey, _currentDeterioration);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(DeteriorableKey);
		_currentDeterioration = component.Get(CurrentDeteriorationKey);
	}

	public void SetDeteriorationToZero()
	{
		_currentDeterioration = 0f;
	}

	private void SetDeteriorationToMaximum()
	{
		_currentDeterioration = _deteriorableSpec.DeteriorationInDays;
	}
}
