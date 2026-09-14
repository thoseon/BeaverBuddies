using Timberborn.BlueprintSystem;

namespace Timberborn.WaterBuildingsUI;

internal record WaterOutputParticleSpec : ComponentSpec
{
	[Serialize]
	public string AttachmentId { get; init; }

	[Serialize]
	public float SpawnOffset { get; init; }

	[Serialize]
	public float FadeDistance { get; init; }

	[Serialize]
	public float MinimumLifetime { get; init; }
}
