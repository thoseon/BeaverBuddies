using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.BaseComponentSystem;

internal class BaseComponentUpdateUnityAdapter : MonoBehaviour
{
	private readonly List<IUpdatableComponent> _updatableComponents = new List<IUpdatableComponent>();

	public void Start()
	{
		base.enabled = _updatableComponents.Count > 0;
	}

	public void Add(IUpdatableComponent component)
	{
		_updatableComponents.Add(component);
		base.enabled = true;
	}

	public void Remove(IUpdatableComponent component)
	{
		_updatableComponents.Remove(component);
		if (_updatableComponents.Count == 0)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		for (int i = 0; i < _updatableComponents.Count; i++)
		{
			_updatableComponents[i].Update();
		}
	}
}
