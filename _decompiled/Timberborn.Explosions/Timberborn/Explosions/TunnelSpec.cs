using Timberborn.BlueprintSystem;

namespace Timberborn.Explosions;

internal record TunnelSpec : ComponentSpec
{
	[Serialize]
	public string ExplosionPrefabPath { get; init; }

	[Serialize]
	public string TunnelSupportTemplateName { get; init; }
}
