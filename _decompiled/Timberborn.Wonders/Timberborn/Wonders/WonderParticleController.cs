using System;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Particles;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Wonders;

internal class WonderParticleController : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener, IPersistentEntity, IFastForwardableParticles
{
	private static readonly ComponentKey WonderParticleControllerKey = new ComponentKey("WonderParticleController");

	private static readonly PropertyKey<float> EmissionDurationKey = new PropertyKey<float>("EmissionDuration");

	private Wonder _wonder;

	private ParticlesRunner _particlesRunner;

	private float _emissionStartTime;

	public float FastForwardDuration { get; private set; }

	public void Awake()
	{
		_wonder = GetComponent<Wonder>();
	}

	public void InitializeEntity()
	{
		ImmutableArray<string> attachmentIds = GetComponent<WonderParticleControllerSpec>().AttachmentIds;
		GetComponent<NonLinearParticlesSpeedMultiplier>().Disable();
		_particlesRunner = GetComponent<ParticlesCache>().GetParticlesRunner(attachmentIds);
		if (FastForwardDuration > 0f)
		{
			_emissionStartTime = Time.time - FastForwardDuration;
			_particlesRunner.Play();
		}
	}

	public void OnEnterFinishedState()
	{
		_wonder.WonderActivated += OnWonderActivated;
	}

	public void OnExitFinishedState()
	{
		_wonder.WonderActivated -= OnWonderActivated;
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_particlesRunner.IsPlaying)
		{
			entitySaver.GetComponent(WonderParticleControllerKey).Set(value: Time.time - _emissionStartTime, key: EmissionDurationKey);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(WonderParticleControllerKey, out var objectLoader))
		{
			FastForwardDuration = objectLoader.Get(EmissionDurationKey);
		}
	}

	private void OnWonderActivated(object sender, EventArgs e)
	{
		_emissionStartTime = Time.time;
		_particlesRunner.Play();
	}
}
