using System.Collections.Generic;
using Timberborn.SingletonSystem;

namespace Timberborn.Particles;

public class ParticlesFastForwarder : ILateUpdatableSingleton
{
	private static readonly float FastForwardDuration = 5f;

	private readonly List<ParticlesRunner> _particlesRunners = new List<ParticlesRunner>();

	private bool _isEnabled = true;

	public void LateUpdateSingleton()
	{
		if (_isEnabled)
		{
			FastForwardAllParticles();
			_isEnabled = false;
		}
	}

	public void Register(ParticlesRunner particlesRunner)
	{
		if (_isEnabled)
		{
			_particlesRunners.Add(particlesRunner);
		}
	}

	public void Unregister(ParticlesRunner particlesRunner)
	{
		if (_isEnabled)
		{
			_particlesRunners.Remove(particlesRunner);
		}
	}

	private void FastForwardAllParticles()
	{
		foreach (ParticlesRunner particlesRunner in _particlesRunners)
		{
			if (particlesRunner != null && particlesRunner.IsPlaying)
			{
				particlesRunner.FastForward(GetDuration(particlesRunner));
				particlesRunner.Play();
			}
		}
		_particlesRunners.Clear();
	}

	private static float GetDuration(ParticlesRunner particlesRunner)
	{
		return particlesRunner.Owner.GetComponent<IFastForwardableParticles>()?.FastForwardDuration ?? FastForwardDuration;
	}
}
