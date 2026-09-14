using UnityEngine;

namespace Timberborn.BaseComponentSystem;

public static class GameObjectExtensions
{
	public static T GetComponentSlow<T>(this GameObject instance)
	{
		ComponentCache component = instance.GetComponent<ComponentCache>();
		if ((object)component != null)
		{
			return component.GetCachedComponent<T>();
		}
		return default(T);
	}

	public static T GetComponentInParentSlow<T>(this GameObject instance)
	{
		Transform transform = instance.transform;
		while ((bool)transform)
		{
			ComponentCache component = transform.GetComponent<ComponentCache>();
			if ((object)component != null)
			{
				T cachedComponent = component.GetCachedComponent<T>();
				if (cachedComponent != null)
				{
					return cachedComponent;
				}
			}
			transform = transform.parent;
		}
		return default(T);
	}
}
