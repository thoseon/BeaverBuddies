using Timberborn.SkySystem;
using Timberborn.ThumbnailCapturing;
using UnityEngine;

namespace Timberborn.MapThumbnailCapturing;

internal class SunThumbnailRenderingListener : IThumbnailRenderingListener
{
	private readonly Sun _sun;

	private float _preRenderingSunYAngle;

	public SunThumbnailRenderingListener(Sun sun)
	{
		_sun = sun;
	}

	public void PreThumbnailRendering(ThumbnailCamera thumbnailCamera)
	{
		float cameraYAngle = _sun.GetCameraYAngle(thumbnailCamera.Transform);
		Transform transform = _sun.Transform;
		Vector3 eulerAngles = transform.eulerAngles;
		_preRenderingSunYAngle = eulerAngles.y;
		transform.eulerAngles = new Vector3(eulerAngles.x, cameraYAngle, eulerAngles.z);
	}

	public void PostThumbnailRendering()
	{
		Transform transform = _sun.Transform;
		Vector3 eulerAngles = transform.eulerAngles;
		transform.eulerAngles = new Vector3(eulerAngles.x, _preRenderingSunYAngle, eulerAngles.z);
	}
}
