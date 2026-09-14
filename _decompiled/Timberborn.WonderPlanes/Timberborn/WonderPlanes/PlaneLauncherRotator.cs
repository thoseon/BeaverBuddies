using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WonderPlanes;

internal class PlaneLauncherRotator : BaseComponent, IAwakableComponent, IUpdatableComponent, IPersistentEntity, IPostInitializableEntity
{
	private static readonly ComponentKey PlaneLauncherRotatorKey = new ComponentKey("PlaneLauncherRotator");

	private static readonly PropertyKey<float> RemainingRotationKey = new PropertyKey<float>("RemainingRotation");

	private static readonly PropertyKey<float> LoadedRotationKey = new PropertyKey<float>("LoadedRotation");

	private static readonly PropertyKey<float> RotationTimeKey = new PropertyKey<float>("RotationTime");

	private static readonly PropertyKey<float> RotationDurationKey = new PropertyKey<float>("RotationDuration");

	private PlaneLauncherRotatorSpec _planeLauncherRotatorSpec;

	private AnimationCurve _rotationCurve;

	private Transform _rotatedElement;

	private float _remainingRotation;

	private float _loadedRotation;

	private float _rotationDuration;

	private float _rotationTime;

	private float CurrentRotation => _rotatedElement.localRotation.eulerAngles.y;

	public event EventHandler RotationFinished;

	public void Awake()
	{
		_planeLauncherRotatorSpec = GetComponent<PlaneLauncherRotatorSpec>();
		_rotationCurve = _planeLauncherRotatorSpec.RotationCurve.ToAnimationCurve();
		_rotatedElement = base.GameObject.FindChildTransform(_planeLauncherRotatorSpec.RotatedElementName);
		DisableComponent();
	}

	public void PostInitializeEntity()
	{
		if (_loadedRotation > 0f)
		{
			_rotatedElement.Rotate(Vector3.up, _loadedRotation);
		}
	}

	public void Update()
	{
		if (_remainingRotation > 0f)
		{
			UpdateRotation();
			return;
		}
		RotationFinished?.Invoke(this, EventArgs.Empty);
		DisableComponent();
	}

	public void StartRotation(float rotationAngle)
	{
		_remainingRotation = rotationAngle;
		_rotationDuration = _planeLauncherRotatorSpec.FullRotationDuration * rotationAngle / 360f;
		_rotationTime = 0f;
		EnableComponent();
	}

	public void RotateToOriginalPosition()
	{
		StartRotation(360f - CurrentRotation);
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(PlaneLauncherRotatorKey);
		component.Set(RemainingRotationKey, _remainingRotation);
		component.Set(LoadedRotationKey, CurrentRotation);
		component.Set(RotationTimeKey, _rotationTime);
		component.Set(RotationDurationKey, _rotationDuration);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(PlaneLauncherRotatorKey);
		_loadedRotation = component.Get(LoadedRotationKey);
		_remainingRotation = component.Get(RemainingRotationKey);
		_rotationTime = component.Get(RotationTimeKey);
		_rotationDuration = component.Get(RotationDurationKey);
		if (_remainingRotation > 0f || _loadedRotation > 0f)
		{
			EnableComponent();
		}
	}

	private void UpdateRotation()
	{
		_rotationTime += Time.deltaTime;
		float num = _rotationCurve.Evaluate(_rotationTime / _rotationDuration) * Time.deltaTime;
		if (num > _remainingRotation)
		{
			num = _remainingRotation;
		}
		_rotatedElement.Rotate(Vector3.up, num);
		_remainingRotation -= num;
	}
}
