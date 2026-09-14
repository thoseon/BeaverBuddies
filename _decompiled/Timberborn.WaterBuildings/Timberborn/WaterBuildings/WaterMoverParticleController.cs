using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Particles;
using Timberborn.TickSystem;

namespace Timberborn.WaterBuildings;

internal class WaterMoverParticleController : TickableComponent, IAwakableComponent, IInitializableEntity, IPostLoadableEntity
{
	private WaterMover _waterMover;

	private ParticlesRunner _particlesRunner;

	public void Awake()
	{
		_waterMover = GetComponent<WaterMover>();
	}

	public void InitializeEntity()
	{
		ImmutableArray<string> attachmentIds = GetComponent<WaterMoverParticleControllerSpec>().AttachmentIds;
		_particlesRunner = GetComponent<ParticlesCache>().GetParticlesRunner(attachmentIds);
	}

	public void PostLoadEntity()
	{
		UpdateParticles();
	}

	public override void Tick()
	{
		UpdateParticles();
	}

	private void UpdateParticles()
	{
		if (_waterMover.CanMoveWater && _waterMover.EffectiveFlowRate > 0f)
		{
			_particlesRunner.Play();
		}
		else
		{
			_particlesRunner.Stop();
		}
	}
}
