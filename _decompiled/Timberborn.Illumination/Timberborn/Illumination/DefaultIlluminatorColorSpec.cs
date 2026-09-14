using Timberborn.BlueprintSystem;

namespace Timberborn.Illumination;

internal record DefaultIlluminatorColorSpec : ComponentSpec
{
	[Serialize]
	public string ColorId { get; init; }
}
