using System;
using System.Collections.Generic;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public class AccessibleDestination : IDestination, IEquatable<AccessibleDestination>
{
	public Accessible Accessible { get; }

	public AccessibleDestination(Accessible accessible)
	{
		Accessible = accessible;
	}

	public bool FindPath(Vector3 start, List<PathCorner> pathCorners, out float distance)
	{
		if (!Accessible)
		{
			distance = 0f;
			return false;
		}
		return Accessible.FindPathUnlimitedRange(start, pathCorners, out distance);
	}

	public bool Equals(AccessibleDestination other)
	{
		if ((object)other == null)
		{
			return false;
		}
		if ((object)this == other)
		{
			return true;
		}
		return object.Equals(Accessible, other.Accessible);
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
		return Equals((AccessibleDestination)obj);
	}

	public override int GetHashCode()
	{
		if (Accessible == null)
		{
			return 0;
		}
		return Accessible.GetHashCode();
	}

	public static bool operator ==(AccessibleDestination left, AccessibleDestination right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(AccessibleDestination left, AccessibleDestination right)
	{
		return !object.Equals(left, right);
	}
}
