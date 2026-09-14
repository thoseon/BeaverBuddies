using Timberborn.AssetSystem;
using Timberborn.BlueprintSystem;

namespace Timberborn.Timbermesh;

public record TimbermeshSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<BinaryData> Model { get; init; }
}
