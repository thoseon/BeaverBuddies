using Timberborn.BlueprintSystem;

namespace Timberborn.TubeSystem;

internal record TubePlatformSpec : ComponentSpec
{
	[Serialize]
	public string PlatformModelName { get; init; }
}
