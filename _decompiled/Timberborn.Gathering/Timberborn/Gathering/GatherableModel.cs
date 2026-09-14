using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.Gathering;

internal class GatherableModel : BaseComponent, IInitializableEntity
{
	private static readonly int EnableDetailId = Shader.PropertyToID("_EnableDetail");

	private readonly List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();

	public void InitializeEntity()
	{
		base.GameObject.FindChild("Mature").GetComponentsInChildren(includeInactive: true, _meshRenderers);
	}

	public void UpdateMaterial(bool showYield)
	{
		foreach (MeshRenderer meshRenderer in _meshRenderers)
		{
			meshRenderer.material.SetFloat(EnableDetailId, showYield ? 1f : 0f);
		}
	}
}
