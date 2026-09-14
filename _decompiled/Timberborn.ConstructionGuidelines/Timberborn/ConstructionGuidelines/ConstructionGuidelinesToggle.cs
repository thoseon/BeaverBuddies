namespace Timberborn.ConstructionGuidelines;

public class ConstructionGuidelinesToggle
{
	public bool Visible { get; private set; }

	public void ShowGuidelines()
	{
		Visible = true;
	}

	public void HideGuidelines()
	{
		Visible = false;
	}
}
