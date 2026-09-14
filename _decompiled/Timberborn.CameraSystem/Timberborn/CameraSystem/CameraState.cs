using System;
using UnityEngine;

namespace Timberborn.CameraSystem;

public readonly struct CameraState : IEquatable<CameraState>
{
	public Vector3 Target { get; }

	public float ZoomLevel { get; }

	public float HorizontalAngle { get; }

	public float VerticalAngle { get; }

	public CameraState(Vector3 target, float zoomLevel, float horizontalAngle, float verticalAngle)
	{
		Target = target;
		ZoomLevel = zoomLevel;
		HorizontalAngle = horizontalAngle;
		VerticalAngle = verticalAngle;
	}

	public bool Equals(CameraState other)
	{
		if (Target.Equals(other.Target) && ZoomLevel.Equals(other.ZoomLevel) && HorizontalAngle.Equals(other.HorizontalAngle))
		{
			return VerticalAngle.Equals(other.VerticalAngle);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is CameraState other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Target, ZoomLevel, HorizontalAngle, VerticalAngle);
	}

	public static bool operator ==(CameraState left, CameraState right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(CameraState left, CameraState right)
	{
		return !left.Equals(right);
	}
}
