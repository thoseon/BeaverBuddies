using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.PrefabOptimization;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Ruins;

public class RuinModelFactory : ILoadableSingleton
{
	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly IPrefabOptimizationChain _prefabOptimizationChain;

	private readonly ISpecService _specService;

	private readonly MeshBuilder _meshBuilder = new MeshBuilder();

	private RuinModelFactorySpec _ruinModelFactorySpec;

	public RuinModelFactory(IRandomNumberGenerator randomNumberGenerator, IPrefabOptimizationChain prefabOptimizationChain, ISpecService specService)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_prefabOptimizationChain = prefabOptimizationChain;
		_specService = specService;
	}

	public void Load()
	{
		_ruinModelFactorySpec = _specService.GetSingleSpec<RuinModelFactorySpec>();
	}

	public void CreateModels(string variantId, Ruin ruin)
	{
		ImmutableArray<RuinModelVariantSpec> ruinModelVariants = _ruinModelFactorySpec.RuinModelVariants;
		RuinModelVariantSpec modelVariantSpec = ruinModelVariants.SingleOrDefault((RuinModelVariantSpec variant) => variant.Id == variantId) ?? _randomNumberGenerator.GetListElementOrDefault(ruinModelVariants);
		CreateModels(modelVariantSpec, ruin);
	}

	private void CreateModels(RuinModelVariantSpec modelVariantSpec, Ruin ruin)
	{
		ruin.GetComponent<RuinModels>().Initialize(wetModel: CreateWetModel(modelVariantSpec, ruin), dryModel: CreateDryModel(modelVariantSpec, ruin), variantId: modelVariantSpec.Id);
		_meshBuilder.Reset("");
	}

	private GameObject CreateWetModel(RuinModelVariantSpec modelVariantSpec, Ruin ruin)
	{
		GameObject gameObject = CreateModel("RuinModelWet", ruin.SpecifiedHeight, modelVariantSpec.Model.Asset, _ruinModelFactorySpec.IvyWetModel.Asset);
		gameObject.transform.SetParent(ruin.ModelParent, worldPositionStays: false);
		return gameObject;
	}

	private GameObject CreateDryModel(RuinModelVariantSpec modelVariantSpec, Ruin ruin)
	{
		GameObject gameObject = CreateModel("RuinModelDry", ruin.SpecifiedHeight, modelVariantSpec.Model.Asset, _ruinModelFactorySpec.IvyDryModel.Asset);
		gameObject.transform.SetParent(ruin.ModelParent, worldPositionStays: false);
		return gameObject;
	}

	private GameObject CreateModel(string ruinName, int height, GameObject ruin, GameObject ivy)
	{
		_meshBuilder.Reset(ruinName);
		GameObject gameObject = new GameObject(ruinName);
		TranslationTransform transform = default(TranslationTransform);
		_meshBuilder.AppendMesh(GetMesh(ruin, height), GetMaterials(ruin, height), transform);
		_meshBuilder.AppendMesh(GetMesh(ivy, height), GetMaterials(ivy, height), transform);
		_meshBuilder.AppendMesh(GetMesh(ivy, 0), GetMaterials(ivy, 0), transform);
		BuiltMesh builtMesh = _meshBuilder.Build();
		gameObject.AddComponent<MeshFilter>().sharedMesh = builtMesh.Mesh;
		gameObject.AddComponent<MeshRenderer>().sharedMaterials = builtMesh.Materials;
		gameObject.SetActive(value: false);
		return gameObject;
	}

	private Material[] GetMaterials(GameObject model, int i)
	{
		return _prefabOptimizationChain.Process(model).GetComponentsInChildren<MeshRenderer>().Single((MeshRenderer mr) => IsOfHeight(mr, i))
			.sharedMaterials;
	}

	private Mesh GetMesh(GameObject model, int i)
	{
		return _prefabOptimizationChain.Process(model).GetComponentsInChildren<MeshFilter>().Single((MeshFilter mf) => IsOfHeight(mf, i))
			.sharedMesh;
	}

	private static bool IsOfHeight(Component component, int i)
	{
		return component.name.Contains($"{i}");
	}
}
