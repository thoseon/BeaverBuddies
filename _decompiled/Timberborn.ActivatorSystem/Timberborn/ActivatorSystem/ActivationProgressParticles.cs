using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Particles;
using UnityEngine;

namespace Timberborn.ActivatorSystem;

internal class ActivationProgressParticles : BaseComponent, IAwakableComponent, IInitializableEntity, IUpdatableComponent
{
	private ParticlesCache _particlesCache;

	private TimedComponentActivator _timedComponentActivator;

	private ActivationProgressParticlesSpec _spec;

	private ParticlesRunner _particlesRunner;

	public void Awake()
	{
		_particlesCache = GetComponent<ParticlesCache>();
		_timedComponentActivator = GetComponent<TimedComponentActivator>();
		_spec = GetComponent<ActivationProgressParticlesSpec>();
	}

	public void InitializeEntity()
	{
		_particlesRunner = _particlesCache.GetParticlesRunner(_spec.AttachmentIds);
		_particlesRunner.Disable();
		if (_timedComponentActivator.CountdownIsActive)
		{
			PlayParticles();
		}
		else if (!_timedComponentActivator.IsPastActivationTime)
		{
			_timedComponentActivator.CountdownActivated += OnCountdownActivated;
		}
		if (!_timedComponentActivator.IsPastActivationTime)
		{
			_timedComponentActivator.Activated += OnActivated;
		}
	}

	public void Update()
	{
		if (_particlesRunner != null)
		{
			float emissionRate = Mathf.Lerp(_spec.MinEmission, _spec.MaxEmission, _timedComponentActivator.ActivationProgress);
			_particlesRunner.SetEmissionRate(emissionRate);
		}
	}

	private void OnCountdownActivated(object sender, EventArgs e)
	{
		_timedComponentActivator.CountdownActivated -= OnCountdownActivated;
		PlayParticles();
	}

	private void OnActivated(object sender, EventArgs e)
	{
		_timedComponentActivator.CountdownActivated -= OnCountdownActivated;
		_timedComponentActivator.Activated -= OnActivated;
		_particlesRunner.Disable();
		_particlesRunner = null;
	}

	private void PlayParticles()
	{
		_particlesRunner.Enable();
		_particlesRunner.Play();
	}
}
