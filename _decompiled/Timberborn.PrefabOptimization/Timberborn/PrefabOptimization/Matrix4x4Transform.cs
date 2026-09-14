using UnityEngine;

namespace Timberborn.PrefabOptimization;

public struct Matrix4x4Transform(Matrix4x4 matrix) : ITransform
{
	private Matrix4x4 _matrix = matrix;

	private Matrix4x4 _normalMatrix = matrix.inverse.transpose;

	public void MultiplyPoints(Vector3[] source, Vector3[] destination, int destinationIndex, int count)
	{
		for (int i = 0; i < count; i++)
		{
			destination[destinationIndex + i] = _matrix.MultiplyPoint(source[i]);
		}
	}

	public void MultiplyNormals(Vector3[] source, Vector3[] destination, int destinationIndex, int count)
	{
		for (int i = 0; i < count; i++)
		{
			destination[destinationIndex + i] = _normalMatrix.MultiplyVector(source[i]).normalized;
		}
	}

	public void MultiplyTangents(Vector4[] source, Vector4[] destination, int destinationIndex, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Vector4 vector = source[i];
			Vector3 normalized = _normalMatrix.MultiplyVector(vector).normalized;
			destination[destinationIndex + i] = new Vector4(normalized.x, normalized.y, normalized.z, vector.w);
		}
	}
}
