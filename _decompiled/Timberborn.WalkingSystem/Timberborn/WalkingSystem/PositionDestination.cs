using System;
using System.Collections.Generic;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public class PositionDestination : IDestination, IEquatable<PositionDestination>
{
	private readonly INavigationService _navigationService;

	private readonly WalkerService _walkerService;

	public Vector3 Destination { get; }

	public float StoppingDistance { get; }

	public PositionDestination(INavigationService navigationService, WalkerService walkerService, Vector3 destination, float stoppingDistance)
	{
		_navigationService = navigationService;
		_walkerService = walkerService;
		Destination = destination;
		StoppingDistance = stoppingDistance;
	}

	public bool FindPath(Vector3 start, List<PathCorner> pathCorners, out float distance)
	{
		if (_navigationService.FindPathUnlimitedRange(start, Destination, pathCorners, out distance))
		{
			OffsetLastCornerInDirectionOfSecondLastCorner(pathCorners, StoppingDistance);
			return true;
		}
		distance = 0f;
		return false;
	}

	public bool Equals(PositionDestination other)
	{
		if ((object)other == null)
		{
			return false;
		}
		if ((object)this == other)
		{
			return true;
		}
		if (Destination.Equals(other.Destination))
		{
			return StoppingDistance.Equals(other.StoppingDistance);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((PositionDestination)obj);
	}

	public override int GetHashCode()
	{
		return (Destination.GetHashCode() * 397) ^ StoppingDistance.GetHashCode();
	}

	public static bool operator ==(PositionDestination left, PositionDestination right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(PositionDestination left, PositionDestination right)
	{
		return !object.Equals(left, right);
	}

	private void OffsetLastCornerInDirectionOfSecondLastCorner(List<PathCorner> pathCorners, float stoppingDistance)
	{
		if (pathCorners.Count > 1 && stoppingDistance != 0f)
		{
			if (stoppingDistance > 0.5f)
			{
				throw new ArgumentException("Stopping distance can't be bigger than " + $"{0.5f}");
			}
			int num = pathCorners.Count - 1;
			PathCorner pathCorner = pathCorners[num];
			Vector3 vector = pathCorners[num - 1].Position - pathCorner.Position;
			vector.y = 0f;
			Vector3 vector2 = vector.normalized * stoppingDistance;
			Vector3 vector3 = pathCorner.Position + vector2;
			if (!_navigationService.IsOnNavMesh(vector3))
			{
				ValidateHorizontalOffset(vector3 = _walkerService.ClosestPositionOnNavMesh(vector3), vector3);
			}
			pathCorners[num] = new PathCorner(vector3, pathCorner.Speed, pathCorner.GroupId);
		}
	}

	private static void ValidateHorizontalOffset(Vector3 correctedOffsetLastCorner, Vector3 offsetLastCorner)
	{
		Vector3 vector = correctedOffsetLastCorner - offsetLastCorner;
		vector.y = 0f;
		if (vector.magnitude > 0.001f)
		{
			Debug.LogWarning("Offset last corner had to be placed on nav mesh, by moving alongside x or z axis. Please report this.");
		}
	}
}
