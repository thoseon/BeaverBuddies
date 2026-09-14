using Timberborn.BlockObjectModelSystem;
using Timberborn.BlueprintSystem;
using Timberborn.PrefabOptimization;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.GoodStackSystem;

public class GoodStackModelFactory : ILoadableSingleton
{
	private static readonly string GoodStackModelPath = "Environment/GoodStack/GoodStackModel.blueprint";

	private readonly OptimizedPrefabInstantiator _optimizedPrefabInstantiator;

	private readonly ISpecService _specService;

	private Blueprint _goodStackModelTemplate;

	public GoodStackModelFactory(OptimizedPrefabInstantiator optimizedPrefabInstantiator, ISpecService specService)
	{
		_optimizedPrefabInstantiator = optimizedPrefabInstantiator;
		_specService = specService;
	}

	public void Load()
	{
		_goodStackModelTemplate = _specService.GetBlueprint(GoodStackModelPath);
	}

	public void Create(GoodStack owner)
	{
		GameObject fullModel = owner.GetComponent<BlockObjectModel>().FullModel;
		GameObject gameObject = _optimizedPrefabInstantiator.InstantiateInactive(_goodStackModelTemplate, fullModel.transform);
		owner.GetComponent<EntityMaterials>().AddMaterials(gameObject);
		owner.GetComponent<GoodStackModel>().Initialize(gameObject, _goodStackModelTemplate.GetSpec<GoodStackModelSpec>());
	}
}
