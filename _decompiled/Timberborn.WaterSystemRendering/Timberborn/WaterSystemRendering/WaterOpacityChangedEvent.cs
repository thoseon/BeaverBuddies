namespace Timberborn.WaterSystemRendering;

public class WaterOpacityChangedEvent
{
	public bool IsWaterTransparent { get; }

	public WaterOpacityChangedEvent(bool isWaterTransparent)
	{
		IsWaterTransparent = isWaterTransparent;
	}
}
