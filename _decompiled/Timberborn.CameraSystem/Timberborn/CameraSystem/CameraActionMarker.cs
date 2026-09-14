using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CameraSystem;

internal class CameraActionMarker : ILoadableSingleton
{
	private static readonly int MarkerHalfSize = 16;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private VisualElement _root;

	public CameraActionMarker(VisualElementLoader visualElementLoader, UILayout uiLayout)
	{
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/CameraActionMarker");
		_uiLayout.AddAbsoluteItem(_root);
		Hide();
	}

	public void ShowMarker(Vector2 positionNdc)
	{
		_root.ToggleDisplayStyle(visible: true);
		float width = _root.parent.resolvedStyle.width;
		float height = _root.parent.resolvedStyle.height;
		_root.style.left = positionNdc.x * width - (float)MarkerHalfSize;
		_root.style.top = (1f - positionNdc.y) * height - (float)MarkerHalfSize;
	}

	public void Hide()
	{
		_root.ToggleDisplayStyle(visible: false);
	}
}
