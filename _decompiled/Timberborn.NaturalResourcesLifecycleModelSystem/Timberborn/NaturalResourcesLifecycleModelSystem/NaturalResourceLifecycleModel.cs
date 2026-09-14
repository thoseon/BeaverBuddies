using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.NaturalResourcesLifecycle;
using UnityEngine;

namespace Timberborn.NaturalResourcesLifecycleModelSystem;

public class NaturalResourceLifecycleModel
{
	private readonly LivingNaturalResource _livingNaturalResource;

	private readonly DyingNaturalResource _dyingNaturalResource;

	private readonly GameObject _aliveModel;

	private readonly GameObject _dyingModel;

	private readonly GameObject _deadModel;

	private NaturalResourceLifecycleModel(LivingNaturalResource livingNaturalResource, DyingNaturalResource dyingNaturalResource, GameObject aliveModel, GameObject dyingModel, GameObject deadModel)
	{
		_livingNaturalResource = livingNaturalResource;
		_dyingNaturalResource = dyingNaturalResource;
		_aliveModel = aliveModel;
		_dyingModel = dyingModel;
		_deadModel = deadModel;
	}

	public static NaturalResourceLifecycleModel Create(BaseComponent naturalResource, GameObject modelsParent, string parentModelName)
	{
		LivingNaturalResource component = naturalResource.GetComponent<LivingNaturalResource>();
		DyingNaturalResource component2 = naturalResource.GetComponent<DyingNaturalResource>();
		GameObject model = GetModel(modelsParent, parentModelName, "#Alive");
		GameObject model2 = GetModel(modelsParent, parentModelName, "#Dying");
		GameObject model3 = GetModel(modelsParent, parentModelName, "#Dead");
		return new NaturalResourceLifecycleModel(component, component2, model, model2, model3);
	}

	public void Show()
	{
		bool isDying = _dyingNaturalResource.IsDying;
		bool isDead = _livingNaturalResource.IsDead;
		_aliveModel.SetActive(!isDead && !isDying);
		_dyingModel.SetActive(!isDead & isDying);
		if ((bool)_deadModel)
		{
			_deadModel.SetActive(isDead);
		}
	}

	public void Hide()
	{
		_aliveModel.SetActive(value: false);
		_dyingModel.SetActive(value: false);
		if ((bool)_deadModel)
		{
			_deadModel.SetActive(value: false);
		}
	}

	private static GameObject GetModel(GameObject modelsParent, string parentModelName, string modelName)
	{
		return modelsParent.GetDirectChildren().Single((GameObject gameObject) => gameObject.name == parentModelName).GetDirectChildren()
			.SingleOrDefault((GameObject gameObject) => gameObject.name == modelName);
	}
}
