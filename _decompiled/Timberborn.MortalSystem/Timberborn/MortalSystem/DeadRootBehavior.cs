using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;

namespace Timberborn.MortalSystem;

public class DeadRootBehavior : RootBehavior, IAwakableComponent
{
	private Mortal _mortal;

	public void Awake()
	{
		_mortal = GetComponent<Mortal>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (!_mortal.Dead)
		{
			return Decision.ReleaseNow();
		}
		return Decision.ReturnNextTick();
	}
}
