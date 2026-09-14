using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.CharacterMovementSystem;

public class MovementAnimator : BaseComponent, IAwakableComponent, IInitializableEntity, IUpdatableComponent, IDeletableEntity, IPersistentEntity
{
	private static readonly float RotationNotificationThreshold = 0.01f;

	private static readonly string AnimatorSpeedPropertyName = "WalkingSpeed";

	private static readonly ComponentKey MovementAnimatorKey = new ComponentKey("MovementAnimator");

	private static readonly PropertyKey<Vector3> PositionKey = new PropertyKey<Vector3>("Position");

	private static readonly PropertyKey<float> LeftTimeKey = new PropertyKey<float>("LeftTimeInSeconds");

	private static readonly PropertyKey<string> AnimationNameKey = new PropertyKey<string>("AnimationName");

	private static readonly PropertyKey<float> AnimationSpeedKey = new PropertyKey<float>("AnimationSpeed");

	private readonly AnimatedPathFollower _animatedPathFollower;

	private CharacterAnimator _characterAnimator;

	private CharacterModel _characterModel;

	private CharacterRotator _characterRotator;

	private MovementAnimatorSpec _movementAnimatorSpec;

	private string _animationName;

	private float _animationSpeed;

	private int _groupId;

	private float _xRotation;

	private Vector3? _loadedPosition;

	private float _loadedLeftTime;

	public event EventHandler<AnimationUpdatedEventArgs> AnimationUpdated;

	public event EventHandler<GroupIdUpdatedEventArgs> GroupIdUpdated;

	public event EventHandler<float> XRotationUpdated;

	public MovementAnimator(AnimatedPathFollower animatedPathFollower)
	{
		_animatedPathFollower = animatedPathFollower;
	}

	public void Awake()
	{
		_characterAnimator = GetComponent<CharacterAnimator>();
		_characterModel = GetComponent<CharacterModel>();
		_characterRotator = GetComponent<CharacterRotator>();
		_movementAnimatorSpec = GetComponent<MovementAnimatorSpec>();
	}

	public void InitializeEntity()
	{
		_characterRotator.Initialize(_animatedPathFollower);
		AnimateMovementFromLoadedPositionToObjectPosition();
	}

	public void Update()
	{
		if (Time.deltaTime > 0f)
		{
			Update(Time.deltaTime);
		}
	}

	public void DeleteEntity()
	{
		StopAnimatingMovement();
	}

	public void SetPostLoadGroupId(int groupId)
	{
		_groupId = groupId;
	}

	public void AnimateMovementAlongPath(IEnumerable<AnimatedPathCorner> pathCorners, string animationName)
	{
		_animatedPathFollower.SetNewPath(pathCorners);
		_characterModel.Position = _animatedPathFollower.CurrentPosition;
		StartAnimation(animationName, _animatedPathFollower.CurrentSpeed);
		NotifyGroupIdUpdated();
		NotifyRotationUpdated();
	}

	public void StopAnimatingMovement()
	{
		_characterModel.ResetModelPosition();
		_animatedPathFollower.Stop();
		_characterRotator.ResetXRotation();
		StopAnimation();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (!_animatedPathFollower.ReachedDestination())
		{
			IObjectSaver component = entitySaver.GetComponent(MovementAnimatorKey);
			component.Set(PositionKey, _animatedPathFollower.CurrentPosition);
			component.Set(value: _animatedPathFollower.LastCornerTime() - Time.time, key: LeftTimeKey);
			component.Set(AnimationNameKey, _animationName);
			component.Set(AnimationSpeedKey, _animationSpeed);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(MovementAnimatorKey, out var objectLoader))
		{
			_loadedPosition = objectLoader.Get(PositionKey);
			_loadedLeftTime = objectLoader.Get(LeftTimeKey);
			_animationName = objectLoader.Get(AnimationNameKey);
			_animationSpeed = objectLoader.Get(AnimationSpeedKey);
		}
	}

	private void AnimateMovementFromLoadedPositionToObjectPosition()
	{
		Vector3? loadedPosition = _loadedPosition;
		if (loadedPosition.HasValue)
		{
			Vector3 valueOrDefault = loadedPosition.GetValueOrDefault();
			_characterModel.Position = valueOrDefault;
			float distanceToPathCorner = Vector3.Distance(base.Transform.position, valueOrDefault);
			List<AnimatedPathCorner> pathCorners = new List<AnimatedPathCorner>
			{
				new AnimatedPathCorner(valueOrDefault, Time.time, _animationSpeed, distanceToPathCorner, _groupId),
				new AnimatedPathCorner(base.Transform.position, Time.time + _loadedLeftTime, _animationSpeed, 0f, _groupId)
			};
			AnimateMovementAlongPath(pathCorners, _animationName);
			UpdateCharacterAnimator();
			NotifyAnimationUpdated();
			NotifyRotationUpdated();
		}
	}

	private void Update(float deltaTime)
	{
		_animatedPathFollower.Update(Time.time);
		if (!_animatedPathFollower.Stopped)
		{
			UpdateTransform(deltaTime);
			UpdateAnimationSpeed();
			UpdateGroupId();
		}
		NotifyAnimationUpdated();
		UpdateRotation();
	}

	private void NotifyAnimationUpdated()
	{
		float animationSpeed = _animationSpeed * _movementAnimatorSpec.AnimationSpeedScale;
		AnimationUpdated?.Invoke(this, new AnimationUpdatedEventArgs(animationSpeed));
	}

	private void UpdateTransform(float deltaTime)
	{
		_characterModel.Position = _animatedPathFollower.CurrentPosition;
		_characterModel.Rotation = _characterRotator.GetCharacterRotation(deltaTime);
	}

	private void UpdateAnimationSpeed()
	{
		float currentSpeed = _animatedPathFollower.CurrentSpeed;
		if (_animationSpeed != currentSpeed)
		{
			_animationSpeed = currentSpeed;
			UpdateSpeedInCharacterAnimator();
		}
	}

	private void UpdateGroupId()
	{
		int currentGroupId = _animatedPathFollower.CurrentGroupId;
		if (_groupId != currentGroupId)
		{
			_groupId = currentGroupId;
			NotifyGroupIdUpdated();
		}
	}

	private void NotifyGroupIdUpdated()
	{
		GroupIdUpdated?.Invoke(this, new GroupIdUpdatedEventArgs(_groupId));
	}

	private void UpdateRotation()
	{
		float currentXRotation = _animatedPathFollower.CurrentXRotation;
		if (Mathf.Abs(_xRotation - currentXRotation) > RotationNotificationThreshold)
		{
			_xRotation = currentXRotation;
			NotifyRotationUpdated();
		}
	}

	private void NotifyRotationUpdated()
	{
		XRotationUpdated?.Invoke(this, _xRotation);
	}

	private void StartAnimation(string animationName, float animationSpeed)
	{
		if (_animationName != animationName || _animationSpeed != animationSpeed)
		{
			_animationName = animationName;
			_animationSpeed = animationSpeed;
			UpdateCharacterAnimator();
		}
	}

	private void StopAnimation()
	{
		if (_animationName != null)
		{
			_characterAnimator.SetBool(_animationName, value: false);
		}
		_animationName = null;
		_animationSpeed = 0f;
	}

	private void UpdateCharacterAnimator()
	{
		_characterAnimator.SetBool(_animationName, value: true);
		UpdateSpeedInCharacterAnimator();
	}

	private void UpdateSpeedInCharacterAnimator()
	{
		_characterAnimator.SetFloat(AnimatorSpeedPropertyName, _animationSpeed * _movementAnimatorSpec.AnimationSpeedScale);
	}
}
