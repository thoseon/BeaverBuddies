using Timberborn.ThumbnailCapturing;

namespace Timberborn.SaveThumbnailCapturing;

internal class SaveThumbnailRenderingListener : IThumbnailRenderingListener
{
	public void PreThumbnailRendering(ThumbnailCamera thumbnailCamera)
	{
		thumbnailCamera.MoveToMainCameraPosition();
	}

	public void PostThumbnailRendering()
	{
	}
}
