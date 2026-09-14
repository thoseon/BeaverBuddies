using Timberborn.CameraSystem;
using Timberborn.TerrainQueryingSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.CameraWorldState;

internal class CameraAnchorPicker : ICameraAnchorPicker
{
	private readonly TerrainPicker _terrainPicker;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	public CameraAnchorPicker(TerrainPicker terrainPicker, IThreadSafeWaterMap threadSafeWaterMap)
	{
		_terrainPicker = terrainPicker;
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public Vector3? PickAnchorPoint(Ray ray)
	{
		return _terrainPicker.PickTerrainCoordinates(ray, IsWaterVoxel)?.Intersection;
	}

	private bool IsWaterVoxel(Vector3Int coordinates)
	{
		return _threadSafeWaterMap.CellIsUnderwater(coordinates);
	}
}
