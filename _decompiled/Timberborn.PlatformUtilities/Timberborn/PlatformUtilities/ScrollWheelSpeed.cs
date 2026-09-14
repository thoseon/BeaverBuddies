using System;

namespace Timberborn.PlatformUtilities;

public static class ScrollWheelSpeed
{
	private static readonly float WindowsWheelScrollSize = 10f;

	private static readonly float MacOSWheelScrollSize = 5f;

	private static readonly float WindowsNormalizedScrollAxis = 2.8f;

	private static readonly float MacOsNormalizedScrollAxis = 14f;

	public static Lazy<float> WheelScrollSize { get; } = new Lazy<float>(() => (!ApplicationPlatform.IsMacOS()) ? WindowsWheelScrollSize : MacOSWheelScrollSize);

	public static Lazy<float> NormalizedScrollAxis { get; } = new Lazy<float>(() => (!ApplicationPlatform.IsMacOS()) ? WindowsNormalizedScrollAxis : MacOsNormalizedScrollAxis);
}
