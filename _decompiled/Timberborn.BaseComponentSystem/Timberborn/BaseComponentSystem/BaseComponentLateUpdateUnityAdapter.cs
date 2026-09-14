using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.BaseComponentSystem;

internal class BaseComponentLateUpdateUnityAdapter : MonoBehaviour
{
	private readonly List<ILateUpdatableComponent> _lateUpdatableComponents = new List<ILateUpdatableComponent>();

	public void Start()
	{
		base.enabled = _lateUpdatableComponents.Count > 0;
	}

	public void Add(ILateUpdatableComponent component)
	{
		_lateUpdatableComponents.Add(component);
		base.enabled = true;
	}

	public void Remove(ILateUpdatableComponent component)
	{
		_lateUpdatableComponents.Remove(component);
		if (_lateUpdatableComponents.Count == 0)
		{
			base.enabled = false;
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < _lateUpdatableComponents.Count; i++)
		{
			_lateUpdatableComponents[i].LateUpdate();
		}
	}
}
