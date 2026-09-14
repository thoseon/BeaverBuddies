using Timberborn.ThumbnailCapturing;
using Timberborn.WaterSystemRendering;

namespace Timberborn.MapThumbnailCapturing;

internal class WaterThumbnailRenderingListener : IThumbnailRenderingListener
{
	private readonly WaterOpacityService _waterOpacityService;

	private bool _preRenderingWaterHidden;

	public WaterThumbnailRenderingListener(WaterOpacityService waterOpacityService)
	{
		_waterOpacityService = waterOpacityService;
	}

	public void PreThumbnailRendering(ThumbnailCamera thumbnailCamera)
	{
		if (_waterOpacityService.IsWaterTransparent)
		{
			_waterOpacityService.ToggleOpacityOverride();
			_preRenderingWaterHidden = true;
		}
	}

	public void PostThumbnailRendering()
	{
		if (_preRenderingWaterHidden)
		{
			_waterOpacityService.ToggleOpacityOverride();
			_preRenderingWaterHidden = false;
		}
	}
}
