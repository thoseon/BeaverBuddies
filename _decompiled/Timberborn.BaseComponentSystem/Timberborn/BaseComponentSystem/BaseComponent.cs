using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.BaseComponentSystem;

public abstract class BaseComponent
{
	private ComponentCache _componentCache;

	public bool Enabled { get; private set; } = true;

	public GameObject GameObject => _componentCache.CachedGameObject;

	public Transform Transform => _componentCache.CachedTransform;

	public ReadOnlyList<object> AllComponents => _componentCache.AllComponents;

	public string Name => _componentCache.name;

	public void EnableComponent()
	{
		if (!Enabled)
		{
			_componentCache.AddEnabledComponent(this);
			Enabled = true;
		}
	}

	public void DisableComponent()
	{
		if (Enabled)
		{
			_componentCache.RemoveDisabledComponent(this);
			Enabled = false;
		}
	}

	public T GetEnabledComponent<T>()
	{
		return _componentCache.GetCachedEnabledComponent<T>();
	}

	public T GetComponent<T>()
	{
		return _componentCache.GetCachedComponent<T>();
	}

	public bool HasComponent<T>()
	{
		return GetComponent<T>() != null;
	}

	public void GetComponents<T>(List<T> results)
	{
		_componentCache.GetCachedComponents(results);
	}

	public List<T> GetComponentsAllocating<T>()
	{
		List<T> list = new List<T>();
		_componentCache.GetCachedComponents(list);
		return list;
	}

	public bool TryGetComponent<T>(out T component)
	{
		return _componentCache.TryGetCachedComponent<T>(out component);
	}

	public T GetComponentInChildren<T>(bool includeInactive = false)
	{
		T val = (includeInactive ? GetComponent<T>() : GetEnabledComponent<T>());
		if (val == null)
		{
			return GameObject.GetComponentInChildren<T>(includeInactive);
		}
		return val;
	}

	public static implicit operator bool(BaseComponent baseComponent)
	{
		if (baseComponent == null)
		{
			return false;
		}
		return baseComponent.GameObject;
	}

	internal void Initialize(ComponentCache componentCache)
	{
		_componentCache = componentCache;
	}
}
