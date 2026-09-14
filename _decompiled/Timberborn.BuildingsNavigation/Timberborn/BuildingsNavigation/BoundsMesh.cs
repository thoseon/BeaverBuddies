using System.Collections.Generic;
using Timberborn.PrefabOptimization;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal class BoundsMesh
{
	private readonly Dictionary<int, BoundsMeshLayer> _layers = new Dictionary<int, BoundsMeshLayer>();

	private Material _baseMaterial;

	public void Initialize(Material baseMaterial)
	{
		_baseMaterial = baseMaterial;
	}

	public void Reset()
	{
		foreach (BoundsMeshLayer value in _layers.Values)
		{
			value.Reset();
		}
	}

	public void Build()
	{
		foreach (BoundsMeshLayer value in _layers.Values)
		{
			value.Build();
		}
	}

	public void Draw()
	{
		foreach (BoundsMeshLayer value in _layers.Values)
		{
			value.Draw();
		}
	}

	public void Append(int index, IntermediateMesh mesh, TranslationTransform translation)
	{
		if (!_layers.TryGetValue(index, out var value))
		{
			value = (_layers[index] = BoundsMeshLayer.Create(_baseMaterial, index));
		}
		value.AppendMesh(mesh, translation);
	}
}
