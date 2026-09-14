using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.Particles;

public class FinishedStateParticlesSpeedMultiplier : BaseComponent, IAwakableComponent, IFinishedStateListener, IParticlesSpeedMultiplier
{
	private BlockObject _blockObject;

	private readonly List<ParticlesRunner> _particlesRunner = new List<ParticlesRunner>();

	public float SpeedMultiplier { get; private set; }

	public bool IsValid => _blockObject != null;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void AddParticlesRunner(ParticlesRunner particlesRunner)
	{
		_particlesRunner.Add(particlesRunner);
		particlesRunner.AddParticleSpeedMultiplier(this);
		if (_blockObject.IsFinished)
		{
			Enable();
		}
	}

	public void OnEnterFinishedState()
	{
		Enable();
	}

	public void OnExitFinishedState()
	{
		UpdateSpeedMultiplier(0);
	}

	private void Enable()
	{
		UpdateSpeedMultiplier(1);
	}

	private void UpdateSpeedMultiplier(int speedMultiplier)
	{
		foreach (ParticlesRunner item in _particlesRunner)
		{
			SpeedMultiplier = speedMultiplier;
			item.UpdateSimulationSpeed();
		}
	}
}
