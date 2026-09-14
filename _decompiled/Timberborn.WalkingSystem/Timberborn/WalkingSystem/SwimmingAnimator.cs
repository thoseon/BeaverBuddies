using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.CharacterMovementSystem;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public class SwimmingAnimator : BaseComponent, IAwakableComponent, IPostLoadableEntity
{
	private static readonly float DivingDepthThreshold = 0.9f;

	private static readonly float MinDivingDepth = 0.5f;

	private static readonly float MinOffsettingSpeed = 6f;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private CharacterModel _characterModel;

	private CharacterAnimator _characterAnimator;

	private SwimmingAnimatorSpec _swimmingAnimatorSpec;

	private float _yModelPositionLastUpdate;

	private bool _updateModel;

	private bool _blockModel;

	public bool IsSwimming { get; private set; }

	public bool IsUnderwater { get; private set; }

	private Vector3Int Coordinates => NavigationCoordinateSystem.WorldToGridInt(_characterModel.Position);

	public event EventHandler SwimmingStateChanged;

	public event EventHandler UnderwaterStateChanged;

	public SwimmingAnimator(IThreadSafeWaterMap threadSafeWaterMap)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void Awake()
	{
		_characterModel = GetComponent<CharacterModel>();
		_characterAnimator = GetComponent<CharacterAnimator>();
		_swimmingAnimatorSpec = GetComponent<SwimmingAnimatorSpec>();
		GetComponent<NavMeshObserver>().PlacedOnNavMesh += delegate
		{
			InstantlyUpdateSwimming();
		};
		GetComponent<MovementAnimator>().AnimationUpdated += delegate(object _, AnimationUpdatedEventArgs e)
		{
			UpdateSwimming(e.AnimationSpeed);
		};
	}

	public void PostLoadEntity()
	{
		if (base.Enabled)
		{
			InstantlyUpdateSwimming();
		}
	}

	public void BlockSwimmingMovement()
	{
		_blockModel = true;
	}

	public void BlockSwimmingMovementAndResetPosition()
	{
		BlockSwimmingMovement();
		Vector3 position = _characterModel.Position;
		Vector3 position2 = base.Transform.position;
		_characterModel.Position = new Vector3(position.x, position2.y, position.z);
	}

	public void UnblockSwimmingMovement()
	{
		_blockModel = false;
		InstantlyUpdateSwimming();
	}

	private void InstantlyUpdateSwimming()
	{
		UpdateSwimming(0f, fastForward: true);
	}

	private void UpdateSwimming(float movementSpeed, bool fastForward = false)
	{
		float modelDepth = ModelDepth(movementSpeed);
		UpdateUnderwaterState(modelDepth);
		UpdateSwimmingState(modelDepth);
		_characterAnimator.SetBool("Swimming", IsSwimming);
		UpdateModel(movementSpeed, modelDepth, fastForward);
		_yModelPositionLastUpdate = _characterModel.Position.y;
	}

	private float ModelDepth(float movementSpeed)
	{
		return _threadSafeWaterMap.WaterHeightOrFloor(Coordinates) - YPosition(movementSpeed);
	}

	private void UpdateUnderwaterState(float modelDepth)
	{
		if (!IsUnderwater && modelDepth > 0f)
		{
			IsUnderwater = true;
			UnderwaterStateChanged?.Invoke(this, EventArgs.Empty);
		}
		else if (IsUnderwater && modelDepth <= 0f)
		{
			IsUnderwater = false;
			UnderwaterStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void UpdateSwimmingState(float modelDepth)
	{
		if (!IsSwimming && modelDepth > _swimmingAnimatorSpec.UpperSwimmingDepthThreshold)
		{
			IsSwimming = true;
			SwimmingStateChanged?.Invoke(this, EventArgs.Empty);
		}
		else if (IsSwimming && modelDepth < _swimmingAnimatorSpec.LowerSwimmingDepthThreshold)
		{
			IsSwimming = false;
			SwimmingStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void UpdateModel(float movementSpeed, float modelDepth, bool fastForward)
	{
		if (IsSwimming)
		{
			_updateModel = true;
		}
		if (_updateModel && !_blockModel)
		{
			float num = YPosition(movementSpeed) + SmoothOffset(modelDepth);
			float num2 = (fastForward ? num : SmoothlyOffsetNewYPosition(num, movementSpeed));
			Vector3 position = _characterModel.Position;
			_characterModel.Position = new Vector3(position.x, num2, position.z);
			if ((double)Math.Abs(num - num2) < 0.0001 && !IsSwimming)
			{
				_updateModel = false;
			}
			if (modelDepth < DivingDepthThreshold)
			{
				_characterModel.Rotation = Quaternion.Euler(0f, _characterModel.Rotation.eulerAngles.y, 0f);
			}
		}
	}

	private float YPosition(float movementSpeed)
	{
		if (!(movementSpeed > 0f))
		{
			return base.Transform.position.y;
		}
		return _characterModel.Position.y;
	}

	private float SmoothOffset(float modelDepth)
	{
		if (IsSwimming)
		{
			if (!(modelDepth < DivingDepthThreshold))
			{
				return SmoothDivingOffset(modelDepth);
			}
			return modelDepth;
		}
		return 0f;
	}

	private static float SmoothDivingOffset(float modelDepth)
	{
		return Math.Max(DivingDepthThreshold - (modelDepth - DivingDepthThreshold), MinDivingDepth);
	}

	private float SmoothlyOffsetNewYPosition(float targetYPosition, float movementSpeed)
	{
		float value = targetYPosition - _yModelPositionLastUpdate;
		float num = Time.deltaTime * Math.Max(movementSpeed, MinOffsettingSpeed);
		float num2 = Mathf.Clamp(value, 0f - num, num);
		return _yModelPositionLastUpdate + num2;
	}
}
