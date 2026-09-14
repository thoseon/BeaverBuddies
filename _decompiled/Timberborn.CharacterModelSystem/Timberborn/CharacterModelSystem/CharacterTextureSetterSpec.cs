using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.CharacterModelSystem;

internal record CharacterTextureSetterSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<CharacterTexturePack> TexturePacks { get; init; }
}
