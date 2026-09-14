using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

internal readonly struct StackedPanel
{
	public IPanelController PanelController { get; }

	public VisualElement VisualElement { get; }

	public bool TopHidden { get; }

	public bool IsOverlay { get; }

	public bool IsDialog { get; }

	public bool LockSpeed { get; }

	public StackedPanel(IPanelController panelController, VisualElement visualElement, bool topHidden, bool isOverlay, bool isDialog, bool lockSpeed)
	{
		PanelController = panelController;
		VisualElement = visualElement;
		TopHidden = topHidden;
		IsOverlay = isOverlay;
		IsDialog = isDialog;
		LockSpeed = lockSpeed;
	}
}
