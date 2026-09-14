using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.DecalSystem;

internal class DecalCategory
{
	private readonly Dictionary<string, DecalSpec> _categorySpecs = new Dictionary<string, DecalSpec>();

	public IEnumerable<DecalSpec> CategorySpecs => _categorySpecs.Values;

	public DecalCategory(IEnumerable<DecalSpec> categorySpecs)
	{
		foreach (DecalSpec categorySpec in categorySpecs)
		{
			TryAdd(categorySpec);
		}
	}

	public bool TryAdd(DecalSpec decalSpec)
	{
		return _categorySpecs.TryAdd(decalSpec.Id, decalSpec);
	}

	public bool TryGet(string decalId, out DecalSpec decalSpec)
	{
		return _categorySpecs.TryGetValue(decalId, out decalSpec);
	}

	public void Remove(string decalId)
	{
		_categorySpecs.Remove(decalId);
	}

	public Texture2D GetDecalTexture(Decal decal)
	{
		return _categorySpecs[decal.Id].Texture.Asset;
	}
}
