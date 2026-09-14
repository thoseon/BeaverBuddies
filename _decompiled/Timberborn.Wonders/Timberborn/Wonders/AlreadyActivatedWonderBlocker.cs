using Timberborn.BaseComponentSystem;

namespace Timberborn.Wonders;

internal class AlreadyActivatedWonderBlocker : BaseComponent, IAwakableComponent, IWonderBlocker
{
	private Wonder _wonder;

	private WonderAnimationController _wonderAnimationController;

	public void Awake()
	{
		_wonder = GetComponent<Wonder>();
		_wonderAnimationController = GetComponent<WonderAnimationController>();
	}

	public bool IsWonderBlocked()
	{
		if (!_wonder.IsActive)
		{
			return _wonderAnimationController.IsAnimating;
		}
		return true;
	}
}
