using System;
using UnityEngine;

namespace Timberborn.CameraSystem;

internal static class CappedTime
{
	public static float CappedUnscaledDeltaTime()
	{
		return Math.Min(Time.unscaledDeltaTime, 0.2f);
	}
}
