using Timberborn.BlueprintSystem;

namespace Timberborn.WonderPlanes;

internal record PlaneSpawnerSpec : ComponentSpec
{
	[Serialize]
	public string SpawnPointName { get; init; }
}
