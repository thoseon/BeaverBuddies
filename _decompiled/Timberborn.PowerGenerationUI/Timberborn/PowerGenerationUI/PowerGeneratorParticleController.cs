using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.MechanicalSystem;
using Timberborn.Particles;
using Timberborn.TickSystem;

namespace Timberborn.PowerGenerationUI;

internal class PowerGeneratorParticleController : TickableComponent, IAwakableComponent, IInitializableEntity
{
	private MechanicalNode _mechanicalNode;

	private ParticlesRunner _particlesRunner;

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
	}

	public void InitializeEntity()
	{
		ImmutableArray<string> attachmentIds = GetComponent<PowerGeneratorParticleControllerSpec>().AttachmentIds;
		_particlesRunner = GetComponent<ParticlesCache>().GetParticlesRunner(attachmentIds);
	}

	public override void Tick()
	{
		if (_mechanicalNode.ActiveAndPowered && _mechanicalNode.OutputMultiplier > 0f)
		{
			_particlesRunner.Play();
		}
		else
		{
			_particlesRunner.Stop();
		}
	}
}
