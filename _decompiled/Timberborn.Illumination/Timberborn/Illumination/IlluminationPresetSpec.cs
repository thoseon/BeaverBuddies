using Timberborn.BlueprintSystem;

namespace Timberborn.Illumination;

internal record IlluminationPresetSpec : ComponentSpec
{
	[Serialize]
	public int Order { get; init; }
}
