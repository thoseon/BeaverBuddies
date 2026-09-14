using Timberborn.BlueprintSystem;

namespace Timberborn.Healthcare;

internal record BeaverInjuryTextureSet
{
	[Serialize]
	public string DiffusePath { get; init; }

	[Serialize]
	public string NormalMapPath { get; init; }

	[Serialize]
	public string DisplacementPath { get; init; }
}
