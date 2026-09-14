using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timberborn.SlotSystem;

public class SlotRetriever
{
	public IEnumerable<Transform> GetSlots(GameObject gameObject, string keyword)
	{
		return from transform in GetTransformsInChildren(gameObject)
			where IsSlot(keyword, transform)
			select transform;
	}

	public (Transform start, Transform end) GetStartAndEnd(GameObject gameObject)
	{
		Transform item = GetTransformsInChildren(gameObject).SingleOrDefault(IsStart);
		Transform item2 = GetTransformsInChildren(gameObject).SingleOrDefault(IsEnd);
		return (start: item, end: item2);
	}

	private static bool IsSlot(string keyword, Transform transform)
	{
		return transform.name.StartsWith("#Slot#" + keyword);
	}

	private static bool IsStart(Transform transform)
	{
		return transform.name.StartsWith("#MiscStart");
	}

	private static bool IsEnd(Transform transform)
	{
		return transform.name.StartsWith("#MiscEnd");
	}

	private static IEnumerable<Transform> GetTransformsInChildren(GameObject gameObject)
	{
		return gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
	}
}
