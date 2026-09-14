using Timberborn.SelectionSystem;
using Timberborn.ThumbnailCapturing;

namespace Timberborn.MapThumbnailCapturing;

internal class SelectionThumbnailRenderingListener : IThumbnailRenderingListener
{
	private readonly EntitySelectionService _entitySelectionService;

	public SelectionThumbnailRenderingListener(EntitySelectionService entitySelectionService)
	{
		_entitySelectionService = entitySelectionService;
	}

	public void PreThumbnailRendering(ThumbnailCamera thumbnailCamera)
	{
		_entitySelectionService.UnhighlightUntilNextUpdate();
	}

	public void PostThumbnailRendering()
	{
	}
}
