using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.CharacterMovementSystem;
using Timberborn.EntitySystem;
using Timberborn.WalkingSystem;
using UnityEngine;

namespace Timberborn.ZiplineMovementSystem;

internal class ZiplineCharacterAnimator : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly float AngleThreshold = 20f;

	private static readonly string AnimationName = "Zipline";

	private static readonly string AnimationUpName = "ZiplineUp";

	private static readonly string AnimationDownName = "ZiplineDown";

	private CharacterAnimator _characterAnimator;

	private ZiplineVisitor _ziplineVisitor;

	private MovementAnimator _movementAnimator;

	private WalkingEnforcerToggle _walkingEnforcerToggle;

	public void Awake()
	{
		_characterAnimator = GetComponent<CharacterAnimator>();
		_ziplineVisitor = GetComponent<ZiplineVisitor>();
		_movementAnimator = GetComponent<MovementAnimator>();
		_walkingEnforcerToggle = GetComponent<WalkingEnforcer>().GetWalkingEnforcerToggle();
	}

	public void InitializeEntity()
	{
		if (_characterAnimator.HasParameter(AnimationName))
		{
			_ziplineVisitor.EnteredZipline += OnZiplineEntered;
			_ziplineVisitor.ExitedZipline += OnZiplineExited;
		}
	}

	private void OnZiplineEntered(object sender, EventArgs e)
	{
		_walkingEnforcerToggle.EnableForcedWalking();
		_characterAnimator.SetBool(AnimationName, value: true);
		_movementAnimator.XRotationUpdated += OnXRotationUpdated;
	}

	private void OnZiplineExited(object sender, EventArgs e)
	{
		_walkingEnforcerToggle.DisableForcedWalking();
		_characterAnimator.SetBool(AnimationName, value: false);
		_characterAnimator.SetBool(AnimationUpName, value: false);
		_characterAnimator.SetBool(AnimationDownName, value: false);
		_movementAnimator.XRotationUpdated -= OnXRotationUpdated;
	}

	private void OnXRotationUpdated(object sender, float xRotation)
	{
		float num = ((Mathf.Abs(xRotation) > 180f) ? (xRotation - Mathf.Sign(xRotation) * 360f) : xRotation);
		if (num > AngleThreshold)
		{
			_characterAnimator.SetBool(AnimationUpName, value: false);
			_characterAnimator.SetBool(AnimationDownName, value: true);
		}
		else if (num < 0f - AngleThreshold)
		{
			_characterAnimator.SetBool(AnimationUpName, value: true);
			_characterAnimator.SetBool(AnimationDownName, value: false);
		}
		else
		{
			_characterAnimator.SetBool(AnimationUpName, value: false);
			_characterAnimator.SetBool(AnimationDownName, value: false);
		}
	}
}
