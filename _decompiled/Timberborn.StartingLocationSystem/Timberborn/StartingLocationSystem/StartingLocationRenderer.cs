using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.StartingLocationSystem;

public class StartingLocationRenderer : BaseComponent, IAwakableComponent
{
	private readonly List<Renderer> _renderers = new List<Renderer>();

	public void Awake()
	{
		_renderers.AddRange(base.GameObject.GetComponentsInChildren<Renderer>(includeInactive: true));
	}

	public void Show()
	{
		foreach (Renderer renderer in _renderers)
		{
			renderer.enabled = true;
		}
	}

	public void Hide()
	{
		foreach (Renderer renderer in _renderers)
		{
			renderer.enabled = false;
		}
	}
}
