using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.WorkSystem;

namespace Timberborn.Wonders;

public class WaitForInactiveWonderWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private Wonder _wonder;

	public void Awake()
	{
		_wonder = GetComponent<Wonder>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (!_wonder.IsActive)
		{
			return Decision.ReleaseNow();
		}
		return Decision.ReturnNextTick();
	}
}
