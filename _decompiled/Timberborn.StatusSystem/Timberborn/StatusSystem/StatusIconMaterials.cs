using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.StatusSystem;

public class StatusIconMaterials
{
	private static readonly int BaseMapProperty = Shader.PropertyToID("_BaseMap");

	private readonly Dictionary<Sprite, Material> _materials = new Dictionary<Sprite, Material>();

	public void SetMaterial(MeshRenderer renderer, Sprite sprite)
	{
		if (!_materials.ContainsKey(sprite))
		{
			Material material = new Material(renderer.sharedMaterial);
			material.SetTexture(BaseMapProperty, sprite.texture);
			_materials[sprite] = material;
		}
		renderer.sharedMaterial = _materials[sprite];
	}
}
