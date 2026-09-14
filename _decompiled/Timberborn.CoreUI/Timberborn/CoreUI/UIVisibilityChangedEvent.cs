namespace Timberborn.CoreUI;

public class UIVisibilityChangedEvent
{
	public bool UIVisible { get; }

	public UIVisibilityChangedEvent(bool uiVisible)
	{
		UIVisible = uiVisible;
	}
}
