using UnityEngine;

namespace Timberborn.Localization;

public static class TextColors
{
	private static readonly Color GreenHighlight = new Color(0f, 0.8f, 0f);

	private static readonly Color RedHighlight = new Color(1f, 0.3f, 0.3f);

	private static readonly Color YellowHighlight = new Color(1f, 1f, 0.1f);

	private static readonly Color GreyHighlight = new Color(0.5f, 0.5f, 0.5f);

	public static string ColorizeText(string text)
	{
		return text.Replace("<GreenHighlight>", "<color=#" + ColorUtility.ToHtmlStringRGB(GreenHighlight) + ">").Replace("</GreenHighlight>", "</color>").Replace("<RedHighlight>", "<color=#" + ColorUtility.ToHtmlStringRGB(RedHighlight) + ">")
			.Replace("</RedHighlight>", "</color>")
			.Replace("<YellowHighlight>", "<color=#" + ColorUtility.ToHtmlStringRGB(YellowHighlight) + ">")
			.Replace("</YellowHighlight>", "</color>")
			.Replace("<GreyHighlight>", "<color=#" + ColorUtility.ToHtmlStringRGB(GreyHighlight) + ">")
			.Replace("</GreyHighlight>", "</color>");
	}
}
