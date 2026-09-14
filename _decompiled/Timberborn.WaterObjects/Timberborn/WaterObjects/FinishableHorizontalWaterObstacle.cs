using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.WaterObjects;

internal class FinishableHorizontalWaterObstacle : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private HorizontalWaterObstacle _horizontalWaterObstacle;

	private FinishableHorizontalWaterObstacleSpec _finishableHorizontalWaterObstacleSpec;

	public void Awake()
	{
		_horizontalWaterObstacle = GetComponent<HorizontalWaterObstacle>();
		_finishableHorizontalWaterObstacleSpec = GetComponent<FinishableHorizontalWaterObstacleSpec>();
	}

	public void OnEnterFinishedState()
	{
		_horizontalWaterObstacle.AddToWaterService(_finishableHorizontalWaterObstacleSpec.Obstacles);
	}

	public void OnExitFinishedState()
	{
		_horizontalWaterObstacle.RemoveFromWaterService();
	}
}
