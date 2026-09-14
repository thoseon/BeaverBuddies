using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.PrefabOptimization;
using UnityEngine;

namespace Timberborn.TerrainSystemRendering;

public class SurfaceBlockCollection
{
	private readonly Dictionary<int, ImmutableArray<IntermediateMesh>> _variations;

	public SurfaceBlockCollection(Dictionary<SurfaceBlockShape, List<IntermediateMesh>> variations)
	{
		_variations = new Dictionary<int, ImmutableArray<IntermediateMesh>>();
		foreach (KeyValuePair<SurfaceBlockShape, List<IntermediateMesh>> variation in variations)
		{
			_variations[variation.Key.Index] = variation.Value.ToImmutableArray();
		}
	}

	public ImmutableArray<IntermediateMesh> GetVariations(SurfaceBlockShape shape)
	{
		ImmutableArray<IntermediateMesh> immutableArray = _variations[shape.Index];
		if (immutableArray == null)
		{
			Debug.LogWarning($"Couldn't find a surface block of shape {shape}.");
			return ImmutableArray<IntermediateMesh>.Empty;
		}
		return immutableArray;
	}
}
