using Timberborn.BaseComponentSystem;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.ReservableSystem;

namespace Timberborn.Forestry;

public class TreeComponent : BaseComponent, IAwakableComponent
{
	private LivingNaturalResource _livingNaturalResource;

	private Reservable _reservable;

	public bool CanBeReplaced
	{
		get
		{
			if (_livingNaturalResource.IsDead)
			{
				return !_reservable.Reserved;
			}
			return false;
		}
	}

	public void Awake()
	{
		_livingNaturalResource = GetComponent<LivingNaturalResource>();
		_reservable = GetComponent<Reservable>();
	}
}
