using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.CharacterMovementSystem;

public class AnimatedPathFollower
{
	private readonly List<AnimatedPathCorner> _pathCorners = new List<AnimatedPathCorner>();

	private int _nextCornerIndex;

	private bool _isPositionedBeforeMovement;

	public Vector3 CurrentPosition { get; private set; }

	public Vector3 CurrentDirection { get; private set; }

	public float CurrentSpeed { get; private set; }

	public float CurrentXRotation { get; private set; }

	public float CurrentDistanceToPathCorner { get; private set; }

	public int CurrentGroupId { get; private set; }

	public bool Stopped => _pathCorners.Count == 0;

	public void SetNewPath(IEnumerable<AnimatedPathCorner> pathCorners)
	{
		ClearCurrentPath();
		_pathCorners.AddRange(pathCorners);
		if (!_isPositionedBeforeMovement)
		{
			_isPositionedBeforeMovement = true;
			PlaceAtCorner(_pathCorners[0]);
		}
	}

	public void Stop()
	{
		_isPositionedBeforeMovement = false;
		ClearCurrentPath();
	}

	public void Update(float timeInSeconds)
	{
		MoveNextCornerIndex(timeInSeconds);
		if (!ReachedDestination())
		{
			PlaceAtCorrectPosition(timeInSeconds);
		}
	}

	public bool ReachedDestination()
	{
		return _nextCornerIndex >= _pathCorners.Count;
	}

	public float LastCornerTime()
	{
		if (!_pathCorners.IsEmpty())
		{
			return _pathCorners.Last().Time;
		}
		return 0f;
	}

	private void ClearCurrentPath()
	{
		_pathCorners.Clear();
		_nextCornerIndex = 0;
	}

	private void MoveNextCornerIndex(float timeInSeconds)
	{
		for (int i = _nextCornerIndex; i < _pathCorners.Count; i++)
		{
			if (timeInSeconds < _pathCorners[i].Time)
			{
				_nextCornerIndex = i;
				return;
			}
		}
		_nextCornerIndex = _pathCorners.Count + 1;
	}

	private void PlaceAtCorrectPosition(float timeInSeconds)
	{
		AnimatedPathCorner animatedPathCorner = _pathCorners[_nextCornerIndex];
		if (_nextCornerIndex == 0)
		{
			PlaceAtCorner(animatedPathCorner);
			return;
		}
		AnimatedPathCorner previousCorner = _pathCorners[_nextCornerIndex - 1];
		PlaceBetweenCorners(previousCorner, animatedPathCorner, timeInSeconds);
	}

	private void PlaceAtCorner(AnimatedPathCorner animatedPathCorner)
	{
		CurrentPosition = animatedPathCorner.Position;
		CurrentDirection = Vector3.zero;
		CurrentSpeed = animatedPathCorner.Speed;
		CurrentDistanceToPathCorner = animatedPathCorner.DistanceToPathCorner;
		CurrentGroupId = animatedPathCorner.GroupId;
		CurrentXRotation = 0f;
	}

	private void PlaceBetweenCorners(AnimatedPathCorner previousCorner, AnimatedPathCorner nextCorner, float timeInSeconds)
	{
		Vector3 vector = nextCorner.Position - previousCorner.Position;
		float num = nextCorner.Time - previousCorner.Time;
		float num2 = (timeInSeconds - previousCorner.Time) / num;
		Vector3 vector2 = vector * num2;
		CurrentPosition = previousCorner.Position + vector2;
		CurrentDirection = vector.normalized;
		CurrentXRotation = ((vector == Vector3.zero) ? 0f : Quaternion.LookRotation(vector).eulerAngles.x);
		CurrentDistanceToPathCorner = nextCorner.DistanceToPathCorner;
		CurrentSpeed = previousCorner.Speed;
		CurrentGroupId = previousCorner.GroupId;
	}
}
