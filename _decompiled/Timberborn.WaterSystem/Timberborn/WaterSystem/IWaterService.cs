using UnityEngine;

namespace Timberborn.WaterSystem;

public interface IWaterService
{
	void AddFullObstacle(Vector3Int coordinates);

	void RemoveFullObstacle(Vector3Int coordinates);

	void AddHorizontalObstacle(Vector3Int coordinatesToAdd);

	void RemoveHorizontalObstacle(Vector3Int coordinatesToRemove);

	void RegisterWaterSource(IWaterSource waterSource);

	void UnregisterWaterSource(IWaterSource waterSource);

	void AddCleanWater(Vector3Int coordinates, float depth);

	void RemoveCleanWater(Vector3Int coordinates, float depth);

	void AddContaminatedWater(Vector3Int coordinates, float depth);

	void RemoveContaminatedWater(Vector3Int coordinates, float depth);

	void SetPartialObstacle(Vector3Int coordinates, float height);

	void RemovePartialObstacle(Vector3Int coordinates);

	void SetInflowLimit(Vector3Int coordinates, float inflowLimit);

	void RemoveInflowLimit(Vector3Int coordinates);

	void AddDirectionLimiter(Vector3Int coordinates, FlowDirection flowDirection);

	void RemoveDirectionLimiter(Vector3Int coordinates);
}
