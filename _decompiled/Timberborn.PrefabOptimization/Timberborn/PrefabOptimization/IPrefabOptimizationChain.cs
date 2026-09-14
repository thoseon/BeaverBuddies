using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

public interface IPrefabOptimizationChain
{
	GameObject Process(GameObject inputPrefab);

	ImmutableArray<GameObject> GetCached();

	GameObject Process(Blueprint inputBlueprint);
}
