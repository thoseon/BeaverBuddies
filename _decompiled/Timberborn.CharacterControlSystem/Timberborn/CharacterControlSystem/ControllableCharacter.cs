using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.TimbermeshAnimations;
using Timberborn.WalkingSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.CharacterControlSystem;

public class ControllableCharacter : BaseComponent, IAwakableComponent, IPostInitializableEntity, IPersistentEntity
{
	private static readonly ComponentKey ControllableCharacterKey = new ComponentKey("ControllableCharacter");

	private static readonly PropertyKey<Vector3> DestinationKey = new PropertyKey<Vector3>("Destination");

	private static readonly PropertyKey<string> WaitAnimationKey = new PropertyKey<string>("WaitAnimation");

	private static readonly PropertyKey<bool> ForcedWalkingKey = new PropertyKey<bool>("ForcedWalking");

	private IAnimatorController _animatorController;

	private IAnimator _animator;

	private WalkingEnforcerToggle _walkingEnforcerToggle;

	public bool UnderControl { get; private set; }

	public Vector3 Destination { get; private set; }

	public string WaitAnimation { get; private set; }

	public bool ForcedWalking { get; private set; }

	public void Awake()
	{
		_animatorController = GetComponent<IAnimatorController>();
		_animator = GetComponentInChildren<IAnimator>();
		_walkingEnforcerToggle = GetComponent<WalkingEnforcer>().GetWalkingEnforcerToggle();
	}

	public void PostInitializeEntity()
	{
		if (ForcedWalking)
		{
			EnableForcedWalking();
		}
	}

	public void TakeControlAndMoveTo(Vector3 destination)
	{
		Destination = destination;
		UnderControl = true;
		ToggleAnimationControl(controlEnabled: false);
	}

	public void ChangeAnimation(string waitAnimation)
	{
		WaitAnimation = waitAnimation;
		if (UnderControl)
		{
			ToggleAnimationControl(controlEnabled: false);
			PlayAnimation();
		}
	}

	public void ReleaseControl()
	{
		UnderControl = false;
		ToggleAnimationControl(controlEnabled: false);
	}

	public IEnumerable<string> GetAnimationNames()
	{
		return _animatorController.AnimationNames;
	}

	public void PlayAnimation()
	{
		ToggleAnimationControl(controlEnabled: true);
		if (_animator.AnimationName != WaitAnimation)
		{
			_animator.Play(WaitAnimation);
		}
	}

	public void EnableForcedWalking()
	{
		_walkingEnforcerToggle.EnableForcedWalking();
	}

	public void DisableForcedWalking()
	{
		_walkingEnforcerToggle.DisableForcedWalking();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (UnderControl)
		{
			IObjectSaver component = entitySaver.GetComponent(ControllableCharacterKey);
			component.Set(DestinationKey, Destination);
			component.Set(WaitAnimationKey, WaitAnimation);
			component.Set(ForcedWalkingKey, _walkingEnforcerToggle.ForcedWalking);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(ControllableCharacterKey, out var objectLoader))
		{
			Destination = objectLoader.Get(DestinationKey);
			WaitAnimation = objectLoader.Get(WaitAnimationKey);
			ForcedWalking = objectLoader.Get(ForcedWalkingKey);
			UnderControl = true;
		}
	}

	private void ToggleAnimationControl(bool controlEnabled)
	{
		if (controlEnabled)
		{
			_animatorController.Disable();
		}
		else
		{
			_animatorController.Enable();
		}
	}
}
