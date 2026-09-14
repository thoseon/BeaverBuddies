using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Emptying;

namespace Timberborn.StockpilePrioritySystem;

public class StockpilePriorityChangeListener : BaseComponent, IAwakableComponent
{
	public event EventHandler PriorityChanged;

	public void Awake()
	{
		GoodObtainer component = GetComponent<GoodObtainer>();
		if ((bool)component)
		{
			component.GoodObtainingChanged += OnPriorityChanged;
		}
		GoodSupplier component2 = GetComponent<GoodSupplier>();
		if ((bool)component2)
		{
			component2.GoodSupplyingChanged += OnPriorityChanged;
		}
		Emptiable component3 = GetComponent<Emptiable>();
		component3.MarkedForEmptying += OnPriorityChanged;
		component3.UnmarkedForEmptying += OnPriorityChanged;
	}

	private void OnPriorityChanged(object sender, EventArgs e)
	{
		PriorityChanged?.Invoke(this, EventArgs.Empty);
	}
}
