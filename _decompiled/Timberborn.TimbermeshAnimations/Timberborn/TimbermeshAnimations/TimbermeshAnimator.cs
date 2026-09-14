using System;
using System.Collections.Generic;
using System.Linq;
using Bindito.Core;
using UnityEngine;

namespace Timberborn.TimbermeshAnimations;

internal class TimbermeshAnimator : MonoBehaviour, IAnimator
{
	[HideInInspector]
	[SerializeField]
	private List<AnimationMetadata> _animations;

	[HideInInspector]
	[SerializeField]
	private bool _playBackwards;

	private AnimatorRegistry _animatorRegistry;

	private IAnimationUpdater[] _animationUpdaters;

	private Dictionary<string, AnimationMetadata> _animationMap;

	private AnimationMetadata _currentAnimation;

	private float _speed = 1f;

	private bool _looped;

	private float _interruptionTime;

	private float _interruptionRepeatedTime;

	private int _interruptionFrame;

	private string _interruptionAnimation;

	public bool Enabled { get; set; }

	public float Time { get; private set; }

	public float RepeatedTime { get; private set; }

	public bool PlayingFinished { get; private set; }

	public float AnimationLength => _currentAnimation?.Length ?? 0f;

	public string AnimationName => _currentAnimation?.Name;

	public bool PlayBackwards
	{
		set
		{
			_playBackwards = value;
		}
	}

	public float Speed
	{
		set
		{
			if (float.IsNaN(value) || float.IsInfinity(value))
			{
				_speed = 0f;
			}
			else
			{
				_speed = value;
			}
		}
	}

	public event EventHandler AnimationChanged;

	[Inject]
	public void InjectDependencies(AnimatorRegistry animatorRegistry)
	{
		_animatorRegistry = animatorRegistry;
	}

	public void Awake()
	{
		_animationUpdaters = GetComponentsInChildren<IAnimationUpdater>(includeInactive: true);
		_animationMap = _animations.ToDictionary((AnimationMetadata anim) => anim.Name, (AnimationMetadata anim) => anim);
		InitializeAnimationUpdaters();
		Play(_animations.First().Name);
	}

	public void Start()
	{
		_animatorRegistry.Add(this);
	}

	public void OnDestroy()
	{
		_animatorRegistry.Remove(this);
	}

	public void SetTime(float time)
	{
		if (_currentAnimation != null)
		{
			Time = time;
			UpdateAnimationProgress();
			UpdateAnimationUpdaters();
		}
	}

	public bool HasAnimation(string animationName)
	{
		for (int i = 0; i < _animations.Count; i++)
		{
			if (_animations[i].Name == animationName)
			{
				return true;
			}
		}
		return false;
	}

	public void SetAnimations(IEnumerable<AnimationMetadata> animations)
	{
		_animations = new List<AnimationMetadata>(animations);
	}

	public void Play(string animationName, bool looped = true)
	{
		UpdateInterruptionState(animationName);
		SetAnimation(animationName, looped);
	}

	public void Stop()
	{
		Time = 0f;
		RepeatedTime = 0f;
		_looped = false;
		_currentAnimation = null;
	}

	public void UpdateAnimation(float deltaTime)
	{
		if (Enabled && _speed != 0f && _currentAnimation != null && !PlayingFinished)
		{
			UpdateTime(deltaTime);
			UpdateAnimationUpdaters();
		}
	}

	private void InitializeAnimationUpdaters()
	{
		for (int i = 0; i < _animationUpdaters.Length; i++)
		{
			_animationUpdaters[i].Initialize();
		}
	}

	private void UpdateInterruptionState(string animationName)
	{
		int frameCount = UnityEngine.Time.frameCount;
		bool flag = _currentAnimation != null && !PlayingFinished;
		if ((frameCount > _interruptionFrame) & flag)
		{
			_interruptionTime = Time;
			_interruptionRepeatedTime = RepeatedTime;
			_interruptionFrame = frameCount;
			_interruptionAnimation = AnimationName;
			Stop();
		}
		else
		{
			Stop();
			if (frameCount == _interruptionFrame && animationName == _interruptionAnimation)
			{
				Time = _interruptionTime;
				RepeatedTime = _interruptionRepeatedTime;
			}
		}
	}

	private void SetAnimation(string animationName, bool looped)
	{
		if (_animationMap.TryGetValue(animationName, out var value))
		{
			_currentAnimation = value;
			_looped = looped;
			PlayingFinished = false;
			for (int i = 0; i < _animationUpdaters.Length; i++)
			{
				_animationUpdaters[i].SetAnimation(animationName, _looped);
			}
			UpdateAnimationUpdaters();
			AnimationChanged?.Invoke(this, EventArgs.Empty);
			return;
		}
		throw new Exception("Animation " + animationName + " not found in " + base.gameObject.name + " animator.");
	}

	private void UpdateTime(float deltaTime)
	{
		Time += deltaTime * _speed;
		UpdateAnimationProgress();
	}

	private void UpdateAnimationProgress()
	{
		float length = _currentAnimation.Length;
		if (!_looped && Time >= length)
		{
			float time = (RepeatedTime = length);
			Time = time;
			PlayingFinished = true;
		}
		else
		{
			RepeatedTime = Mathf.Repeat(Time, length);
		}
	}

	private void UpdateAnimationUpdaters()
	{
		float num = Mathf.Clamp01(RepeatedTime / _currentAnimation.Length);
		float normalizedTime = (_playBackwards ? (1f - num) : num);
		for (int i = 0; i < _animationUpdaters.Length; i++)
		{
			_animationUpdaters[i].UpdateAnimation(normalizedTime);
		}
	}
}
