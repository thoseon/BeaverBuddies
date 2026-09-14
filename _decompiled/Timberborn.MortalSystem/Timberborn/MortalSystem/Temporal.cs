using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.LifeSystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.MortalSystem;

public class Temporal : BaseComponent, IAwakableComponent, IDeletableEntity
{
	private static readonly string DiedOldAgeLocKey = "Beaver.DiedOldAge";

	private readonly EventBus _eventBus;

	private readonly ILoc _loc;

	private Mortal _mortal;

	private Character _character;

	private LifeProgressor _lifeProgressor;

	public Temporal(EventBus eventBus, ILoc loc)
	{
		_eventBus = eventBus;
		_loc = loc;
	}

	public void Awake()
	{
		_mortal = GetComponent<Mortal>();
		_character = GetComponent<Character>();
		_lifeProgressor = GetComponent<LifeProgressor>();
		_eventBus.Register(this);
	}

	public void DeleteEntity()
	{
		_eventBus.Unregister(this);
	}

	[OnEvent]
	public void OnDaytimeStart(DaytimeStartEvent daytimeStartEvent)
	{
		DieIfTooOld();
	}

	private void DieIfTooOld()
	{
		if (_lifeProgressor.ShouldDie)
		{
			_mortal.DieSilentlyAsSoonAsPossible(_loc.T(DiedOldAgeLocKey, _character.FirstName));
		}
	}
}
