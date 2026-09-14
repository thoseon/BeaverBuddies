using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.TimbermeshAnimations;

namespace Timberborn.Particles;

internal class AnimationParticlesTrigger : BaseComponent, IAwakableComponent, IUpdatableComponent
{
	private ParticlesCache _particlesCache;

	private IAnimator _animator;

	private AnimationParticlesTriggerSpec _animationParticlesTriggerSpec;

	private float _lastFrameTime;

	private readonly List<AnimationParticle> _runningParticles = new List<AnimationParticle>();

	private readonly Dictionary<AnimationParticle, ParticlesRunner> _particleRunners = new Dictionary<AnimationParticle, ParticlesRunner>();

	public void Awake()
	{
		_particlesCache = GetComponent<ParticlesCache>();
		_animator = GetComponentInChildren<IAnimator>(includeInactive: true);
		_animationParticlesTriggerSpec = GetComponent<AnimationParticlesTriggerSpec>();
		_animator.AnimationChanged += delegate
		{
			UpdateState();
		};
		DisableComponent();
	}

	public void Update()
	{
		float repeatedTime = _animator.RepeatedTime;
		foreach (AnimationParticle runningParticle in _runningParticles)
		{
			for (int i = 0; i < runningParticle.TriggerTimes.Length; i++)
			{
				UpdateTrigger(i, repeatedTime, runningParticle);
			}
		}
		_lastFrameTime = repeatedTime;
	}

	private void UpdateTrigger(int index, float animatorTime, AnimationParticle animationParticle)
	{
		float num = animationParticle.TriggerTimes[index];
		if (_lastFrameTime <= num && (animatorTime > num || animatorTime < _lastFrameTime))
		{
			_particleRunners[animationParticle].Play();
		}
	}

	private void UpdateState()
	{
		FindCurrentAnimationParticle();
		if (_runningParticles.Count > 0)
		{
			EnableComponent();
			CreateParticlesRunner();
			_lastFrameTime = _animator.RepeatedTime;
		}
		else
		{
			DisableComponent();
		}
	}

	private void FindCurrentAnimationParticle()
	{
		_runningParticles.Clear();
		foreach (AnimationParticle animationParticle in _animationParticlesTriggerSpec.AnimationParticles)
		{
			if (animationParticle.AnimationName == _animator.AnimationName)
			{
				_runningParticles.Add(animationParticle);
			}
		}
	}

	private void CreateParticlesRunner()
	{
		foreach (AnimationParticle runningParticle in _runningParticles)
		{
			if (!_particleRunners.ContainsKey(runningParticle))
			{
				string particlesAttachmentId = runningParticle.ParticlesAttachmentId;
				_particleRunners[runningParticle] = _particlesCache.GetParticlesRunner(particlesAttachmentId);
			}
		}
	}
}
