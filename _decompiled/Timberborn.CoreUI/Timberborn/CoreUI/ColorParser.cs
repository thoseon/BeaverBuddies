using UnityEngine;

namespace Timberborn.CoreUI;

public static class ColorParser
{
	public static bool TryGetColorFromText(string text, out Color color)
	{
		if (!string.IsNullOrEmpty(text))
		{
			int num = text.IndexOf('#');
			if (num != -1 && num + 7 <= text.Length)
			{
				return ColorUtility.TryParseHtmlString(text.Substring(num, 7), out color);
			}
		}
		color = default(Color);
		return false;
	}
}
