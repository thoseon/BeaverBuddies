using Timberborn.CameraSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.MapEditorScene;

public class MapEditorCameraCenterer : ILoadableSingleton
{
	private readonly MapSize _mapSize;

	private readonly CameraService _cameraService;

	public MapEditorCameraCenterer(MapSize mapSize, CameraService cameraService)
	{
		_mapSize = mapSize;
		_cameraService = cameraService;
	}

	public void Load()
	{
		Vector3Int coordinates = (_mapSize.TerrainSize.XY() / 2).XYZ();
		_cameraService.MoveTargetTo(CoordinateSystem.GridToWorld(coordinates));
	}
}
