using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.PrefabOptimization;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.StockpileVisualization;

public class GoodColumnVariantsService : ILoadableSingleton, IUnloadableSingleton
{
	private readonly TemplateService _templateService;

	private readonly GoodVisualizationSpecService _goodVisualizationSpecService;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly Dictionary<string, Mesh> _variants = new Dictionary<string, Mesh>();

	private readonly MeshBuilder _meshBuilder = new MeshBuilder();

	public GoodColumnVariantsService(TemplateService templateService, GoodVisualizationSpecService goodVisualizationSpecService, IRandomNumberGenerator randomNumberGenerator)
	{
		_templateService = templateService;
		_goodVisualizationSpecService = goodVisualizationSpecService;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Load()
	{
		foreach (StockpileGoodColumnVisualizerSpec item in _templateService.GetAll<StockpileGoodColumnVisualizerSpec>())
		{
			LoadVisualizerVariants(item);
		}
		_meshBuilder.Reset("");
	}

	public void Unload()
	{
		foreach (Mesh value in _variants.Values)
		{
			Object.Destroy(value);
		}
	}

	public Mesh GetVariant(StockpileGoodColumnVisualizer visualizer, int amount)
	{
		return _variants[GetKey(visualizer.GetComponent<TemplateSpec>(), visualizer.CurrentVisualization, amount)];
	}

	private void LoadVisualizerVariants(StockpileGoodColumnVisualizerSpec visualizer)
	{
		GoodVisualizationSpec visualization = _goodVisualizationSpecService.GetVisualization(visualizer.GoodVisualizationId, visualizer.GoodVisualizationVariant);
		IntermediateMesh mesh = BuildIntermediateMesh(visualization);
		TemplateSpec spec = visualizer.GetSpec<TemplateSpec>();
		BlockObjectSpec spec2 = visualizer.GetSpec<BlockObjectSpec>();
		List<Vector3> list = new List<Vector3>(GetColumnPositions(visualization, spec2));
		for (int i = 0; i < list.Count; i++)
		{
			if (i != 0)
			{
				RiseRandomColumn(list, visualization, spec2);
			}
			AddVariant(list, mesh, GetKey(spec, visualization, i));
		}
	}

	private IntermediateMesh BuildIntermediateMesh(GoodVisualizationSpec visualization)
	{
		_meshBuilder.Reset("");
		AssetRef<Mesh> primaryMesh = visualization.PrimaryMesh;
		AssetRef<Material> material = visualization.Material;
		_meshBuilder.AppendMesh(primaryMesh.Asset, new Material[1] { material.Asset }, new TranslationTransform(Vector3.zero));
		return _meshBuilder.BuildIntermediateMesh();
	}

	private static IEnumerable<Vector3> GetColumnPositions(GoodVisualizationSpec visualization, BlockObjectSpec blockObjectSpec)
	{
		IEnumerable<Vector3Int> enumerable = from coords in blockObjectSpec.GetBlocks().GetOccupiedCoordinates()
			where coords.z == blockObjectSpec.BaseZ
			select coords;
		foreach (Vector3Int baseLevelCoord in enumerable)
		{
			int x = -1;
			while (x <= 1)
			{
				int num;
				for (int y = -1; y <= 1; y = num)
				{
					Vector3 offset = visualization.Offset;
					yield return baseLevelCoord + new Vector3((float)x * offset.x, (float)y * offset.y, 0f);
					num = y + 1;
				}
				num = x + 1;
				x = num;
			}
		}
	}

	private void RiseRandomColumn(ICollection<Vector3> positions, GoodVisualizationSpec visualization, BlockObjectSpec blockObjectSpec)
	{
		IEnumerable<Vector3> source = positions.Where((Vector3 position) => Mathf.Approximately(position.z, blockObjectSpec.BaseZ));
		Vector3 enumerableElement = _randomNumberGenerator.GetEnumerableElement(source);
		positions.Remove(enumerableElement);
		positions.Add(enumerableElement + new Vector3(0f, 0f, visualization.Offset.z));
	}

	private void AddVariant(List<Vector3> positions, IntermediateMesh mesh, string key)
	{
		_meshBuilder.Reset("");
		foreach (Vector3 position in positions)
		{
			Vector3 translation = CoordinateSystem.GridToWorld(position);
			TranslationTransform transform = new TranslationTransform(translation);
			_meshBuilder.AppendIntermediateMesh(mesh, transform);
		}
		_variants.Add(key, _meshBuilder.Build().Mesh);
	}

	private static string GetKey(TemplateSpec templateSpec, GoodVisualizationSpec visualization, int amount)
	{
		return $"{templateSpec.TemplateName}{visualization.Id}{visualization.Variant}{amount}";
	}
}
