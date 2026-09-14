namespace Timberborn.SelectionSystem;

public class SelectableObjectUnselectedEvent
{
	public SelectableObject SelectableObject { get; }

	public SelectableObjectUnselectedEvent(SelectableObject selectableObject)
	{
		SelectableObject = selectableObject;
	}
}
