using Timberborn.CameraSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.MapStateSystem;
using UnityEngine;

namespace Timberborn.MapThumbnailCapturing;

public class ThumbnailCameraDefaultPositionProvider
{
	private static readonly Quaternion DefaultRotation = Quaternion.Euler(40f, 0f, 0f);

	private readonly MapSize _mapSize;

	public ThumbnailCameraDefaultPositionProvider(MapSize mapSize)
	{
		_mapSize = mapSize;
	}

	public CameraConfiguration GetDefaultPosition()
	{
		Vector3 vector = CoordinateSystem.GridToWorld((_mapSize.TerrainSize.XY() / 2).XYZ());
		int x = _mapSize.TerrainSize.x;
		return new CameraConfiguration(vector + DefaultRotation * Vector3.back * x, DefaultRotation, ShadowDistanceUpdater.MaxDistance);
	}
}
