using UnityEngine;

namespace Timberborn.PrefabOptimization;

public readonly struct BuiltMesh
{
	public Mesh Mesh { get; }

	public Material[] Materials { get; }

	public BuiltMesh(Mesh mesh, Material[] materials)
	{
		Mesh = mesh;
		Materials = materials;
	}
}
