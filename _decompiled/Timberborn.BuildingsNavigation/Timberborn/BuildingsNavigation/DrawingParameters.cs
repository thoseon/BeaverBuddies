using System;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal readonly struct DrawingParameters : IEquatable<DrawingParameters>
{
	private readonly Vector3 _coordinates;

	private readonly Orientation _orientation;

	private readonly bool _isSingle;

	public bool IsPreview { get; }

	public DrawingParameters(bool isPreview, Vector3 coordinates, Orientation orientation, bool isSingle)
	{
		IsPreview = isPreview;
		_coordinates = coordinates;
		_orientation = orientation;
		_isSingle = isSingle;
	}

	public bool Equals(DrawingParameters other)
	{
		if (_coordinates.Equals(other._coordinates) && _orientation == other._orientation && _isSingle == other._isSingle)
		{
			return IsPreview == other.IsPreview;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is DrawingParameters other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_coordinates, _orientation, _isSingle, IsPreview);
	}
}
