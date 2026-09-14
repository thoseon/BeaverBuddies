namespace Timberborn.CoreUI;

public class PanelShownEvent
{
	public bool IsDialog { get; }

	public bool LockSpeed { get; }

	public PanelShownEvent(bool isDialog, bool lockSpeed)
	{
		IsDialog = isDialog;
		LockSpeed = lockSpeed;
	}
}
