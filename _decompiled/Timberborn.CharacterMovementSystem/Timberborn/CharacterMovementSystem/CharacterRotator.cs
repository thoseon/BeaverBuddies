using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.CharacterMovementSystem;

internal class CharacterRotator : BaseComponent, IAwakableComponent
{
	private static readonly float XRotationSpeed = 330f;

	private static readonly float YRotationSpeed = 360f;

	private static readonly float MovementSpeedInfluence = 2.7f;

	private static readonly float CornerSnapMaxAngle = 80f;

	private static readonly float CornerSnapMaxDistance = 0.9f;

	private readonly NavMeshGroupService _navMeshGroupService;

	private CharacterModel _characterModel;

	private RunningProhibitor _runningProhibitor;

	private IMovementSpeedAffector _movementSpeedAffector;

	private AnimatedPathFollower _animatedPathFollower;

	public CharacterRotator(NavMeshGroupService navMeshGroupService)
	{
		_navMeshGroupService = navMeshGroupService;
	}

	public void Awake()
	{
		_characterModel = GetComponent<CharacterModel>();
		_runningProhibitor = GetComponent<RunningProhibitor>();
		_movementSpeedAffector = GetComponent<IMovementSpeedAffector>();
	}

	public void Initialize(AnimatedPathFollower animatedPathFollower)
	{
		_animatedPathFollower = animatedPathFollower;
	}

	public Quaternion GetCharacterRotation(float deltaTime)
	{
		return Quaternion.Euler(GetXRotation(deltaTime), GetYRotation(deltaTime), 0f);
	}

	public void ResetXRotation()
	{
		_characterModel.Rotation = Quaternion.Euler(0f, _characterModel.Rotation.eulerAngles.y, 0f);
	}

	private float GetXRotation(float deltaTime)
	{
		float target = (IsRunning() ? _animatedPathFollower.CurrentXRotation : 0f);
		float x = _characterModel.Rotation.eulerAngles.x;
		float b = _animatedPathFollower.CurrentSpeed / MovementSpeedInfluence;
		float num = XRotationSpeed * Mathf.Max(1f, b);
		return Mathf.MoveTowardsAngle(x, target, deltaTime * num);
	}

	private float GetYRotation(float deltaTime)
	{
		float y = _characterModel.Rotation.eulerAngles.y;
		Vector3 currentDirection = _animatedPathFollower.CurrentDirection;
		if (!currentDirection.Equals(Vector3.zero) && !currentDirection.Equals(Vector3.down) && !currentDirection.Equals(Vector3.up))
		{
			float num = MinimizeRotation(Quaternion.LookRotation(currentDirection).eulerAngles.y - y);
			float num2 = CalculateYRotationSpeed(num) * deltaTime;
			float num3 = Mathf.Clamp(num, 0f - num2, num2);
			return y + num3;
		}
		return y;
	}

	private float CalculateYRotationSpeed(float angleToTarget)
	{
		float num = Mathf.Abs(angleToTarget);
		float b = _animatedPathFollower.CurrentSpeed / MovementSpeedInfluence;
		float num2 = YRotationSpeed * Mathf.Max(1f, b);
		float currentDistanceToPathCorner = _animatedPathFollower.CurrentDistanceToPathCorner;
		float num3 = num / num2;
		float num4 = currentDistanceToPathCorner / _animatedPathFollower.CurrentSpeed;
		if (currentDistanceToPathCorner <= CornerSnapMaxDistance && num <= CornerSnapMaxAngle && num4 > 0f && _animatedPathFollower.CurrentGroupId == _navMeshGroupService.GetDefaultGroupId())
		{
			return num / num4;
		}
		if (num4 < num3 && num4 > 0f)
		{
			return Mathf.Max(num2, num2 * (num3 / num4));
		}
		return num2;
	}

	private bool IsRunning()
	{
		bool flag = _movementSpeedAffector?.IsMovementSlowed ?? false;
		if (!_runningProhibitor.RunningProhibited)
		{
			return !flag;
		}
		return false;
	}

	private static float MinimizeRotation(float rotation)
	{
		if (!(Mathf.Abs(rotation) > 180f))
		{
			return rotation;
		}
		return rotation - Mathf.Sign(rotation) * 360f;
	}
}
