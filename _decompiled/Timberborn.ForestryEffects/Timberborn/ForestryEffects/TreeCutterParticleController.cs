using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Forestry;
using Timberborn.Particles;
using Timberborn.WalkingSystem;

namespace Timberborn.ForestryEffects;

internal class TreeCutterParticleController : BaseComponent, IAwakableComponent
{
	private ParticlesCache _particlesCache;

	private SwimmingAnimator _swimmingAnimator;

	private TreeCutterParticleControllerSpec _treeCutterParticleControllerSpec;

	private ParticlesRunner _particlesRunner;

	public void Awake()
	{
		_particlesCache = GetComponent<ParticlesCache>();
		_swimmingAnimator = GetComponent<SwimmingAnimator>();
		_treeCutterParticleControllerSpec = GetComponent<TreeCutterParticleControllerSpec>();
		TreeCutter component = GetComponent<TreeCutter>();
		component.CuttingStarted += StartCutting;
		component.CuttingStopped += StopCutting;
	}

	private void StartCutting(object sender, EventArgs eventArgs)
	{
		CreateParticlesRunner();
		UpdateParticlesState();
		_swimmingAnimator.SwimmingStateChanged += OnSwimmingStateChanged;
	}

	private void StopCutting(object sender, EventArgs eventArgs)
	{
		_particlesRunner.Stop();
		_swimmingAnimator.SwimmingStateChanged -= OnSwimmingStateChanged;
	}

	private void OnSwimmingStateChanged(object sender, EventArgs e)
	{
		UpdateParticlesState();
	}

	private void CreateParticlesRunner()
	{
		if (_particlesRunner == null)
		{
			string particlesAttachmentId = _treeCutterParticleControllerSpec.ParticlesAttachmentId;
			_particlesRunner = _particlesCache.GetParticlesRunner(particlesAttachmentId);
		}
	}

	private void UpdateParticlesState()
	{
		if (_swimmingAnimator.IsSwimming)
		{
			_particlesRunner.Stop();
		}
		else
		{
			_particlesRunner.Play();
		}
	}
}
