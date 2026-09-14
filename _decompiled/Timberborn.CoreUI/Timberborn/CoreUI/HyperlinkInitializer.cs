using System;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Timberborn.CoreUI;

public class HyperlinkInitializer
{
	public void Initialize(Label label, Action openLinkCallback)
	{
		AddUnityRequiredValue(label);
		string originalText = label.text;
		string highlightedText = originalText.Replace("<link=", "<color=#ffff00><link=").Replace("</link>", "</link></color>");
		label.RegisterCallback<PointerDownLinkTagEvent>(delegate
		{
			openLinkCallback();
		});
		label.RegisterCallback<PointerOverLinkTagEvent>(delegate
		{
			label.text = highlightedText;
		});
		label.RegisterCallback<PointerOutLinkTagEvent>(delegate
		{
			label.text = originalText;
		});
	}

	private static void AddUnityRequiredValue(Label label)
	{
		label.text = label.text.Replace("<link>", "<link=\"AnyValue\">");
	}
}
