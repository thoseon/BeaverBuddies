using System;
using System.Collections.Generic;
using Bindito.Core;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.BaseComponentSystem;

internal class ComponentCache : MonoBehaviour
{
	private ComponentCacheService _componentCacheService;

	private TypeBlacklist _typeBlacklist;

	private TypeIndexMap _typeIndexMap;

	private List<object> _components;

	private BaseComponentUpdateUnityAdapter _updateAdapter;

	private BaseComponentLateUpdateUnityAdapter _lateUpdateAdapter;

	private string _name;

	public GameObject CachedGameObject { get; private set; }

	public Transform CachedTransform { get; private set; }

	public ReadOnlyList<object> AllComponents => _components.AsReadOnlyList();

	[Inject]
	public void InjectDependencies(ComponentCacheService componentCacheService, TypeBlacklist typeBlacklist)
	{
		_componentCacheService = componentCacheService;
		_typeBlacklist = typeBlacklist;
	}

	public void Initialize(List<object> instantiatedComponents, string blueprintName, TypeIndexMap typeIndexMap)
	{
		_typeIndexMap = typeIndexMap;
		CachedGameObject = base.gameObject;
		CachedTransform = base.transform;
		_name = blueprintName;
		_components = instantiatedComponents;
		_componentCacheService.SaveComponentsCount(_name, _components.Count);
		_updateAdapter = GetComponent<BaseComponentUpdateUnityAdapter>();
		_lateUpdateAdapter = GetComponent<BaseComponentLateUpdateUnityAdapter>();
		foreach (object instantiatedComponent in instantiatedComponents)
		{
			if (instantiatedComponent is BaseComponent baseComponent)
			{
				baseComponent.Initialize(this);
			}
		}
	}

	public void SetActive()
	{
		int count = _components.Count;
		for (int i = 0; i < count; i++)
		{
			object obj = _components[i];
			if (obj is IUpdatableComponent component)
			{
				_updateAdapter.Add(component);
			}
			if (obj is ILateUpdatableComponent component2)
			{
				_lateUpdateAdapter.Add(component2);
			}
			if (obj is IAwakableComponent awakableComponent)
			{
				awakableComponent.Awake();
			}
		}
		_componentCacheService.SaveComponentsCount(_name, _components.Count);
	}

	public void AddEnabledComponent(BaseComponent baseComponent)
	{
		if (baseComponent is IUpdatableComponent component)
		{
			_updateAdapter.Add(component);
		}
		if (baseComponent is ILateUpdatableComponent component2)
		{
			_lateUpdateAdapter.Add(component2);
		}
	}

	public void RemoveDisabledComponent(BaseComponent baseComponent)
	{
		if (baseComponent is IUpdatableComponent component)
		{
			_updateAdapter.Remove(component);
		}
		if (baseComponent is ILateUpdatableComponent component2)
		{
			_lateUpdateAdapter.Remove(component2);
		}
	}

	public T GetCachedComponent<T>()
	{
		object index = GetIndex<T>();
		if (!(index is int index2))
		{
			if (index is List<int>)
			{
				throw new InvalidOperationException(GetMoreThenOneErrorMessage<T>());
			}
			return default(T);
		}
		return (T)_components[index2];
	}

	public T GetCachedEnabledComponent<T>()
	{
		object index = GetIndex<T>();
		if (!(index is int index2))
		{
			if (index is List<int> list)
			{
				int num = -1;
				for (int i = 0; i < list.Count; i++)
				{
					if (IsEnabled(_components[list[i]]))
					{
						if (num != -1)
						{
							throw new InvalidOperationException(GetMoreThenOneErrorMessage<T>(enabledComponent: true));
						}
						num = list[i];
					}
				}
				if (num == -1)
				{
					return default(T);
				}
				return (T)_components[num];
			}
			return default(T);
		}
		T val = (T)_components[index2];
		if (!IsEnabled(val))
		{
			return default(T);
		}
		return val;
	}

	public void GetCachedComponents<T>(List<T> results)
	{
		object index = GetIndex<T>();
		if (!(index is int index2))
		{
			if (index is List<int> list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					results.Add((T)_components[list[i]]);
				}
			}
		}
		else
		{
			results.Add((T)_components[index2]);
		}
	}

	public bool TryGetCachedComponent<T>(out T returnedComponent)
	{
		object index = GetIndex<T>();
		if (!(index is int index2))
		{
			if (index is List<int>)
			{
				throw new InvalidOperationException(GetMoreThenOneErrorMessage<T>());
			}
			returnedComponent = default(T);
			return false;
		}
		returnedComponent = (T)_components[index2];
		return true;
	}

	private void OnDestroy()
	{
		CachedGameObject = null;
		CachedTransform = null;
		_componentCacheService = null;
		_typeBlacklist = null;
		_typeIndexMap = null;
		_components = null;
		_updateAdapter = null;
		_lateUpdateAdapter = null;
	}

	private object GetIndex<T>()
	{
		Type typeFromHandle = typeof(T);
		if (_typeIndexMap.TryGetIndex(typeFromHandle, out var index))
		{
			return index;
		}
		_typeBlacklist.Verify(typeFromHandle);
		_typeIndexMap.CacheType<T>(_components.AsReadOnlyList());
		return _typeIndexMap.GetIndex(typeFromHandle);
	}

	private static bool IsEnabled(object obj)
	{
		if (obj is MonoBehaviour monoBehaviour)
		{
			if (monoBehaviour.enabled)
			{
				goto IL_0026;
			}
		}
		else if (obj is BaseComponent { Enabled: not false })
		{
			goto IL_0026;
		}
		return false;
		IL_0026:
		return true;
	}

	private string GetMoreThenOneErrorMessage<T>(bool enabledComponent = false)
	{
		string arg = (enabledComponent ? "enabled component" : "component");
		return $"More than one {arg} of type {typeof(T)} found in {base.name}";
	}
}
