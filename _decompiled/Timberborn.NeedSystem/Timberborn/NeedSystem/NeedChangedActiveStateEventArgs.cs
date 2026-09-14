using Timberborn.NeedSpecs;

namespace Timberborn.NeedSystem;

public struct NeedChangedActiveStateEventArgs
{
	public NeedSpec NeedSpec { get; }

	public bool IsActive { get; }

	public NeedChangedActiveStateEventArgs(NeedSpec needSpec, bool isActive)
	{
		NeedSpec = needSpec;
		IsActive = isActive;
	}
}
