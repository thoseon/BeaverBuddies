using Timberborn.BlueprintSystem;

namespace Timberborn.EnterableSystem;

internal record EnterableAnimationControllerSpec : ComponentSpec
{
	[Serialize]
	public bool ResetAnimationUponExit { get; init; }
}
