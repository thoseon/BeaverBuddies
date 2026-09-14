using Timberborn.BaseComponentSystem;
using Timberborn.ReservableSystem;
using Timberborn.Yielding;

namespace Timberborn.UncuttableYielding;

internal class UncuttableRemoveYieldStrategy : BaseComponent, IAwakableComponent, IRemoveYieldStrategy
{
	public ReservableReacher Reacher { get; private set; }

	public string Id => "Uncuttable";

	public bool IsStillRemovable => true;

	public void Awake()
	{
		Reacher = GetComponent<UncuttableReacher>();
	}
}
