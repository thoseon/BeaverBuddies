using Timberborn.NeedSpecs;

namespace Timberborn.NeedSystem;

public class NeedChangedCriticalStateEventArgs
{
	public NeedSpec NeedSpec { get; }

	public bool IsInCriticalState { get; }

	public NeedChangedCriticalStateEventArgs(NeedSpec needSpec, bool isInCriticalState)
	{
		NeedSpec = needSpec;
		IsInCriticalState = isInCriticalState;
	}
}
