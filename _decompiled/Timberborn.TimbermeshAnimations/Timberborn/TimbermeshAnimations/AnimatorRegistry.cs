using System.Collections.Generic;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.TimbermeshAnimations;

internal class AnimatorRegistry : IUpdatableSingleton
{
	private readonly List<TimbermeshAnimator> _animators = new List<TimbermeshAnimator>();

	public void UpdateSingleton()
	{
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < _animators.Count; i++)
		{
			TimbermeshAnimator timbermeshAnimator = _animators[i];
			if ((bool)timbermeshAnimator && timbermeshAnimator.isActiveAndEnabled)
			{
				timbermeshAnimator.UpdateAnimation(deltaTime);
			}
		}
	}

	public void Add(TimbermeshAnimator timbermeshAnimator)
	{
		_animators.Add(timbermeshAnimator);
	}

	public void Remove(TimbermeshAnimator timbermeshAnimator)
	{
		_animators.Remove(timbermeshAnimator);
	}
}
