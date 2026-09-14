using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WaterObjects;

internal class HorizontalWaterObstacle : BaseComponent, IAwakableComponent
{
	private readonly IWaterService _waterService;

	private BlockObject _blockObject;

	private readonly List<Vector3Int> _addedObstacles = new List<Vector3Int>();

	public HorizontalWaterObstacle(IWaterService waterService)
	{
		_waterService = waterService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void AddToWaterService(IEnumerable<Vector3Int> obstacles)
	{
		if (_addedObstacles.Count != 0 || !_blockObject.AddedToService)
		{
			return;
		}
		_addedObstacles.AddRange(obstacles);
		foreach (Vector3Int addedObstacle in _addedObstacles)
		{
			Vector3Int vector3Int = _blockObject.TransformCoordinates(addedObstacle);
			Vector3Int coordinatesToAdd = new Vector3Int(vector3Int.x, vector3Int.y, _blockObject.Coordinates.z + addedObstacle.z);
			_waterService.AddHorizontalObstacle(coordinatesToAdd);
		}
	}

	public void RemoveFromWaterService()
	{
		if (_addedObstacles.Count <= 0)
		{
			return;
		}
		foreach (Vector3Int addedObstacle in _addedObstacles)
		{
			Vector3Int vector3Int = _blockObject.TransformCoordinates(addedObstacle);
			Vector3Int coordinatesToRemove = new Vector3Int(vector3Int.x, vector3Int.y, _blockObject.Coordinates.z + addedObstacle.z);
			_waterService.RemoveHorizontalObstacle(coordinatesToRemove);
		}
		_addedObstacles.Clear();
	}
}
