using Timberborn.NeedSpecs;

namespace Timberborn.NeedSystem;

public class NeedChangedIsFavorableEventArgs
{
	public NeedSpec NeedSpec { get; }

	public NeedChangedIsFavorableEventArgs(NeedSpec needSpec)
	{
		NeedSpec = needSpec;
	}
}
