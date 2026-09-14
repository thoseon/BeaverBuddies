using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Common;

public static class GameObjectExtensions
{
	public static IEnumerable<GameObject> GetDirectChildren(this GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		int i = 0;
		while (i < transform.childCount)
		{
			yield return transform.GetChild(i).gameObject;
			int num = i + 1;
			i = num;
		}
	}

	public static IEnumerable<GameObject> GetAllChildren(this GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		int i = 0;
		while (i < transform.childCount)
		{
			GameObject child = transform.GetChild(i).gameObject;
			yield return child;
			foreach (GameObject allChild in child.GetAllChildren())
			{
				yield return allChild;
			}
			int num = i + 1;
			i = num;
		}
	}

	public static GameObject FindChildIfNameNotEmpty(this GameObject gameObject, string childName)
	{
		if (!string.IsNullOrEmpty(childName))
		{
			return gameObject.FindChild(childName);
		}
		return null;
	}

	public static GameObject FindChild(this GameObject gameObject, string childName)
	{
		return gameObject.FindChildTransform(childName).gameObject;
	}

	public static Transform FindChildTransform(this GameObject gameObject, string childName)
	{
		if (string.IsNullOrEmpty(childName))
		{
			throw new ArgumentException("Child name cannot be empty", "childName");
		}
		Transform transform = gameObject.FindChildRecursive(childName);
		if (transform == null)
		{
			throw new NullReferenceException("Child " + childName + " not found in " + gameObject.name);
		}
		return transform;
	}

	private static Transform FindChildRecursive(this GameObject gameObject, string childName)
	{
		Transform transform = gameObject.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child.name == childName)
			{
				return child;
			}
			Transform transform2 = child.gameObject.FindChildRecursive(childName);
			if (transform2 != null)
			{
				return transform2;
			}
		}
		return null;
	}
}
