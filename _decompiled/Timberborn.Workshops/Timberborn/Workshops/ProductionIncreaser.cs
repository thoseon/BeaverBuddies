using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;

namespace Timberborn.Workshops;

public class ProductionIncreaser : TickableComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly IDayNightCycle _dayNightCycle;

	private Manufactory _manufactory;

	public ProductionIncreaser(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_manufactory = GetComponent<Manufactory>();
		DisableComponent();
	}

	public override void Tick()
	{
		_manufactory.IncreaseProductionProgress(_dayNightCycle.FixedDeltaTimeInHours);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}
}
