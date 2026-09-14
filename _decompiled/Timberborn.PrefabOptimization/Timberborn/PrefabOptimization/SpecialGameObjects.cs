using UnityEngine;

namespace Timberborn.PrefabOptimization;

public class SpecialGameObjects
{
	private static readonly string RootMarker = "#";

	public static bool GameObjectIsRoot(GameObject target)
	{
		if (target.name.StartsWith(RootMarker))
		{
			return true;
		}
		if (target.TryGetComponent<MeshFilter>(out var component))
		{
			return component.sharedMesh.name.StartsWith(RootMarker);
		}
		return false;
	}
}
