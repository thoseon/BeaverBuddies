namespace Timberborn.EnterableSystem;

public class EnteredEnterableEventArgs
{
	public Enterable Enterable { get; }

	public EnteredEnterableEventArgs(Enterable enterable)
	{
		Enterable = enterable;
	}
}
