using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Particles;
using Timberborn.TickSystem;

namespace Timberborn.MechanicalSystem;

internal class MechanicalNodeParticlesController : TickableComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener, IParticlesSpeedMultiplier
{
	private MechanicalNode _mechanicalNode;

	private MechanicalNodeParticlesControllerSpec _mechanicalNodeParticlesControllerSpec;

	private ParticlesRunner _particlesRunner;

	public float SpeedMultiplier
	{
		get
		{
			if (!ShouldPlayParticles)
			{
				return 1f;
			}
			return _mechanicalNode.PowerEfficiency;
		}
	}

	private bool ShouldPlayParticles
	{
		get
		{
			if (_mechanicalNode.ActiveAndPowered)
			{
				return _mechanicalNode.PowerEfficiency > _mechanicalNodeParticlesControllerSpec.MinEfficiency;
			}
			return false;
		}
	}

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_mechanicalNodeParticlesControllerSpec = GetComponent<MechanicalNodeParticlesControllerSpec>();
		DisableComponent();
	}

	public void InitializeEntity()
	{
		_particlesRunner = GetComponent<ParticlesCache>().GetParticlesRunner(_mechanicalNodeParticlesControllerSpec.AttachmentIds);
		_particlesRunner.AddParticleSpeedMultiplier(this);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public override void Tick()
	{
		if (ShouldPlayParticles)
		{
			_particlesRunner.Play();
			_particlesRunner.UpdateSimulationSpeed();
		}
		else
		{
			_particlesRunner.Stop();
		}
	}
}
