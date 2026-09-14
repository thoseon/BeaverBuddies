using UnityEngine;

namespace Timberborn.WaterSystem;

internal class WaterService : IWaterService
{
	private readonly WaterSourceRegistry _waterSourceRegistry;

	private readonly WaterChangeService _waterChangeService;

	private readonly WaterSimulator _waterSimulator;

	private readonly FlowLimiterService _flowLimiterService;

	public WaterService(WaterSourceRegistry waterSourceRegistry, WaterChangeService waterChangeService, WaterSimulator waterSimulator, FlowLimiterService flowLimiterService)
	{
		_waterSourceRegistry = waterSourceRegistry;
		_waterChangeService = waterChangeService;
		_waterSimulator = waterSimulator;
		_flowLimiterService = flowLimiterService;
	}

	public void AddFullObstacle(Vector3Int coordinates)
	{
		_waterSimulator.AddFullObstacle(coordinates);
	}

	public void RemoveFullObstacle(Vector3Int coordinates)
	{
		_waterSimulator.RemoveFullObstacle(coordinates);
	}

	public void AddHorizontalObstacle(Vector3Int coordinates)
	{
		_waterSimulator.AddHorizontalObstacle(coordinates);
	}

	public void RemoveHorizontalObstacle(Vector3Int coordinates)
	{
		_waterSimulator.RemoveHorizontalObstacle(coordinates);
	}

	public void RegisterWaterSource(IWaterSource waterSource)
	{
		_waterSourceRegistry.RegisterWaterSource(waterSource);
	}

	public void UnregisterWaterSource(IWaterSource waterSource)
	{
		_waterSourceRegistry.UnregisterWaterSource(waterSource);
	}

	public void AddCleanWater(Vector3Int coordinates, float depth)
	{
		_waterChangeService.EnqueueWaterChange(coordinates, depth, 0f);
	}

	public void RemoveCleanWater(Vector3Int coordinates, float depth)
	{
		_waterChangeService.EnqueueWaterChange(coordinates, 0f - depth, 0f);
	}

	public void AddContaminatedWater(Vector3Int coordinates, float depth)
	{
		_waterChangeService.EnqueueWaterChange(coordinates, depth, 1f);
	}

	public void RemoveContaminatedWater(Vector3Int coordinates, float depth)
	{
		_waterChangeService.EnqueueWaterChange(coordinates, 0f - depth, 1f);
	}

	public void SetPartialObstacle(Vector3Int coordinates, float height)
	{
		_flowLimiterService.UpdateHeightLimit(coordinates, height);
	}

	public void RemovePartialObstacle(Vector3Int coordinates)
	{
		_flowLimiterService.RemoveHeightLimit(coordinates);
	}

	public void SetInflowLimit(Vector3Int coordinates, float inflowLimit)
	{
		_flowLimiterService.SetInflowLimit(coordinates, inflowLimit);
	}

	public void RemoveInflowLimit(Vector3Int coordinates)
	{
		_flowLimiterService.RemoveInflowLimit(coordinates);
	}

	public void AddDirectionLimiter(Vector3Int coordinates, FlowDirection flowDirection)
	{
		_flowLimiterService.AddDirectionLimiter(coordinates, flowDirection);
	}

	public void RemoveDirectionLimiter(Vector3Int coordinates)
	{
		_flowLimiterService.RemoveDirectionLimiter(coordinates);
	}
}
