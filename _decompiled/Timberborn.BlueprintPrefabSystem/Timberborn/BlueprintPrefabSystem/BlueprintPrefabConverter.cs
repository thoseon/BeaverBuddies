using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlueprintPrefabSystem;

public class BlueprintPrefabConverter
{
	private readonly ImmutableArray<ISpecToPrefabConverter> _specToPrefabConverters;

	public BlueprintPrefabConverter(IEnumerable<ISpecToPrefabConverter> specToPrefabConverters)
	{
		_specToPrefabConverters = specToPrefabConverters.ToImmutableArray();
	}

	public GameObject Convert(Blueprint blueprint, Transform parent)
	{
		GameObject gameObject = new GameObject(blueprint.Name);
		gameObject.transform.SetParent(parent.transform);
		foreach (ComponentSpec spec in blueprint.Specs)
		{
			foreach (ISpecToPrefabConverter specToPrefabConverter in _specToPrefabConverters)
			{
				if (specToPrefabConverter.CanConvert(spec))
				{
					specToPrefabConverter.Convert(gameObject, spec);
				}
			}
		}
		foreach (Blueprint child in blueprint.Children)
		{
			Convert(child, gameObject.transform);
		}
		return gameObject;
	}
}
