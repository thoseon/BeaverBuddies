namespace Timberborn.EnterableSystem;

public class EntererAddedEventArgs
{
	public Enterer Enterer { get; }

	public EntererAddedEventArgs(Enterer enterer)
	{
		Enterer = enterer;
	}
}
