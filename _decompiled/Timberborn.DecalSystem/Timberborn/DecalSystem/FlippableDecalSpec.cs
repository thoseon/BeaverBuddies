using Timberborn.BlueprintSystem;

namespace Timberborn.DecalSystem;

internal record FlippableDecalSpec : ComponentSpec
{
	[Serialize]
	public string DecalName { get; init; }
}
