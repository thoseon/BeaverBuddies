using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.WaterSystem;

internal class WaterMapBoundary : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity
{
	private readonly WaterMapBoundaryService _waterMapBoundaryService;

	private IWaterSource _waterSource;

	public WaterMapBoundary(WaterMapBoundaryService waterMapBoundaryService)
	{
		_waterMapBoundaryService = waterMapBoundaryService;
	}

	public void Awake()
	{
		_waterSource = GetComponent<IWaterSource>();
	}

	public void InitializeEntity()
	{
		BlockNearbyBoundaries();
	}

	public void DeleteEntity()
	{
		UnblockNearbyBoundaries();
	}

	private void BlockNearbyBoundaries()
	{
		SetCellBlockAtNearbyBoundaries(block: true);
	}

	private void UnblockNearbyBoundaries()
	{
		SetCellBlockAtNearbyBoundaries(block: false);
	}

	private void SetCellBlockAtNearbyBoundaries(bool block)
	{
		Vector2Int[] neighbors4Vector2Int = Deltas.Neighbors4Vector2Int;
		foreach (Vector2Int vector2Int in neighbors4Vector2Int)
		{
			for (int j = 0; j < _waterSource.Coordinates.Length; j++)
			{
				Vector3Int value = _waterSource.Coordinates[j];
				if (block)
				{
					_waterMapBoundaryService.FullyBlockCell(value.XY() + vector2Int);
				}
				else
				{
					_waterMapBoundaryService.FullyUnblockCell(value.XY() + vector2Int);
				}
			}
		}
	}
}
