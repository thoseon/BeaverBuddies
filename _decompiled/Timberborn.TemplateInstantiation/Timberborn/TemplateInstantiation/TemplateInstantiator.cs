using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.PrefabOptimization;
using UnityEngine;

namespace Timberborn.TemplateInstantiation;

public class TemplateInstantiator
{
	private static readonly string CacheContainerName = "TemplateCache";

	private readonly BaseInstantiator _baseInstantiator;

	private readonly OptimizedPrefabInstantiator _optimizedPrefabInstantiator;

	private readonly FrozenDictionary<Type, ImmutableArray<DecoratorDefinition>> _decorators;

	private readonly Dictionary<Blueprint, CachedTemplate> _cache = new Dictionary<Blueprint, CachedTemplate>();

	private readonly Lazy<GameObject> _cacheContainer = new Lazy<GameObject>(CreateCacheContainer);

	private readonly List<Type> _temporaryTypeCache = new List<Type>();

	public TemplateInstantiator(BaseInstantiator baseInstantiator, OptimizedPrefabInstantiator optimizedPrefabInstantiator, IEnumerable<KeyValuePair<Type, IEnumerable<DecoratorDefinition>>> decorators)
	{
		_baseInstantiator = baseInstantiator;
		_optimizedPrefabInstantiator = optimizedPrefabInstantiator;
		_decorators = decorators.ToFrozenDictionary((KeyValuePair<Type, IEnumerable<DecoratorDefinition>> pair) => pair.Key, (KeyValuePair<Type, IEnumerable<DecoratorDefinition>> pair) => pair.Value.ToImmutableArray());
	}

	public void CacheInstance(Blueprint blueprint)
	{
		GetCachedTemplate(blueprint);
	}

	public GameObject Instantiate(Blueprint blueprint, Transform parent, IReadOnlyList<object> initializingComponents = null)
	{
		CachedTemplate cachedTemplate = GetCachedTemplate(blueprint);
		GameObject gameObject = _baseInstantiator.InstantiateInactive(cachedTemplate.Prefab, parent, blueprint, cachedTemplate.Components, initializingComponents, out var instantiatedComponents);
		ImmutableArray<CachedTemplateInitializer> initializers = cachedTemplate.Initializers;
		int num = initializingComponents?.Count ?? 0;
		for (int i = 0; i < initializers.Length; i++)
		{
			CachedTemplateInitializer cachedTemplateInitializer = initializers[i];
			cachedTemplateInitializer.Method(instantiatedComponents[cachedTemplateInitializer.SubjectIndex + num], instantiatedComponents[cachedTemplateInitializer.DecoratorIndex + num]);
		}
		gameObject.SetActive(value: true);
		return gameObject;
	}

	private static GameObject CreateCacheContainer()
	{
		GameObject gameObject = new GameObject(CacheContainerName);
		gameObject.SetActive(value: false);
		return gameObject;
	}

	private CachedTemplate GetCachedTemplate(Blueprint blueprint)
	{
		if (!_cache.ContainsKey(blueprint))
		{
			GameObject gameObject = _optimizedPrefabInstantiator.InstantiateInactive(blueprint, _cacheContainer.Value.transform);
			gameObject.name = blueprint.Name;
			GetInstanceComponents(blueprint, out var initializers, out var components);
			_cache.Add(blueprint, new CachedTemplate(gameObject, initializers, components));
		}
		return _cache[blueprint];
	}

	private void GetInstanceComponents(Blueprint blueprint, out List<CachedTemplateInitializer> initializers, out List<Type> components)
	{
		initializers = new List<CachedTemplateInitializer>();
		components = blueprint.Specs.Select((ComponentSpec spec) => spec.GetType()).ToList();
		_temporaryTypeCache.AddRange(components);
		for (int num = 0; num < _temporaryTypeCache.Count; num++)
		{
			Type subjectType = _temporaryTypeCache[num];
			foreach (DecoratorDefinition item in DecorateTypeAndInterfaces(subjectType))
			{
				Type decoratorType = GetDecoratorType(components, item);
				if (decoratorType != null)
				{
					components.Add(decoratorType);
					_temporaryTypeCache.Add(decoratorType);
					if (item.Dedicated)
					{
						initializers.Add(new CachedTemplateInitializer(item.Initializer, num, _temporaryTypeCache.Count - 1));
					}
				}
			}
		}
		_temporaryTypeCache.Clear();
	}

	private static Type GetDecoratorType(List<Type> components, DecoratorDefinition decorator)
	{
		Type decoratorType = decorator.DecoratorType;
		if (decorator.Dedicated || !components.Contains(decoratorType))
		{
			return decoratorType;
		}
		return null;
	}

	private IEnumerable<DecoratorDefinition> DecorateTypeAndInterfaces(Type subjectType)
	{
		Type[] interfaces = subjectType.GetInterfaces();
		return Enumerables.One(subjectType).Concat(interfaces).SelectMany(DecorateComponentType);
	}

	private IEnumerable<DecoratorDefinition> DecorateComponentType(Type decoratedType)
	{
		if (_decorators.TryGetValue(decoratedType, out var value))
		{
			foreach (DecoratorDefinition item in value)
			{
				yield return item;
			}
		}
	}
}
