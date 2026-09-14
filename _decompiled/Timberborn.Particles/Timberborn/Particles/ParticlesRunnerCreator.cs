using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.Particles;

internal class ParticlesRunnerCreator : BaseComponent, IAwakableComponent
{
	private FinishedStateParticlesSpeedMultiplier _finishedStateParticlesSpeedMultiplier;

	private NonLinearParticlesSpeedMultiplier _nonLinearParticlesSpeedMultiplier;

	public void Awake()
	{
		_finishedStateParticlesSpeedMultiplier = GetComponent<FinishedStateParticlesSpeedMultiplier>();
		_nonLinearParticlesSpeedMultiplier = GetComponent<NonLinearParticlesSpeedMultiplier>();
	}

	public ParticlesRunner Create(IEnumerable<ParticleSystem> particleSystems)
	{
		ParticlesRunner particlesRunner = ParticlesRunner.Create(this, particleSystems);
		_nonLinearParticlesSpeedMultiplier.AddParticlesRunner(particlesRunner);
		if (_finishedStateParticlesSpeedMultiplier.IsValid)
		{
			_finishedStateParticlesSpeedMultiplier.AddParticlesRunner(particlesRunner);
		}
		particlesRunner.UpdateSimulationSpeed();
		return particlesRunner;
	}
}
