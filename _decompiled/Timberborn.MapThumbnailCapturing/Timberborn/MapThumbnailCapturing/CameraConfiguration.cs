using UnityEngine;

namespace Timberborn.MapThumbnailCapturing;

public readonly struct CameraConfiguration
{
	public Vector3 Position { get; }

	public Quaternion Rotation { get; }

	public float ShadowDistance { get; }

	public CameraConfiguration(Vector3 position, Quaternion rotation, float shadowDistance)
	{
		Position = position;
		Rotation = rotation;
		ShadowDistance = shadowDistance;
	}
}
