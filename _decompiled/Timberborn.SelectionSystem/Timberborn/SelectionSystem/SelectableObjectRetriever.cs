using System;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.SelectionSystem;

public class SelectableObjectRetriever
{
	public SelectableObject GetSelectableObject(BaseComponent target)
	{
		if (TryGetSelectableObject(target.GameObject, out var selectableObject))
		{
			return selectableObject;
		}
		throw new Exception("SelectableObject component not found on object " + target.GameObject.name + "!");
	}

	public bool TryGetSelectableObject(GameObject gameObject, out SelectableObject selectableObject)
	{
		selectableObject = gameObject.GetComponentInParentSlow<SelectableObject>();
		return selectableObject != null;
	}
}
