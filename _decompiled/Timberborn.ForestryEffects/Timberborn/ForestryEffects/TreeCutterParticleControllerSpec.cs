using Timberborn.BlueprintSystem;

namespace Timberborn.ForestryEffects;

internal record TreeCutterParticleControllerSpec : ComponentSpec
{
	[Serialize]
	public string ParticlesAttachmentId { get; init; }
}
