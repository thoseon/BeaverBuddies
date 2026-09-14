using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace Timberborn.TemplateInstantiation;

public class CachedTemplate
{
	public GameObject Prefab { get; }

	public ImmutableArray<CachedTemplateInitializer> Initializers { get; }

	public ImmutableArray<Type> Components { get; }

	public CachedTemplate(GameObject prefab, IEnumerable<CachedTemplateInitializer> initializers, List<Type> components)
	{
		Prefab = prefab;
		Initializers = initializers.ToImmutableArray();
		Components = components.ToImmutableArray();
	}
}
