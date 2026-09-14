using System;
using System.Collections.Generic;
using System.Linq;
using Bindito.Core;
using UnityEngine;

namespace Timberborn.TimbermeshAnimations;

internal class NodeAnimationUpdater : MonoBehaviour, IAnimationUpdater
{
	[HideInInspector]
	[SerializeField]
	private int _animationsId;

	private NodeAnimationCache _nodeAnimationCache;

	private Dictionary<string, NodeAnimation> _animationsMap;

	private NodeAnimation _currentAnimation;

	private bool _looped;

	private Transform _selfTransform;

	[Inject]
	public void InjectDependencies(NodeAnimationCache nodeAnimationCache)
	{
		_nodeAnimationCache = nodeAnimationCache;
	}

	public void Initialize()
	{
		_animationsMap = _nodeAnimationCache.GetAnimations(_animationsId).ToDictionary((NodeAnimation anim) => anim.Name, (NodeAnimation anim) => anim);
		_selfTransform = base.transform;
	}

	public void AssignAnimationsId(int animationSetId)
	{
		_animationsId = animationSetId;
	}

	public void SetAnimation(string animationName, bool looped)
	{
		_currentAnimation = null;
		if (_animationsMap.TryGetValue(animationName, out var value))
		{
			_currentAnimation = value;
			_looped = looped;
			ResetAnimationToInitialState();
		}
	}

	public void UpdateAnimation(float normalizedTime)
	{
		if (_currentAnimation != null)
		{
			int frameCount = _currentAnimation.FrameCount;
			if (_looped)
			{
				int num = Mathf.FloorToInt(normalizedTime * (float)frameCount) % frameCount;
				int toFrame = (num + 1) % frameCount;
				float weight = normalizedTime * (float)frameCount % 1f;
				UpdateTransform(num, toFrame, weight);
			}
			else
			{
				int num2 = frameCount - 1;
				int num3 = Mathf.FloorToInt(normalizedTime * (float)num2);
				int toFrame2 = Math.Clamp(num3 + 1, 0, num2);
				float weight2 = normalizedTime * (float)num2 % 1f;
				UpdateTransform(num3, toFrame2, weight2);
			}
		}
	}

	private void ResetAnimationToInitialState()
	{
		_selfTransform.SetLocalPositionAndRotation(_currentAnimation.GetPositionUnsafe(0), _currentAnimation.GetRotationUnsafe(0));
		_selfTransform.localScale = _currentAnimation.GetScaleUnsafe(0);
	}

	private void UpdateTransform(int fromFrame, int toFrame, float weight)
	{
		int fromFrame2 = Math.Clamp(fromFrame, 0, _currentAnimation.FrameCount - 1);
		int toFrame2 = Math.Clamp(toFrame, 0, _currentAnimation.FrameCount - 1);
		if (_currentAnimation.HasDifferentPositions && _currentAnimation.HasDifferentRotations)
		{
			_selfTransform.SetLocalPositionAndRotation(GetPosition(fromFrame2, toFrame2, weight), GetRotation(fromFrame2, toFrame2, weight));
		}
		else if (_currentAnimation.HasDifferentPositions)
		{
			_selfTransform.localPosition = GetPosition(fromFrame2, toFrame2, weight);
		}
		else if (_currentAnimation.HasDifferentRotations)
		{
			_selfTransform.localRotation = GetRotation(fromFrame2, toFrame2, weight);
		}
		if (_currentAnimation.HasDifferentScales)
		{
			_selfTransform.localScale = GetScale(fromFrame2, toFrame2, weight);
		}
	}

	private Vector3 GetPosition(int fromFrame, int toFrame, float weight)
	{
		Vector3 positionUnsafe = _currentAnimation.GetPositionUnsafe(fromFrame);
		Vector3 positionUnsafe2 = _currentAnimation.GetPositionUnsafe(toFrame);
		return Vector3.Lerp(positionUnsafe, positionUnsafe2, weight);
	}

	private Quaternion GetRotation(int fromFrame, int toFrame, float weight)
	{
		Quaternion rotationUnsafe = _currentAnimation.GetRotationUnsafe(fromFrame);
		Quaternion rotationUnsafe2 = _currentAnimation.GetRotationUnsafe(toFrame);
		return Quaternion.Lerp(rotationUnsafe, rotationUnsafe2, weight);
	}

	private Vector3 GetScale(int fromFrame, int toFrame, float weight)
	{
		Vector3 scaleUnsafe = _currentAnimation.GetScaleUnsafe(fromFrame);
		Vector3 scaleUnsafe2 = _currentAnimation.GetScaleUnsafe(toFrame);
		return Vector3.Lerp(scaleUnsafe, scaleUnsafe2, weight);
	}
}
