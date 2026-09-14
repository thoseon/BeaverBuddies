namespace Timberborn.CoreUI;

public class PanelHiddenEvent
{
	public bool AnyPanelShown { get; }

	public bool UnlockSpeed { get; }

	public bool WasDialog { get; }

	public PanelHiddenEvent(bool anyPanelShown, bool unlockSpeed, bool wasDialog)
	{
		AnyPanelShown = anyPanelShown;
		UnlockSpeed = unlockSpeed;
		WasDialog = wasDialog;
	}
}
