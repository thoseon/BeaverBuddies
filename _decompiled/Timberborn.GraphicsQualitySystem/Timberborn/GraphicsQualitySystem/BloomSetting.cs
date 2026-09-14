using System;

namespace Timberborn.GraphicsQualitySystem;

public class BloomSetting
{
	public static bool GetValueForPreset(GraphicsQualityPreset preset)
	{
		return preset switch
		{
			GraphicsQualityPreset.Ultra => true, 
			GraphicsQualityPreset.High => true, 
			GraphicsQualityPreset.Medium => true, 
			GraphicsQualityPreset.Low => false, 
			_ => throw new ArgumentException(), 
		};
	}
}
