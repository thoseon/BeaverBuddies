using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Illumination;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.IlluminationUI;

public class NightTimeLightController : BaseComponent, IFinishedStateListener
{
	private readonly EventBus _eventBus;

	private readonly IDayNightCycle _dayNightCycle;

	private IlluminatorToggle _illuminatorToggle;

	public NightTimeLightController(EventBus eventBus, IDayNightCycle dayNightCycle)
	{
		_eventBus = eventBus;
		_dayNightCycle = dayNightCycle;
	}

	public void OnEnterFinishedState()
	{
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
		_eventBus.Register(this);
		if (_dayNightCycle.IsNighttime)
		{
			_illuminatorToggle.TurnOn();
		}
	}

	public void OnExitFinishedState()
	{
		_eventBus.Unregister((object)this);
	}

	[OnEvent]
	public void OnNighttimeStartEvent(NighttimeStartEvent nighttimeStartEvent)
	{
		_illuminatorToggle.TurnOn();
	}

	[OnEvent]
	public void OnDayTimeStartEvent(DaytimeStartEvent daytimeStartEvent)
	{
		_illuminatorToggle.TurnOff();
	}
}
