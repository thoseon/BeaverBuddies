namespace Timberborn.BuildingsNavigation;

public class RangeChangedEventArgs
{
	public bool IsInitialChange { get; }

	public RangeChangedEventArgs(bool isInitialChange)
	{
		IsInitialChange = isInitialChange;
	}
}
