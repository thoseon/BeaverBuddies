using UnityEngine;

namespace Timberborn.PrefabOptimization;

public interface ITransform
{
	void MultiplyPoints(Vector3[] source, Vector3[] destination, int destinationIndex, int count);

	void MultiplyNormals(Vector3[] source, Vector3[] destination, int destinationIndex, int count);

	void MultiplyTangents(Vector4[] source, Vector4[] destination, int destinationIndex, int count);
}
