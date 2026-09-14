using Timberborn.BlueprintSystem;

namespace Timberborn.EntityNaming;

internal record NumberedEntityNamerSpec : ComponentSpec
{
	[Serialize]
	public string FormatLocKey { get; init; }

	[Serialize]
	public string NumberingGroup { get; init; }

	[Serialize]
	public bool IsPersistent { get; init; }
}
