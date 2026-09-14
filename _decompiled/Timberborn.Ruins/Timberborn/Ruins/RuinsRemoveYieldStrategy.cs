using Timberborn.BaseComponentSystem;
using Timberborn.Demolishing;
using Timberborn.ReservableSystem;
using Timberborn.Yielding;

namespace Timberborn.Ruins;

internal class RuinsRemoveYieldStrategy : BaseComponent, IAwakableComponent, IRemoveYieldStrategy
{
	public ReservableReacher Reacher { get; private set; }

	public string Id => "Ruins";

	public bool IsStillRemovable => true;

	public void Awake()
	{
		Reacher = GetComponent<AccessibleDemolishableReacher>();
	}
}
