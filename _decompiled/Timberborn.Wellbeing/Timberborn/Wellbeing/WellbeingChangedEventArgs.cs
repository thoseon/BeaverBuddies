namespace Timberborn.Wellbeing;

public struct WellbeingChangedEventArgs(int oldWellbeing, int newWellbeing)
{
	public readonly int OldWellbeing = oldWellbeing;

	public readonly int NewWellbeing = newWellbeing;
}
