using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WaterObjects;

public class WaterObstacle : BaseComponent, IAwakableComponent
{
	private readonly IWaterService _waterService;

	private BlockObject _blockObject;

	private WaterObstacleSpec _waterObstacleSpec;

	private bool _wasAdded;

	private float _height;

	public WaterObstacle(IWaterService waterService)
	{
		_waterService = waterService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_waterObstacleSpec = GetComponent<WaterObstacleSpec>();
	}

	public void AddToWaterService(float height)
	{
		if (_wasAdded || !_blockObject.AddedToService)
		{
			return;
		}
		_height = height;
		int z = _blockObject.CoordinatesAtBaseZ.z;
		foreach (Vector2Int coordinate in _waterObstacleSpec.Coordinates)
		{
			Vector2Int vector2Int = _blockObject.TransformTile(coordinate);
			for (int i = 0; (float)i < height; i++)
			{
				Vector3Int coordinates = new Vector3Int(vector2Int.x, vector2Int.y, z + i);
				float num = _height % 1f;
				if ((float)(i + 1) > height && num > 0f)
				{
					_waterService.SetPartialObstacle(coordinates, num);
				}
				else
				{
					_waterService.AddFullObstacle(coordinates);
				}
			}
		}
		_wasAdded = true;
	}

	public void RemoveFromWaterService()
	{
		if (!_wasAdded)
		{
			return;
		}
		int z = _blockObject.CoordinatesAtBaseZ.z;
		foreach (Vector2Int coordinate in _waterObstacleSpec.Coordinates)
		{
			Vector2Int vector2Int = _blockObject.TransformTile(coordinate);
			int num = Mathf.CeilToInt(_height - 1f);
			for (int num2 = num; num2 >= 0; num2--)
			{
				Vector3Int coordinates = new Vector3Int(vector2Int.x, vector2Int.y, z + num2);
				if (num2 == num && _height % 1f > 0f)
				{
					_waterService.RemovePartialObstacle(coordinates);
				}
				else
				{
					_waterService.RemoveFullObstacle(coordinates);
				}
			}
		}
		_wasAdded = false;
	}
}
