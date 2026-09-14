using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.Rendering;

public class EntityMaterials : BaseComponent, IAwakableComponent, IDeletableEntity
{
	private readonly struct ChildMaterial
	{
		public Transform Child { get; }

		public Material Material { get; }

		public ChildMaterial(Transform child, Material material)
		{
			Child = child;
			Material = material;
		}
	}

	private readonly List<ChildMaterial> _childMaterials = new List<ChildMaterial>();

	public void Awake()
	{
		AddMaterials(base.GameObject);
	}

	public void DeleteEntity()
	{
		foreach (ChildMaterial childMaterial in _childMaterials)
		{
			Object.Destroy(childMaterial.Material);
		}
	}

	public void AddMaterials(GameObject owner)
	{
		MeshRenderer[] componentsInChildren = owner.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			Material[] materials = meshRenderer.materials;
			foreach (Material material in materials)
			{
				AddMaterial(meshRenderer.transform, material);
			}
		}
	}

	public void AddMaterial(Transform owner, Material material)
	{
		_childMaterials.Add(new ChildMaterial(owner, material));
	}

	public void DestroyMaterial(Material material)
	{
		for (int num = _childMaterials.Count - 1; num >= 0; num--)
		{
			if (_childMaterials[num].Material == material)
			{
				_childMaterials.RemoveAt(num);
			}
		}
		Object.Destroy(material);
	}

	public void GetChildMaterials(Transform parent, List<Material> childMaterials)
	{
		for (int i = 0; i < _childMaterials.Count; i++)
		{
			ChildMaterial childMaterial = _childMaterials[i];
			if (childMaterial.Child.IsChildOf(parent))
			{
				childMaterials.Add(childMaterial.Material);
			}
		}
	}
}
