using Timberborn.BlueprintSystem;

namespace Timberborn.Carrying;

internal record GoodCarrierModelSpec : ComponentSpec
{
	[Serialize]
	public string CarriedInHandsAttachmentName { get; init; }

	[Serialize]
	public string BackpackAttachmentName { get; init; }
}
