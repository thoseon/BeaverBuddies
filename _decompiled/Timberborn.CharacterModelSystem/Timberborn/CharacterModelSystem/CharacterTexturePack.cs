using Timberborn.BlueprintSystem;

namespace Timberborn.CharacterModelSystem;

internal record CharacterTexturePack
{
	[Serialize]
	public string DiffuseTexture { get; init; }

	[Serialize]
	public string EmissionTexture { get; init; }

	[Serialize]
	public string NormalTexture { get; init; }

	[Serialize]
	public string DisplacementTexture { get; init; }
}
