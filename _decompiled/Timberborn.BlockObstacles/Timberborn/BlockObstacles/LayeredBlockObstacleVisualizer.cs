using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.BlockObstacles;

internal class LayeredBlockObstacleVisualizer : TickableComponent, IAwakableComponent, IPostLoadableEntity, IUpdatableComponent
{
	private static readonly float MinimumChangeRate = 0.001f;

	private static readonly float MaxOccupancyRangeDifference = 0.25f;

	private readonly ITickService _tickService;

	private LayeredBlockObstacle _obstacle;

	private Transform _positionTransform;

	private Transform _scaleTransform;

	private Vector3 _originalPosition;

	private Vector3 _originalScale;

	private float _previousOccupancyRange;

	private float _occupancyChangeRate;

	public LayeredBlockObstacleVisualizer(ITickService tickService)
	{
		_tickService = tickService;
	}

	public void Awake()
	{
		_obstacle = GetComponent<LayeredBlockObstacle>();
		LayeredBlockObstacleVisualizerSpec component = GetComponent<LayeredBlockObstacleVisualizerSpec>();
		_positionTransform = base.GameObject.FindChildTransform(component.PositionTransformName);
		_scaleTransform = base.GameObject.FindChildTransform(component.ScaleTransformName);
		_originalPosition = _positionTransform.localPosition;
		_originalScale = _scaleTransform.localScale;
	}

	public void PostLoadEntity()
	{
		UpdatePositionAndScale(_obstacle.OccupancyRange);
	}

	public void Update()
	{
		UpdateTransforms();
	}

	public override void Tick()
	{
		UpdateOccupancyChangeRate();
	}

	private void UpdateTransforms()
	{
		float num = _originalPosition.y - _positionTransform.localPosition.y;
		float occupancyRange = _obstacle.OccupancyRange;
		if (Math.Abs(num - occupancyRange) > MaxOccupancyRangeDifference)
		{
			UpdatePositionAndScale(occupancyRange);
		}
		else
		{
			Interpolate(num, occupancyRange);
		}
	}

	private void UpdateOccupancyChangeRate()
	{
		float val = Math.Abs((_obstacle.OccupancyRange - _previousOccupancyRange) / _tickService.TickIntervalInSeconds);
		_occupancyChangeRate = Math.Max(MinimumChangeRate, val);
		_previousOccupancyRange = _obstacle.OccupancyRange;
	}

	private void Interpolate(float currentOccupancyRange, float targetOccupancyRange)
	{
		float newOccupancyRange = Mathf.MoveTowards(currentOccupancyRange, targetOccupancyRange, Time.deltaTime * _occupancyChangeRate);
		UpdatePositionAndScale(newOccupancyRange);
	}

	private void UpdatePositionAndScale(float newOccupancyRange)
	{
		_positionTransform.localPosition = _originalPosition - new Vector3(0f, newOccupancyRange, 0f);
		_scaleTransform.localScale = _originalScale + new Vector3(0f, newOccupancyRange, 0f);
	}
}
