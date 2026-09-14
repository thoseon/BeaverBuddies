using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Particles;
using Timberborn.Planting;
using Timberborn.WalkingSystem;

namespace Timberborn.PlantingEffects;

internal class PlantingParticleController : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private ParticlesCache _particlesCache;

	private SwimmingAnimator _swimmingAnimator;

	private ParticlesRunner _particlesRunner;

	private PlantExecutor _plantExecutor;

	public void Awake()
	{
		_particlesCache = GetComponent<ParticlesCache>();
		_swimmingAnimator = GetComponent<SwimmingAnimator>();
	}

	public void InitializeEntity()
	{
		_plantExecutor = GetComponent<PlantExecutor>();
		_plantExecutor.PlantingStarted += OnPlantingStarted;
		_plantExecutor.PlantingFinished += OnPlantingFinished;
		if (_plantExecutor.IsPlanting)
		{
			InitializePlantingParticles();
		}
	}

	private void OnPlantingStarted(object sender, EventArgs eventArgs)
	{
		InitializePlantingParticles();
	}

	private void OnPlantingFinished(object sender, EventArgs eventArgs)
	{
		_particlesRunner.Stop();
		_swimmingAnimator.UnderwaterStateChanged -= OnSwimmingStateChanged;
	}

	private void OnSwimmingStateChanged(object sender, EventArgs e)
	{
		UpdateParticlesState();
	}

	private void InitializePlantingParticles()
	{
		CreateParticlesRunner();
		UpdateParticlesState();
		_swimmingAnimator.UnderwaterStateChanged += OnSwimmingStateChanged;
	}

	private void CreateParticlesRunner()
	{
		if (_particlesRunner == null)
		{
			string particlesAttachmentId = GetComponent<PlantingParticleControllerSpec>().ParticlesAttachmentId;
			_particlesRunner = _particlesCache.GetParticlesRunner(particlesAttachmentId);
		}
	}

	private void UpdateParticlesState()
	{
		if (_swimmingAnimator.IsUnderwater)
		{
			_particlesRunner.Stop();
		}
		else
		{
			_particlesRunner.Play();
		}
	}
}
