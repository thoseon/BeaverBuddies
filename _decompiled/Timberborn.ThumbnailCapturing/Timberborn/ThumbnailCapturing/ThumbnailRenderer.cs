using System.Collections.Generic;
using System.Collections.Immutable;

namespace Timberborn.ThumbnailCapturing;

public class ThumbnailRenderer
{
	private readonly ThumbnailCamera _thumbnailCamera;

	private readonly ImmutableArray<IThumbnailRenderingListener> _thumbnailRenderingListeners;

	public ThumbnailRenderer(ThumbnailCamera thumbnailCamera, IEnumerable<IThumbnailRenderingListener> thumbnailRenderingListeners)
	{
		_thumbnailCamera = thumbnailCamera;
		_thumbnailRenderingListeners = thumbnailRenderingListeners.ToImmutableArray();
	}

	public void Render()
	{
		PreCameraRendering();
		_thumbnailCamera.Render();
		PostCameraRendering();
	}

	private void PreCameraRendering()
	{
		foreach (IThumbnailRenderingListener thumbnailRenderingListener in _thumbnailRenderingListeners)
		{
			thumbnailRenderingListener.PreThumbnailRendering(_thumbnailCamera);
		}
	}

	private void PostCameraRendering()
	{
		foreach (IThumbnailRenderingListener thumbnailRenderingListener in _thumbnailRenderingListeners)
		{
			thumbnailRenderingListener.PostThumbnailRendering();
		}
	}
}
