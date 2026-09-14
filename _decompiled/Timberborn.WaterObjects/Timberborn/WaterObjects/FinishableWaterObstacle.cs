using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.WaterObjects;

internal class FinishableWaterObstacle : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private WaterObstacle _waterObstacle;

	private FinishableWaterObstacleSpec _finishableWaterObstacleSpec;

	public void Awake()
	{
		_waterObstacle = GetComponent<WaterObstacle>();
		_finishableWaterObstacleSpec = GetComponent<FinishableWaterObstacleSpec>();
	}

	public void OnEnterFinishedState()
	{
		_waterObstacle.AddToWaterService(_finishableWaterObstacleSpec.Height);
	}

	public void OnExitFinishedState()
	{
		_waterObstacle.RemoveFromWaterService();
	}
}
