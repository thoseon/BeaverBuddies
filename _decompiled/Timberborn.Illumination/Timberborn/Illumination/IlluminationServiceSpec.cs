using Timberborn.BlueprintSystem;

namespace Timberborn.Illumination;

internal record IlluminationServiceSpec : ComponentSpec
{
	[Serialize]
	public string DefaultColorId { get; init; }

	[Serialize]
	public float IconExponent { get; init; }

	[Serialize]
	public float IconMultiplier { get; init; }
}
