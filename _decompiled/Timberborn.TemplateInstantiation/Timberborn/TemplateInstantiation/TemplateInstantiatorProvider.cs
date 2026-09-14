using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bindito.Core;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.PrefabOptimization;

namespace Timberborn.TemplateInstantiation;

public class TemplateInstantiatorProvider : IProvider<TemplateInstantiator>
{
	private readonly BaseInstantiator _baseInstantiator;

	private readonly OptimizedPrefabInstantiator _optimizedPrefabInstantiator;

	private readonly ImmutableArray<TemplateModule> _templateModules;

	public TemplateInstantiatorProvider(BaseInstantiator baseInstantiator, OptimizedPrefabInstantiator optimizedPrefabInstantiator, IEnumerable<TemplateModule> templateModules)
	{
		_baseInstantiator = baseInstantiator;
		_optimizedPrefabInstantiator = optimizedPrefabInstantiator;
		_templateModules = templateModules.ToImmutableArray();
	}

	public TemplateInstantiator Get()
	{
		IEnumerable<KeyValuePair<Type, IEnumerable<DecoratorDefinition>>> decorators = from pair in GetDecoratorsFromAllModules()
			select new KeyValuePair<Type, IEnumerable<DecoratorDefinition>>(pair.Key, pair.Value);
		return new TemplateInstantiator(_baseInstantiator, _optimizedPrefabInstantiator, decorators);
	}

	private Dictionary<Type, List<DecoratorDefinition>> GetDecoratorsFromAllModules()
	{
		Dictionary<Type, List<DecoratorDefinition>> dictionary = new Dictionary<Type, List<DecoratorDefinition>>();
		foreach (TemplateModule templateModule in _templateModules)
		{
			foreach (KeyValuePair<Type, ImmutableArray<DecoratorDefinition>> decorator in templateModule.Decorators)
			{
				dictionary.GetOrAdd(decorator.Key).AddRange(decorator.Value);
			}
		}
		return dictionary;
	}
}
