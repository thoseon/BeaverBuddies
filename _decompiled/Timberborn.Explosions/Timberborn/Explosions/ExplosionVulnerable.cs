using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.Localization;
using Timberborn.MortalSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Explosions;

internal class ExplosionVulnerable : BaseComponent, IAwakableComponent
{
	private static readonly string BlownInExplosionLocKey = "Explosions.BlownInExplosionMessage";

	private readonly ILoc _loc;

	private readonly EventBus _eventBus;

	private Character _character;

	private Mortal _mortal;

	public ExplosionVulnerable(ILoc loc, EventBus eventBus)
	{
		_loc = loc;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_character = GetComponent<Character>();
		_mortal = GetComponent<Mortal>();
	}

	public void DieFromExplosion(BaseComponent source)
	{
		_mortal.DieInstantly(_loc.T(BlownInExplosionLocKey, _character.FirstName));
		_eventBus.Post(new MortalDiedFromExplosionEvent(source));
	}
}
