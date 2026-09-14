using System;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

public readonly struct TranslationTransform(Vector3 translation) : ITransform
{
	private readonly Vector3 _translation = translation;

	public void MultiplyPoints(Vector3[] source, Vector3[] destination, int destinationIndex, int count)
	{
		for (int i = 0; i < count; i++)
		{
			destination[destinationIndex + i] = source[i] + _translation;
		}
	}

	public void MultiplyNormals(Vector3[] source, Vector3[] destination, int destinationIndex, int count)
	{
		Array.Copy(source, 0, destination, destinationIndex, count);
	}

	public void MultiplyTangents(Vector4[] source, Vector4[] destination, int destinationIndex, int count)
	{
		Array.Copy(source, 0, destination, destinationIndex, count);
	}
}
