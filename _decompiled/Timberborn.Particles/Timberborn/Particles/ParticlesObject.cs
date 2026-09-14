using UnityEngine;

namespace Timberborn.Particles;

internal readonly struct ParticlesObject
{
	private readonly ParticleSystem _particleSystem;

	private readonly float _initialSpeed;

	public bool IsPlaying => _particleSystem.isPlaying;

	private ParticlesObject(ParticleSystem particleSystem, float initialSpeed)
	{
		_particleSystem = particleSystem;
		_initialSpeed = initialSpeed;
	}

	public static ParticlesObject Create(ParticleSystem particleSystem)
	{
		ParticleSystem.MainModule main = particleSystem.main;
		float simulationSpeed = main.simulationSpeed;
		main.simulationSpeed = 0f;
		particleSystem.Stop();
		return new ParticlesObject(particleSystem, simulationSpeed);
	}

	public void Play()
	{
		_particleSystem.Play();
	}

	public void Stop()
	{
		_particleSystem.Stop();
	}

	public void Enable()
	{
		_particleSystem.gameObject.SetActive(value: true);
	}

	public void Disable()
	{
		_particleSystem.gameObject.SetActive(value: false);
	}

	public void EnableEmission()
	{
		ParticleSystem.EmissionModule emission = _particleSystem.emission;
		emission.enabled = true;
	}

	public void SetEmissionRate(float rate)
	{
		ParticleSystem.EmissionModule emission = _particleSystem.emission;
		emission.rateOverTime = rate;
	}

	public void DisableEmission()
	{
		ParticleSystem.EmissionModule emission = _particleSystem.emission;
		emission.enabled = false;
	}

	public void UpdateSimulationSpeed(float speedMultiplier)
	{
		ParticleSystem.MainModule main = _particleSystem.main;
		main.simulationSpeed = _initialSpeed * speedMultiplier;
	}

	public void UpdateForceMultiplier(Vector3 forceMultiplier)
	{
		ParticleSystem.ForceOverLifetimeModule forceOverLifetime = _particleSystem.forceOverLifetime;
		forceOverLifetime.xMultiplier = forceMultiplier.x;
		forceOverLifetime.yMultiplier = forceMultiplier.y;
		forceOverLifetime.zMultiplier = forceMultiplier.z;
	}

	public void FastForward(float time)
	{
		float simulationSpeed = _particleSystem.main.simulationSpeed;
		UpdateSimulationSpeed(1f);
		_particleSystem.Play();
		_particleSystem.Simulate(time, withChildren: true, restart: false, fixedTimeStep: false);
		UpdateSimulationSpeed(simulationSpeed);
	}

	public int ParticleCount()
	{
		return _particleSystem.particleCount;
	}
}
