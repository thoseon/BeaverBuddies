namespace Timberborn.ThumbnailCapturing;

public interface IThumbnailRenderingListener
{
	void PreThumbnailRendering(ThumbnailCamera thumbnailCamera);

	void PostThumbnailRendering();
}
