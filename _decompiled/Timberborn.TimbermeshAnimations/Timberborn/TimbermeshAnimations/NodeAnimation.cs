using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.TimbermeshAnimations;

internal class NodeAnimation
{
	private static readonly float DistanceDifferenceValidationThreshold = 0.001f;

	private static readonly float AngleDifferenceValidationThreshold = 0.01f;

	private readonly Vector3[] _positions;

	private readonly Quaternion[] _rotations;

	private readonly Vector3[] _scales;

	public string Name { get; }

	public int FrameCount { get; }

	public bool HasDifferentPositions { get; private set; }

	public bool HasDifferentRotations { get; private set; }

	public bool HasDifferentScales { get; private set; }

	private NodeAnimation(string name, int frameCount, Vector3[] positions, Quaternion[] rotations, Vector3[] scales, bool hasDifferentPositions, bool hasDifferentRotations, bool hasDifferentScales)
	{
		Name = name;
		FrameCount = frameCount;
		_positions = positions;
		_rotations = rotations;
		_scales = scales;
		HasDifferentPositions = hasDifferentPositions;
		HasDifferentRotations = hasDifferentRotations;
		HasDifferentScales = hasDifferentScales;
	}

	public static NodeAnimation Create(string name, int frameCount, Vector3[] positions, Quaternion[] rotations, Vector3[] scales)
	{
		bool hasDifferentPositions = HasDifferentValues(positions);
		bool hasDifferentRotations = HasDifferentValues(rotations);
		bool hasDifferentScales = HasDifferentValues(scales);
		return new NodeAnimation(name, frameCount, positions, rotations, scales, hasDifferentPositions, hasDifferentRotations, hasDifferentScales);
	}

	public Vector3 GetPositionUnsafe(int frame)
	{
		return _positions[frame];
	}

	public Quaternion GetRotationUnsafe(int frame)
	{
		return _rotations[frame];
	}

	public Vector3 GetScaleUnsafe(int frame)
	{
		return _scales[frame];
	}

	private static bool HasDifferentValues(IReadOnlyList<Vector3> input)
	{
		Vector3 vector = input[0];
		for (int i = 1; i < input.Count; i++)
		{
			if ((input[i] - vector).magnitude > DistanceDifferenceValidationThreshold)
			{
				return true;
			}
		}
		return false;
	}

	private static bool HasDifferentValues(IReadOnlyList<Quaternion> input)
	{
		Quaternion b = input[0];
		for (int i = 1; i < input.Count; i++)
		{
			if (Quaternion.Angle(input[i], b) > AngleDifferenceValidationThreshold)
			{
				return true;
			}
		}
		return false;
	}
}
