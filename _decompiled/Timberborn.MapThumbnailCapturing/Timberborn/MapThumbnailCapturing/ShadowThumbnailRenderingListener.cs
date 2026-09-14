using Timberborn.CameraSystem;
using Timberborn.ThumbnailCapturing;

namespace Timberborn.MapThumbnailCapturing;

internal class ShadowThumbnailRenderingListener : IThumbnailRenderingListener
{
	private readonly MapThumbnailCameraMover _mapThumbnailCameraMover;

	private readonly ShadowDistanceUpdater _shadowDistanceUpdater;

	private float _preRenderingShadowDistance;

	public ShadowThumbnailRenderingListener(MapThumbnailCameraMover mapThumbnailCameraMover, ShadowDistanceUpdater shadowDistanceUpdater)
	{
		_mapThumbnailCameraMover = mapThumbnailCameraMover;
		_shadowDistanceUpdater = shadowDistanceUpdater;
	}

	public void PreThumbnailRendering(ThumbnailCamera thumbnailCamera)
	{
		_preRenderingShadowDistance = _shadowDistanceUpdater.GetShadowDistance();
		_shadowDistanceUpdater.SetShadowDistance(_mapThumbnailCameraMover.CurrentConfiguration.ShadowDistance);
	}

	public void PostThumbnailRendering()
	{
		_shadowDistanceUpdater.SetShadowDistance(_preRenderingShadowDistance);
	}
}
