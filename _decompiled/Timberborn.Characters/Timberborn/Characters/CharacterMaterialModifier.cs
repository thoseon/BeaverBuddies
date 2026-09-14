using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.Characters;

public class CharacterMaterialModifier : BaseComponent, IAwakableComponent, IDeletableEntity
{
	private Renderer _meshRenderer;

	public void Awake()
	{
		_meshRenderer = GetComponentInChildren<Renderer>(includeInactive: true);
	}

	public void DeleteEntity()
	{
		Object.Destroy(_meshRenderer.material);
	}

	public void SetColor(int propertyId, Color color)
	{
		_meshRenderer.material.SetColor(propertyId, color);
	}

	public void SetFloat(int propertyId, float value)
	{
		_meshRenderer.material.SetFloat(propertyId, value);
	}

	public void SetTexture(int propertyId, Texture texture)
	{
		_meshRenderer.material.SetTexture(propertyId, texture);
	}
}
