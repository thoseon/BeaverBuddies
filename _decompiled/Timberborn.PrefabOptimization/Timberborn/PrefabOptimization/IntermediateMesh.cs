using Timberborn.Common;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

public class IntermediateMesh
{
	public int VertexCount { get; set; }

	public Vector3[] Vertices { get; set; }

	public Vector3[] Normals { get; set; }

	public Vector4[] Tangents { get; set; }

	public Color32[] Colors { get; set; }

	public Vector4[] UV0 { get; set; }

	public Vector4[] UV1 { get; set; }

	public Vector4[] UV2 { get; set; }

	public (NullableKey<Material>, int[])[] Submeshes { get; set; }
}
