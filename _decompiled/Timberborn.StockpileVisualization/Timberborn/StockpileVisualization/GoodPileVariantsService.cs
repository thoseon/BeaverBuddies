using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.PrefabOptimization;
using Timberborn.SingletonSystem;
using Timberborn.Stockpiles;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.StockpileVisualization;

internal class GoodPileVariantsService : ILoadableSingleton, IUnloadableSingleton
{
	private class Pile
	{
		public int MaxLevels { get; }

		public bool Rotated { get; }

		public Vector3 Position { get; }

		public int MaxItemsPerLevel { get; }

		public int Items { get; private set; }

		public bool IsNotFull => Items < MaxItemsPerLevel;

		public Pile(int maxLevels, bool rotated, Vector3 position, int maxItemsPerLevel)
		{
			MaxLevels = maxLevels;
			Rotated = rotated;
			Position = position;
			MaxItemsPerLevel = maxItemsPerLevel;
		}

		public void AddItem()
		{
			Items++;
		}
	}

	private static readonly int IndividualItemsLevel = -1;

	private readonly TemplateService _templateService;

	private readonly GoodVisualizationSpecService _goodVisualizationSpecService;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly Dictionary<string, Mesh> _variants = new Dictionary<string, Mesh>();

	private readonly Dictionary<string, Mesh> _rotatedVariants = new Dictionary<string, Mesh>();

	private readonly MeshBuilder _meshBuilder = new MeshBuilder();

	private IntermediateMesh _primaryIntermediateMesh;

	private IntermediateMesh _secondaryIntermediateMesh;

	public GoodPileVariantsService(TemplateService templateService, GoodVisualizationSpecService goodVisualizationSpecService, IRandomNumberGenerator randomNumberGenerator)
	{
		_templateService = templateService;
		_goodVisualizationSpecService = goodVisualizationSpecService;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Load()
	{
		foreach (StockpileGoodPileVisualizerSpec item in _templateService.GetAll<StockpileGoodPileVisualizerSpec>())
		{
			LoadVisualizerVariants(item);
		}
		_meshBuilder.Reset("");
		_primaryIntermediateMesh = null;
		_secondaryIntermediateMesh = null;
	}

	public void Unload()
	{
		foreach (Mesh value in _variants.Values)
		{
			Object.Destroy(value);
		}
		foreach (Mesh value2 in _rotatedVariants.Values)
		{
			Object.Destroy(value2);
		}
	}

	public Mesh GetVariant(StockpileGoodPileVisualizer visualizer, int amount, bool rotated)
	{
		string key = GetKey(visualizer.GetComponent<TemplateSpec>(), visualizer.CurrentVisualization, amount);
		if (!rotated)
		{
			return _variants[key];
		}
		return _rotatedVariants[key];
	}

	private void LoadVisualizerVariants(StockpileGoodPileVisualizerSpec visualizer)
	{
		TemplateSpec spec = visualizer.GetSpec<TemplateSpec>();
		foreach (string goodPileVisualization in visualizer.GoodPileVisualizations)
		{
			GoodVisualizationSpec visualization = _goodVisualizationSpecService.GetVisualization(goodPileVisualization);
			BuildIntermediateMeshes(visualization);
			List<Pile> list = new List<Pile>(GetPiles(visualizer, visualization));
			float num = (float)list.Count * visualization.LimitingAmount;
			for (int i = 0; (float)i <= num; i++)
			{
				if (i != 0)
				{
					GetRandomPile(list).AddItem();
				}
				string key = GetKey(spec, visualization, i);
				_variants.Add(key, GetMesh(list, visualization, rotated: false));
				_rotatedVariants.Add(key, GetMesh(list, visualization, rotated: true));
			}
		}
	}

	private void BuildIntermediateMeshes(GoodVisualizationSpec visualization)
	{
		Material asset = visualization.Material.Asset;
		_primaryIntermediateMesh = BuildIntermediateMesh(visualization.PrimaryMesh.Asset, asset);
		_secondaryIntermediateMesh = BuildIntermediateMesh(visualization.SecondaryMesh.Asset, asset);
	}

	private IntermediateMesh BuildIntermediateMesh(Mesh mesh, Material material)
	{
		_meshBuilder.Reset("");
		_meshBuilder.AppendMesh(mesh, new Material[1] { material }, new TranslationTransform(Vector3.zero));
		return _meshBuilder.BuildIntermediateMesh();
	}

	private IEnumerable<Pile> GetPiles(StockpileGoodPileVisualizerSpec visualizer, GoodVisualizationSpec visualization)
	{
		BlockObjectSpec blockObjectSpec = visualizer.GetSpec<BlockObjectSpec>();
		List<Vector3Int> list = (from coords in blockObjectSpec.GetBlocks().GetOccupiedCoordinates()
			where coords.z == blockObjectSpec.BaseZ
			select coords).ToList();
		int num = Mathf.CeilToInt((float)visualizer.GetSpec<StockpileSpec>().MaxCapacity / (float)list.Count);
		float limitingAmount = visualization.LimitingAmount;
		int numberOfLevels = Mathf.CeilToInt((float)num / limitingAmount);
		foreach (Vector3Int item in list)
		{
			bool rotated = _randomNumberGenerator.CheckProbability(0.5f);
			yield return new Pile(numberOfLevels, rotated, item, visualization.LimitingAmountFlooredToInt);
		}
	}

	private Pile GetRandomPile(IEnumerable<Pile> piles)
	{
		IEnumerable<Pile> source = piles.Where((Pile pile) => pile.IsNotFull);
		return _randomNumberGenerator.GetEnumerableElement(source);
	}

	private Mesh GetMesh(List<Pile> piles, GoodVisualizationSpec visualization, bool rotated)
	{
		_meshBuilder.Reset("");
		foreach (Pile pile in piles)
		{
			AddFullLevels(visualization, pile, rotated);
			if (pile.IsNotFull)
			{
				AddIndividualItems(visualization, pile, rotated);
			}
			else
			{
				AddFullLevel(visualization, pile, rotated, IndividualItemsLevel);
			}
		}
		return _meshBuilder.Build().Mesh;
	}

	private void AddFullLevels(GoodVisualizationSpec visualization, Pile pile, bool rotatedVariant)
	{
		for (int i = 0; i < pile.MaxLevels; i++)
		{
			AddFullLevel(visualization, pile, rotatedVariant, i);
		}
	}

	private void AddFullLevel(GoodVisualizationSpec visualization, Pile pile, bool rotatedVariant, int index)
	{
		Vector3 position = CalculatePosition(pile.Position, visualization.Offset, index);
		bool rotated = (rotatedVariant ? (!pile.Rotated) : pile.Rotated);
		bool rotate = ShouldRotate(index, rotated);
		_meshBuilder.AppendIntermediateMesh(_secondaryIntermediateMesh, GetTransform(position, rotate));
	}

	private void AddIndividualItems(GoodVisualizationSpec visualization, Pile pile, bool rotatedVariant)
	{
		Vector3 offset = visualization.Offset;
		Vector3 vector = CalculatePosition(pile.Position, offset, IndividualItemsLevel);
		float num = (float)pile.MaxItemsPerLevel * offset.x / 2f - offset.x / 2f;
		bool rotated = (rotatedVariant ? (!pile.Rotated) : pile.Rotated);
		bool flag = ShouldRotate(pile.MaxLevels, rotated);
		Vector3 vector2 = (flag ? Vector3.right : Vector3.forward);
		for (int i = 0; i < pile.Items; i++)
		{
			ITransform transform = GetTransform(vector - vector2 * (num - offset.x * (float)i), flag);
			_meshBuilder.AppendIntermediateMesh(_primaryIntermediateMesh, transform);
		}
	}

	private static ITransform GetTransform(Vector3 position, bool rotate)
	{
		if (rotate)
		{
			Quaternion q = Quaternion.AngleAxis(90f, Vector3.up);
			return new Matrix4x4Transform(Matrix4x4.TRS(position, q, Vector3.one));
		}
		return new TranslationTransform(position);
	}

	private static Vector3 CalculatePosition(Vector3 position, Vector3 offset, int level)
	{
		return CoordinateSystem.GridToWorld(position) - Vector3.up * (offset.z * (float)level);
	}

	private static bool ShouldRotate(int visibleLevels, bool rotated)
	{
		int num = ((visibleLevels % 2 != 0) ? 90 : 0);
		return (rotated ? 90 : 0) + num == 90;
	}

	private static string GetKey(TemplateSpec templateSpec, GoodVisualizationSpec visualization, int amount)
	{
		return $"{templateSpec.TemplateName}{visualization.Id}{visualization.Variant}{amount}";
	}
}
