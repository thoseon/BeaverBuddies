using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;

namespace Timberborn.TimbermeshAnimations;

internal class TimbermeshAnimatorController : BaseComponent, IAwakableComponent, IAnimatorController
{
	private TimbermeshAnimator _animator;

	private TimbermeshAnimatorControllerSpec _spec;

	private readonly Dictionary<string, bool> _boolValues = new Dictionary<string, bool>();

	private readonly Dictionary<string, float> _floatValues = new Dictionary<string, float>();

	private AnimatorState _currentState;

	private bool _enabled = true;

	public IEnumerable<string> AnimationNames => _spec.AnimationNames;

	public void Awake()
	{
		_animator = base.GameObject.GetComponentsInChildren<TimbermeshAnimator>().Single();
		_spec = GetComponent<TimbermeshAnimatorControllerSpec>();
		ValidateAnimations();
		InitializeAllParameters();
		_animator.Enabled = true;
		UpdateState();
	}

	public bool HasParameter(string parameterName)
	{
		if (!_spec.BoolParameters.Contains(parameterName))
		{
			return _spec.FloatParameters.Contains(parameterName);
		}
		return true;
	}

	public void SetFloat(string parameterName, float value)
	{
		if (_floatValues[parameterName] != value)
		{
			_floatValues[parameterName] = value;
			if (_currentState != null && parameterName == _currentState.SpeedModifier)
			{
				UpdateAnimationSpeed();
			}
		}
	}

	public void SetBool(string parameterName, bool value)
	{
		if (_boolValues[parameterName] != value)
		{
			_boolValues[parameterName] = value;
			UpdateState();
		}
	}

	public void Enable()
	{
		if (!_enabled)
		{
			_enabled = true;
			UpdateState();
		}
	}

	public void Disable()
	{
		_enabled = false;
	}

	private void InitializeAllParameters()
	{
		foreach (string boolParameter in _spec.BoolParameters)
		{
			_boolValues.Add(boolParameter, value: false);
		}
		foreach (string floatParameter in _spec.FloatParameters)
		{
			_floatValues.Add(floatParameter, 1f);
		}
	}

	private void ValidateAnimations()
	{
		for (int i = 0; i < _spec.AnimatorStates.Length; i++)
		{
			string animationName = _spec.AnimatorStates[i].AnimationName;
			if (!_animator.HasAnimation(animationName))
			{
				throw new Exception("Missing animation: " + animationName + " in " + base.Name + " animator.");
			}
		}
	}

	private void UpdateState()
	{
		if (_enabled)
		{
			AnimatorState animatorState = FindBestMatchState();
			if (_currentState != animatorState)
			{
				SetAnimatorState(animatorState);
			}
		}
	}

	private AnimatorState FindBestMatchState()
	{
		AnimatorState result = _spec.AnimatorStates[0];
		int num = int.MinValue;
		for (int num2 = _spec.AnimatorStates.Length - 1; num2 >= 0; num2--)
		{
			AnimatorState animatorState = _spec.AnimatorStates[num2];
			if (IsExactMatch(animatorState, out var matchingStatesCount))
			{
				result = animatorState;
				break;
			}
			if (matchingStatesCount > num)
			{
				result = animatorState;
				num = matchingStatesCount;
			}
		}
		return result;
	}

	private bool IsExactMatch(AnimatorState animatorState, out int matchingStatesCount)
	{
		matchingStatesCount = 0;
		for (int i = 0; i < animatorState.Conditions.Length; i++)
		{
			AnimatorStateCondition animatorStateCondition = animatorState.Conditions[i];
			bool num = _boolValues[animatorStateCondition.ParameterName];
			bool flag = num && animatorStateCondition.MustBeTrue;
			bool flag2 = !num && !animatorStateCondition.MustBeTrue;
			if (flag | flag2)
			{
				matchingStatesCount++;
			}
		}
		return matchingStatesCount == animatorState.Conditions.Length;
	}

	private void SetAnimatorState(AnimatorState animatorState)
	{
		_currentState = animatorState;
		_animator.Play(_currentState.AnimationName, _currentState.Looped);
		UpdateAnimationSpeed();
	}

	private void UpdateAnimationSpeed()
	{
		float num = ((!string.IsNullOrWhiteSpace(_currentState.SpeedModifier)) ? _floatValues[_currentState.SpeedModifier] : 1f);
		_animator.Speed = _currentState.Speed * num;
	}
}
