using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.Particles;

public class ParticlesRunner
{
	private readonly ImmutableArray<ParticlesObject> _particleObjects;

	private readonly List<IParticlesSpeedMultiplier> _particleSpeedMultipliers = new List<IParticlesSpeedMultiplier>();

	public BaseComponent Owner { get; }

	public bool IsPlaying
	{
		get
		{
			if (_particleObjects.Length > 0)
			{
				return _particleObjects.Any((ParticlesObject particleObject) => particleObject.IsPlaying);
			}
			return false;
		}
	}

	private ParticlesRunner(BaseComponent owner, IEnumerable<ParticlesObject> particleObjects)
	{
		Owner = owner;
		_particleObjects = particleObjects.ToImmutableArray();
	}

	public static ParticlesRunner Create(BaseComponent owner, IEnumerable<ParticleSystem> particleSystems)
	{
		return new ParticlesRunner(owner, particleSystems.Select(ParticlesObject.Create));
	}

	public void AddParticleSpeedMultiplier(IParticlesSpeedMultiplier particlesSpeedMultiplier)
	{
		_particleSpeedMultipliers.Add(particlesSpeedMultiplier);
	}

	public void RemoveParticleSpeedMultiplier(IParticlesSpeedMultiplier particlesSpeedMultiplier)
	{
		_particleSpeedMultipliers.Remove(particlesSpeedMultiplier);
	}

	public void Play()
	{
		for (int i = 0; i < _particleObjects.Length; i++)
		{
			_particleObjects[i].Play();
		}
	}

	public void Stop()
	{
		for (int i = 0; i < _particleObjects.Length; i++)
		{
			_particleObjects[i].Stop();
		}
	}

	public void Enable()
	{
		for (int i = 0; i < _particleObjects.Length; i++)
		{
			_particleObjects[i].Enable();
		}
	}

	public void Disable()
	{
		for (int i = 0; i < _particleObjects.Length; i++)
		{
			_particleObjects[i].Disable();
		}
	}

	public void EnableEmission()
	{
		for (int i = 0; i < _particleObjects.Length; i++)
		{
			_particleObjects[i].EnableEmission();
		}
	}

	public void SetEmissionRate(float rate)
	{
		for (int i = 0; i < _particleObjects.Length; i++)
		{
			_particleObjects[i].SetEmissionRate(rate);
		}
	}

	public void DisableEmission()
	{
		for (int i = 0; i < _particleObjects.Length; i++)
		{
			_particleObjects[i].DisableEmission();
		}
	}

	public void UpdateSimulationSpeed()
	{
		float speedMultiplier = GetSpeedMultiplier();
		for (int i = 0; i < _particleObjects.Length; i++)
		{
			_particleObjects[i].UpdateSimulationSpeed(speedMultiplier);
		}
	}

	public void FastForward(float simulationTime)
	{
		for (int i = 0; i < _particleObjects.Length; i++)
		{
			_particleObjects[i].FastForward(simulationTime);
		}
	}

	public bool HasParticles()
	{
		for (int i = 0; i < _particleObjects.Length; i++)
		{
			if (_particleObjects[i].ParticleCount() > 0)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateForceMultiplier(Vector3 forceMultiplier)
	{
		for (int i = 0; i < _particleObjects.Length; i++)
		{
			_particleObjects[i].UpdateForceMultiplier(forceMultiplier);
		}
	}

	private float GetSpeedMultiplier()
	{
		float num = 1f;
		for (int i = 0; i < _particleSpeedMultipliers.Count; i++)
		{
			num *= _particleSpeedMultipliers[i].SpeedMultiplier;
		}
		return num;
	}
}
