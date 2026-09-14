using Timberborn.StartingLocationSystem;
using Timberborn.ThumbnailCapturing;

namespace Timberborn.MapThumbnailCapturing;

internal class StartingLocationThumbnailRenderingListener : IThumbnailRenderingListener
{
	private readonly StartingLocationService _startingLocationService;

	private StartingLocationRenderer _startingLocationRenderer;

	public StartingLocationThumbnailRenderingListener(StartingLocationService startingLocationService)
	{
		_startingLocationService = startingLocationService;
	}

	public void PreThumbnailRendering(ThumbnailCamera thumbnailCamera)
	{
		if (_startingLocationService.HasStartingLocation())
		{
			StartingLocation startingLocation = _startingLocationService.GetStartingLocation();
			_startingLocationRenderer = startingLocation.GetComponent<StartingLocationRenderer>();
			_startingLocationRenderer.Hide();
		}
	}

	public void PostThumbnailRendering()
	{
		if ((bool)_startingLocationRenderer)
		{
			_startingLocationRenderer.Show();
			_startingLocationRenderer = null;
		}
	}
}
