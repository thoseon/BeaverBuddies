using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Rendering;

public class MaterialLightingRenderers : BaseComponent, IAwakableComponent
{
	private readonly List<MeshRenderer> _renderers = new List<MeshRenderer>();

	private readonly List<MeshRenderer> _disabledRenderers = new List<MeshRenderer>();

	public ReadOnlyList<MeshRenderer> Renderers => _renderers.AsReadOnlyList();

	public void Awake()
	{
		CollectRenderers();
	}

	public void CollectRenderers()
	{
		_renderers.Clear();
		base.GameObject.GetComponentsInChildren(includeInactive: true, _renderers);
		foreach (MeshRenderer disabledRenderer in _disabledRenderers)
		{
			_renderers.Remove(disabledRenderer);
		}
	}

	internal void DisableRendering(MeshRenderer renderer)
	{
		_disabledRenderers.Add(renderer);
		_renderers.Remove(renderer);
	}
}
