using Timberborn.PrefabOptimization;
using UnityEngine;

namespace Timberborn.Coordinates;

public readonly struct OrientationTransform(Orientation orientation) : ITransform
{
	private readonly Orientation _orientation = orientation;

	public void MultiplyPoints(Vector3[] source, Vector3[] destination, int destinationIndex, int count)
	{
		for (int i = 0; i < count; i++)
		{
			destination[destinationIndex + i] = _orientation.TransformInWorldSpace(source[i]);
		}
	}

	public void MultiplyNormals(Vector3[] source, Vector3[] destination, int destinationIndex, int count)
	{
		MultiplyPoints(source, destination, destinationIndex, count);
	}

	public void MultiplyTangents(Vector4[] source, Vector4[] destination, int destinationIndex, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Vector4 vector = source[i];
			Vector3 vector2 = _orientation.TransformInWorldSpace(vector);
			destination[destinationIndex + i] = new Vector4(vector2.x, vector2.y, vector2.z, vector.w);
		}
	}
}
