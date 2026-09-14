using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bindito.Core;
using Bindito.Unity;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.BaseComponentSystem;

public class BaseInstantiator
{
	private readonly IInstantiator _instantiator;

	private readonly IContainer _container;

	private readonly ComponentCacheService _componentCacheService;

	private readonly Dictionary<string, TypeIndexMap> _typeMaps = new Dictionary<string, TypeIndexMap>();

	internal BaseInstantiator(IInstantiator instantiator, IContainer container, ComponentCacheService componentCacheService)
	{
		_instantiator = instantiator;
		_container = container;
		_componentCacheService = componentCacheService;
	}

	public GameObject InstantiateInactive(GameObject prefab, Transform parent, Blueprint blueprint, ImmutableArray<Type> decoratedComponents, IReadOnlyList<object> initializingComponents, out List<object> instantiatedComponents)
	{
		GameObject gameObject = _instantiator.InstantiateInactive(prefab, parent, out var _);
		ComponentCache componentCache = _instantiator.AddComponent<ComponentCache>(gameObject);
		string text = (TryGetComponentsHash(initializingComponents, out var hash) ? $"{blueprint.Name}.{hash}" : blueprint.Name);
		instantiatedComponents = InstantiateComponents(blueprint, text, initializingComponents, decoratedComponents);
		instantiatedComponents.Add(componentCache);
		_instantiator.AddComponent<BaseComponentUnityAdapter>(gameObject);
		_instantiator.AddComponent<BaseComponentUpdateUnityAdapter>(gameObject);
		_instantiator.AddComponent<BaseComponentLateUpdateUnityAdapter>(gameObject);
		TypeIndexMap orAdd = _typeMaps.GetOrAdd(text);
		componentCache.Initialize(instantiatedComponents, text, orAdd);
		return gameObject;
	}

	private List<object> InstantiateComponents(Blueprint blueprint, string name, IReadOnlyList<object> initializingComponents, ImmutableArray<Type> decoratedComponents)
	{
		List<object> list = new List<object>(_componentCacheService.GetComponentsCount(name));
		if (initializingComponents != null)
		{
			foreach (object initializingComponent in initializingComponents)
			{
				list.Add(initializingComponent);
			}
		}
		foreach (Type item2 in decoratedComponents)
		{
			object item = InstantiateComponent(blueprint, item2);
			list.Add(item);
		}
		return list;
	}

	private object InstantiateComponent(Blueprint blueprint, Type type)
	{
		if (typeof(ComponentSpec).IsAssignableFrom(type))
		{
			return blueprint.GetSpec(type) ?? throw new InvalidOperationException("Blueprint " + blueprint.Name + " does not contain spec for component " + type.Name);
		}
		return _container.GetInstance(type);
	}

	private static bool TryGetComponentsHash(IReadOnlyList<object> initializingComponents, out Hash128 hash)
	{
		hash = default(Hash128);
		if (initializingComponents == null || initializingComponents.Count == 0)
		{
			return false;
		}
		foreach (object initializingComponent in initializingComponents)
		{
			hash.Append(initializingComponent.GetType().FullName);
		}
		return true;
	}
}
