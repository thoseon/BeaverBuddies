namespace Timberborn.SelectionSystem;

public class SelectableObjectSelectedEvent
{
	public SelectableObject SelectableObject { get; }

	public SelectableObjectSelectedEvent(SelectableObject selectableObject)
	{
		SelectableObject = selectableObject;
	}
}
