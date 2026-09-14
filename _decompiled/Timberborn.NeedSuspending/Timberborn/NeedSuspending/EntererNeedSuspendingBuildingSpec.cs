using Timberborn.BlueprintSystem;

namespace Timberborn.NeedSuspending;

internal record EntererNeedSuspendingBuildingSpec : ComponentSpec
{
	[Serialize]
	public NeedSuspender NeedSuspender { get; init; }
}
