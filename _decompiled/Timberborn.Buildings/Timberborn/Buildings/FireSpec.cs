using Timberborn.BlueprintSystem;

namespace Timberborn.Buildings;

internal record FireSpec : ComponentSpec
{
	[Serialize]
	public string AttachmentId { get; init; }

	[Serialize]
	public bool HasLight { get; init; }
}
