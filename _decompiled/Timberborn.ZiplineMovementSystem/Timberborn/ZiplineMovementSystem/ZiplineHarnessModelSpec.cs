using Timberborn.BlueprintSystem;

namespace Timberborn.ZiplineMovementSystem;

internal record ZiplineHarnessModelSpec : ComponentSpec
{
	[Serialize]
	public string AttachmentId { get; init; }
}
