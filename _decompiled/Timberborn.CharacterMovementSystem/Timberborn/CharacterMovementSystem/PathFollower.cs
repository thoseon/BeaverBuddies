using System;
using System.Collections.Generic;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.CharacterMovementSystem;

public class PathFollower
{
	private static readonly float RemainingTimeThreshold = 0.0001f;

	private static readonly float RemainingDistanceThreshold = 0.0001f;

	private static readonly float MaxMovementStep = 0.1f;

	private static readonly int SpeedModifyingThreshold = 3;

	private readonly INavigationService _navigationService;

	private readonly MovementAnimator _movementAnimator;

	private readonly Transform _transform;

	private readonly List<AnimatedPathCorner> _animatedPathCorners = new List<AnimatedPathCorner>(100);

	private IReadOnlyList<PathCorner> _pathCorners;

	private int _nextCornerIndex;

	private bool _movedAlongPath;

	public event EventHandler<MovementEventArgs> MovedAlongPath;

	public PathFollower(INavigationService navigationService, MovementAnimator movementAnimator, Transform transform)
	{
		_navigationService = navigationService;
		_movementAnimator = movementAnimator;
		_transform = transform;
	}

	public void StartMovingAlongPath(IReadOnlyList<PathCorner> pathCorners)
	{
		_pathCorners = pathCorners;
		_nextCornerIndex = 1;
		_movedAlongPath = false;
	}

	public void MoveAlongPath(float tickDeltaTime, string animationName, Func<float> movementSpeedProvider)
	{
		float? speedLimitIfCloseToTarget = GetSpeedLimitIfCloseToTarget(tickDeltaTime, movementSpeedProvider);
		float num = speedLimitIfCloseToTarget ?? GetMovementSpeed(movementSpeedProvider);
		int groupId = _pathCorners[_nextCornerIndex - 1].GroupId;
		float remainingTime = tickDeltaTime;
		bool reachedTarget = false;
		float timeFromLastPathPoint = GetTimeFromLastPathPoint();
		_animatedPathCorners.Clear();
		AddAnimatedPathCorner(_transform.position, timeFromLastPathPoint, num, groupId);
		while (remainingTime > RemainingTimeThreshold && !ReachedLastPathCorner())
		{
			if (reachedTarget)
			{
				_nextCornerIndex = ((_nextCornerIndex + 1 < _pathCorners.Count) ? (_nextCornerIndex + 1) : _nextCornerIndex);
				num = speedLimitIfCloseToTarget ?? GetMovementSpeed(movementSpeedProvider);
			}
			int groupId2 = _pathCorners[_nextCornerIndex - 1].GroupId;
			if (num < float.MaxValue)
			{
				Vector3 position = _pathCorners[_nextCornerIndex].Position;
				Vector3 position2 = MoveInDirection(_transform.position, position, num, ref remainingTime, out reachedTarget);
				float time = timeFromLastPathPoint + tickDeltaTime - remainingTime;
				AddAnimatedPathCorner(position2, time, num, groupId2);
			}
			else
			{
				Vector3 position3 = _pathCorners[_nextCornerIndex].Position;
				float time2 = timeFromLastPathPoint + tickDeltaTime - remainingTime;
				AddAnimatedPathCorner(position3, time2, num, groupId2);
				reachedTarget = true;
			}
		}
		AddSmoothingAnimatedPathCorner(tickDeltaTime, reachedTarget, timeFromLastPathPoint, num);
		_movementAnimator.AnimateMovementAlongPath(_animatedPathCorners, animationName);
		_movedAlongPath = true;
		NotifyAfterMovement();
	}

	public void StopMoving()
	{
		_pathCorners = null;
		_movedAlongPath = false;
		_movementAnimator.StopAnimatingMovement();
	}

	public bool ReachedLastPathCorner()
	{
		INavigationService navigationService = _navigationService;
		IReadOnlyList<PathCorner> pathCorners = _pathCorners;
		return navigationService.InStoppingProximity(pathCorners[pathCorners.Count - 1].Position, _transform.position);
	}

	private float? GetSpeedLimitIfCloseToTarget(float tickDeltaTime, Func<float> movementSpeedProvider)
	{
		if (_nextCornerIndex >= _pathCorners.Count - SpeedModifyingThreshold)
		{
			float movementSpeed = GetMovementSpeed(movementSpeedProvider);
			float remainingDistance = GetRemainingDistance();
			float f = remainingDistance / movementSpeed / tickDeltaTime;
			int num = Mathf.Max(1, Mathf.RoundToInt(f));
			return remainingDistance / ((float)num * tickDeltaTime);
		}
		return null;
	}

	private float GetMovementSpeed(Func<float> movementSpeedProvider)
	{
		float speed = _pathCorners[_nextCornerIndex - 1].Speed;
		if (!(speed < float.MaxValue))
		{
			return float.MaxValue;
		}
		return speed * movementSpeedProvider();
	}

	private float GetRemainingDistance()
	{
		float num = Vector3.Distance(_transform.position, _pathCorners[_nextCornerIndex].Position);
		for (int i = _nextCornerIndex; i < _pathCorners.Count - 1; i++)
		{
			num += Vector3.Distance(_pathCorners[i].Position, _pathCorners[i + 1].Position);
		}
		return num;
	}

	private float GetTimeFromLastPathPoint()
	{
		if (_movedAlongPath && _animatedPathCorners.Count > 1)
		{
			float time = Time.time;
			List<AnimatedPathCorner> animatedPathCorners = _animatedPathCorners;
			if (time - animatedPathCorners[animatedPathCorners.Count - 2].Time > 0f)
			{
				List<AnimatedPathCorner> animatedPathCorners2 = _animatedPathCorners;
				return animatedPathCorners2[animatedPathCorners2.Count - 2].Time;
			}
		}
		return Time.time;
	}

	private static Vector3 MoveInDirection(Vector3 position, Vector3 target, float speed, ref float remainingTime, out bool reachedTarget)
	{
		Vector3 vector = target - position;
		float magnitude = vector.magnitude;
		if (magnitude < RemainingDistanceThreshold)
		{
			reachedTarget = true;
			return target;
		}
		float num = Mathf.Min(speed * remainingTime, MaxMovementStep);
		if (magnitude > num)
		{
			remainingTime -= num / speed;
			Vector3 vector2 = vector.normalized * num;
			reachedTarget = false;
			return position + vector2;
		}
		remainingTime -= magnitude / speed;
		reachedTarget = true;
		return target;
	}

	private void AddAnimatedPathCorner(Vector3 position, float time, float speed, int groupId)
	{
		float distanceToPathCorner = Vector3.Distance(position, _pathCorners[_nextCornerIndex].Position);
		_transform.position = position;
		_animatedPathCorners.Add(new AnimatedPathCorner(position, time, speed, distanceToPathCorner, groupId));
	}

	private void AddSmoothingAnimatedPathCorner(float tickDeltaTime, bool reachedLastTarget, float startingTime, float speed)
	{
		if (!ReachedLastPathCorner())
		{
			int index = Math.Min(reachedLastTarget ? (_nextCornerIndex + 1) : _nextCornerIndex, _pathCorners.Count - 1);
			PathCorner pathCorner = _pathCorners[index];
			float num = Vector3.Distance(_transform.position, pathCorner.Position);
			float time = startingTime + tickDeltaTime + num / speed;
			_animatedPathCorners.Add(new AnimatedPathCorner(pathCorner.Position, time, speed, num, pathCorner.GroupId));
		}
	}

	private void NotifyAfterMovement()
	{
		PathCorner pathCorner = _pathCorners[_nextCornerIndex - 1];
		PathCorner to = _pathCorners[_nextCornerIndex];
		if (_nextCornerIndex + 1 < _pathCorners.Count)
		{
			MovedAlongPath?.Invoke(this, new MovementEventArgs(pathCorner, to, _pathCorners[_nextCornerIndex + 1]));
		}
		else
		{
			MovedAlongPath?.Invoke(this, new MovementEventArgs(pathCorner, to, null));
		}
	}
}
