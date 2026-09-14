using Timberborn.BlueprintSystem;

namespace Timberborn.DeconstructionSystem;

internal record DeconstructionParticleFactorySpec : ComponentSpec
{
	[Serialize]
	public float MinParticleSpawnThreshold { get; init; }

	[Serialize]
	public float MaxParticleSpawnThreshold { get; init; }

	[Serialize]
	public int MinParticlesForThreshold { get; init; }

	[Serialize]
	public int MaxParticlesForThreshold { get; init; }

	[Serialize]
	public string ParticlePrefabPath { get; init; }

	[Serialize]
	public float MaxNeighboursCount { get; init; }
}
