using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.HazardousWeatherSystem;
using Timberborn.MechanicalSystem;
using Timberborn.SingletonSystem;
using Timberborn.WaterSourceSystem;

namespace Timberborn.GameWaterSourceSystem;

public class UndergroundWaterSourceDrill : BaseComponent, IAwakableComponent, IFinishedStateListener, IWaterStrengthModifier
{
	private readonly EventBus _eventBus;

	private UnderlyingWaterSource _underlyingWaterSource;

	private HazardousWeatherObserver _hazardousWeatherObserver;

	private MechanicalNode _mechanicalNode;

	private BlockableObject _blockableObject;

	private bool _isBlocking;

	public UndergroundWaterSourceDrill(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_underlyingWaterSource = GetComponent<UnderlyingWaterSource>();
		_hazardousWeatherObserver = GetComponent<HazardousWeatherObserver>();
		_mechanicalNode = GetComponent<MechanicalNode>();
		_blockableObject = GetComponent<BlockableObject>();
	}

	public void OnEnterFinishedState()
	{
		_underlyingWaterSource.AddWaterStrengthModifier(this);
		_underlyingWaterSource.WaterSource?.GetComponent<UndergroundWaterSource>().SetOccupied();
		if (_hazardousWeatherObserver.IsBadtideWeather || _hazardousWeatherObserver.IsDroughtWeather)
		{
			Block();
		}
		_eventBus.Register(this);
	}

	public void OnExitFinishedState()
	{
		_underlyingWaterSource.RemoveWaterStrengthModifier(this);
		_underlyingWaterSource.WaterSource?.GetComponent<UndergroundWaterSource>().SetUnoccupied();
		_eventBus.Unregister((object)this);
	}

	public float GetStrengthModifier()
	{
		if (_hazardousWeatherObserver.IsBadtideWeather)
		{
			return 0f;
		}
		if (!_mechanicalNode.ActiveAndPowered)
		{
			return 0f;
		}
		return _mechanicalNode.PowerEfficiency;
	}

	[OnEvent]
	public void OnHazardousWeatherStarted(HazardousWeatherStartedEvent hazardousWeatherStartedEvent)
	{
		if (!_isBlocking)
		{
			Block();
		}
	}

	[OnEvent]
	public void OnHazardousWeatherEnded(HazardousWeatherEndedEvent hazardousWeatherEndedEvent)
	{
		if (_isBlocking)
		{
			_blockableObject.Unblock(this);
			_isBlocking = false;
		}
	}

	private void Block()
	{
		_isBlocking = true;
		_blockableObject.Block(this);
	}
}
