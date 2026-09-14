using UnityEngine;

namespace Timberborn.PrefabOptimization;

public class DestroyEmptyChildrenPrefabOptimizer : IPrefabOptimizer
{
	public void Optimize(GameObject prefab)
	{
		VisitChildren(prefab);
	}

	private static void VisitChildren(GameObject visitee)
	{
		Transform transform = visitee.transform;
		int num = 0;
		while (num < transform.childCount)
		{
			if (!VisitGameObject(transform.GetChild(num).gameObject))
			{
				num++;
			}
		}
	}

	private static bool VisitGameObject(GameObject visitee)
	{
		VisitChildren(visitee);
		return DestroyIfEmpty(visitee);
	}

	private static bool DestroyIfEmpty(GameObject visitee)
	{
		if (!SpecialGameObjects.GameObjectIsRoot(visitee) && visitee.transform.childCount == 0 && visitee.GetComponents<Component>().Length == 1)
		{
			Object.DestroyImmediate(visitee);
			return true;
		}
		return false;
	}
}
