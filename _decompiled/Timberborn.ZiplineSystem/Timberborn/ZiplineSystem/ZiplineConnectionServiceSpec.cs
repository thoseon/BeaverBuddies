using Timberborn.BlueprintSystem;

namespace Timberborn.ZiplineSystem;

internal record ZiplineConnectionServiceSpec : ComponentSpec
{
	[Serialize]
	public int MaxCableInclination { get; init; }
}
