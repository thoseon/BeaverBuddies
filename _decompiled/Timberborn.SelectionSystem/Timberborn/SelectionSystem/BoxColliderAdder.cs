using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.SelectionSystem;

internal class BoxColliderAdder : BaseComponent, IInitializableEntity
{
	public void InitializeEntity()
	{
		AddCollider();
	}

	private void AddCollider()
	{
		BoxColliderAdderSpec component = GetComponent<BoxColliderAdderSpec>();
		BoxCollider boxCollider = base.GameObject.FindChild(component.TargetName).gameObject.AddComponent<BoxCollider>();
		boxCollider.center = component.Center;
		boxCollider.size = component.Size;
	}
}
