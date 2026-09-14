using Timberborn.BlueprintSystem;

namespace Timberborn.PlantingEffects;

internal record PlantingParticleControllerSpec : ComponentSpec
{
	[Serialize]
	public string ParticlesAttachmentId { get; init; }
}
