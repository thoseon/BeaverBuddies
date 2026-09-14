using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.WindSystem;

public class WindRotationAnimator : BaseComponent, IAwakableComponent, IUpdatableComponent, IInitializableEntity, IFinishedStateListener
{
	private class WindRotator
	{
		public WindRotatorSpec Spec { get; }

		public Transform Transform { get; }

		private WindRotator(WindRotatorSpec spec, Transform transform)
		{
			Spec = spec;
			Transform = transform;
		}

		public static WindRotator Create(WindRotatorSpec windRotatorSpec, GameObject parent)
		{
			Transform transform = parent.FindChildTransform(windRotatorSpec.TransformName);
			return new WindRotator(windRotatorSpec, transform);
		}
	}

	private readonly WindService _windService;

	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private bool _animationSuspended;

	private readonly List<WindRotator> _windRotators = new List<WindRotator>();

	private WindRotator _tower;

	public WindRotationAnimator(WindService windService, NonlinearAnimationManager nonlinearAnimationManager)
	{
		_windService = windService;
		_nonlinearAnimationManager = nonlinearAnimationManager;
	}

	public void Awake()
	{
		DisableComponent();
	}

	public void InitializeEntity()
	{
		WindRotationAnimatorSpec component = GetComponent<WindRotationAnimatorSpec>();
		foreach (WindRotatorSpec windRotator in component.WindRotators)
		{
			_windRotators.Add(WindRotator.Create(windRotator, base.GameObject));
		}
		if (!string.IsNullOrWhiteSpace(component.Tower.TransformName))
		{
			_tower = WindRotator.Create(component.Tower, base.GameObject);
		}
	}

	public void OnEnterFinishedState()
	{
		RotateTowerInstantly();
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public void Update()
	{
		UpdateAnimation();
	}

	public void SuspendAnimation()
	{
		_animationSuspended = true;
	}

	public void UnsuspendAnimation()
	{
		_animationSuspended = false;
	}

	private void UpdateAnimation()
	{
		if (!_animationSuspended)
		{
			float deltaTime = Time.deltaTime * _nonlinearAnimationManager.SpeedMultiplier * _windService.WindStrength;
			RotateTower(deltaTime);
			for (int i = 0; i < _windRotators.Count; i++)
			{
				RotateRotators(_windRotators[i], deltaTime);
			}
		}
	}

	private void RotateTowerInstantly()
	{
		if (_tower != null)
		{
			_tower.Transform.rotation = GetTargetTowerRotation();
		}
	}

	private void RotateTower(float deltaTime)
	{
		if (_tower != null)
		{
			_tower.Transform.rotation = Quaternion.RotateTowards(_tower.Transform.rotation, GetTargetTowerRotation(), deltaTime * _tower.Spec.RotationSpeed);
		}
	}

	private Quaternion GetTargetTowerRotation()
	{
		float num = Vector2.SignedAngle(Vector2.down, _windService.WindDirection);
		return Quaternion.Euler(_tower.Spec.RotationAxis * num);
	}

	private static void RotateRotators(WindRotator windRotator, float deltaTime)
	{
		windRotator.Transform.Rotate(windRotator.Spec.RotationAxis, deltaTime * windRotator.Spec.RotationSpeed);
	}
}
