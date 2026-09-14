using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.WaterSystem;

namespace Timberborn.WaterBuildings;

internal class WaterObstacleController : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly IWaterService _waterService;

	private BlockObject _blockObject;

	private bool _wasAdded;

	public WaterObstacleController(IWaterService waterService)
	{
		_waterService = waterService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void OnEnterFinishedState()
	{
	}

	public void OnExitFinishedState()
	{
		if (_wasAdded)
		{
			_waterService.RemoveFullObstacle(_blockObject.Coordinates);
		}
	}

	public void UpdateState(bool add)
	{
		if (add && !_wasAdded)
		{
			_waterService.AddFullObstacle(_blockObject.Coordinates);
			_wasAdded = true;
		}
		else if (!add && _wasAdded)
		{
			_waterService.RemoveFullObstacle(_blockObject.Coordinates);
			_wasAdded = false;
		}
	}
}
