using System;
using UnityEngine;

namespace Timberborn.HttpApiSystem;

internal class HttpApiUrlGenerator
{
	public string SwitchOnLeverUrlPath(string name)
	{
		return "/api/switch-on/" + Uri.EscapeDataString(name);
	}

	public string SwitchOffLeverUrlPath(string name)
	{
		return "/api/switch-off/" + Uri.EscapeDataString(name);
	}

	public string ColorLeverUrlPath(string name, Color color)
	{
		return "/api/color/" + Uri.EscapeDataString(name) + "/" + FormatColor(color);
	}

	private static string FormatColor(Color color)
	{
		return ColorUtility.ToHtmlStringRGB(color).ToLowerInvariant();
	}
}
