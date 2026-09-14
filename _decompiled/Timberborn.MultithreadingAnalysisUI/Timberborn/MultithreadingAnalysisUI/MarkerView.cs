using Timberborn.Common;
using Timberborn.MultithreadingAnalysis;
using UnityEngine.UIElements;

namespace Timberborn.MultithreadingAnalysisUI;

internal class MarkerView
{
	private static readonly float PixelWidth = 3f;

	private readonly Marker _marker;

	public VisualElement Root { get; }

	public MarkerView(VisualElement root, Marker marker)
	{
		Root = root;
		_marker = marker;
	}

	public string GetTooltipText()
	{
		return "<b>" + _marker.Id + "</b>\nThread: " + _marker.Thread.DisplayName();
	}

	public void SetScale(float scale, long referenceTimestamp)
	{
		Root.style.left = new StyleLength(new Length(scale * (float)(_marker.Timestamp - referenceTimestamp) - 0.5f * PixelWidth, LengthUnit.Pixel));
		Root.style.width = new StyleLength(new Length(PixelWidth, LengthUnit.Pixel));
	}
}
