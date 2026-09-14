using Timberborn.NeedSpecs;

namespace Timberborn.NeedSystem;

public class NeedChangedIsAtMinimumStateEventArgs
{
	public NeedSpec NeedSpec { get; }

	public bool IsAtMinimum { get; }

	public NeedChangedIsAtMinimumStateEventArgs(NeedSpec needSpec, bool isAtMinimum)
	{
		NeedSpec = needSpec;
		IsAtMinimum = isAtMinimum;
	}
}
