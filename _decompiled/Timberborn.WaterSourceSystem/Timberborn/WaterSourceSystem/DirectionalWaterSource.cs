using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WaterSourceSystem;

internal class DirectionalWaterSource : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly IWaterService _waterService;

	private WaterSource _waterSource;

	private BlockObject _blockObject;

	public DirectionalWaterSource(IWaterService waterService)
	{
		_waterService = waterService;
	}

	public void Awake()
	{
		_waterSource = GetComponent<WaterSource>();
		_blockObject = GetComponent<BlockObject>();
	}

	public void InitializeEntity()
	{
		FlowDirection flowDirection = OrientationToFlowDirection(_blockObject.Orientation);
		foreach (Vector3Int coordinate in _waterSource.Coordinates)
		{
			_waterService.AddDirectionLimiter(coordinate, flowDirection);
		}
	}

	private static FlowDirection OrientationToFlowDirection(Orientation orientation)
	{
		return orientation switch
		{
			Orientation.Cw0 => FlowDirection.Top, 
			Orientation.Cw90 => FlowDirection.Right, 
			Orientation.Cw180 => FlowDirection.Bottom, 
			Orientation.Cw270 => FlowDirection.Left, 
			_ => throw new ArgumentOutOfRangeException("orientation", orientation, null), 
		};
	}
}
