namespace Timberborn.EnterableSystem;

public class EntererRemovedEventArgs
{
	public Enterer Enterer { get; }

	public EntererRemovedEventArgs(Enterer enterer)
	{
		Enterer = enterer;
	}
}
