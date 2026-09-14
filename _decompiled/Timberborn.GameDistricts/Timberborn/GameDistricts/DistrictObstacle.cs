using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.GameDistricts;

public class DistrictObstacle : BaseComponent, IAwakableComponent, IUnfinishedStateListener, IFinishedStateListener
{
	private readonly IDistrictService _districtService;

	private BlockObject _blockObject;

	private DistrictObstacleSpec _districtObstacleSpec;

	private Vector3Int ObstacleCoordinates => _blockObject.TransformCoordinates(_districtObstacleSpec.CoordinateOffset);

	public DistrictObstacle(IDistrictService districtService)
	{
		_districtService = districtService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_districtObstacleSpec = GetComponent<DistrictObstacleSpec>();
	}

	public void OnEnterUnfinishedState()
	{
		AddToPreviewDistricts();
	}

	public void OnExitUnfinishedState()
	{
		RemoveFromPreviewDistricts();
	}

	public void OnEnterFinishedState()
	{
		_districtService.SetObstacle(ObstacleCoordinates);
	}

	public void OnExitFinishedState()
	{
		_districtService.UnsetObstacle(ObstacleCoordinates);
	}

	public void AddToPreviewDistricts()
	{
		_districtService.SetPreviewObstacle(ObstacleCoordinates);
	}

	public void RemoveFromPreviewDistricts()
	{
		_districtService.UnsetPreviewObstacle(ObstacleCoordinates);
	}
}
