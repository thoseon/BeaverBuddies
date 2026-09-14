using Timberborn.Coordinates;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ModularShafts;

internal class ModularShaftModelService : ILoadableSingleton
{
	private readonly RootObjectProvider _rootObjectProvider;

	private readonly ShaftModelFactory _shaftModelFactory;

	private Transform _root;

	private readonly ModularShaftOrientedVariants _nonStackableShafts = new ModularShaftOrientedVariants();

	private readonly ModularShaftOrientedVariants _stackableShafts = new ModularShaftOrientedVariants();

	public ModularShaftModelService(RootObjectProvider rootObjectProvider, ShaftModelFactory shaftModelFactory)
	{
		_rootObjectProvider = rootObjectProvider;
		_shaftModelFactory = shaftModelFactory;
	}

	public void Load()
	{
		_root = _rootObjectProvider.CreateRootObject("ModularShaftModelService").transform;
		BuildAllVariants();
	}

	public OrientedValue<GameObject> GetModel(ShaftVariant variant)
	{
		return _nonStackableShafts.GetMatch(variant);
	}

	public OrientedValue<GameObject> GetStackableModel(ShaftVariant variant)
	{
		return _stackableShafts.GetMatch(variant);
	}

	private void BuildAllVariants()
	{
		foreach (ShaftVariant allVariant in ShaftVariants.GetAllVariants())
		{
			if (!_nonStackableShafts.Contains(allVariant) && allVariant.Top == 0)
			{
				GameObject value = BuildModel(allVariant, isStackable: false);
				_nonStackableShafts.AddVariant(value, allVariant);
			}
			if (!_stackableShafts.Contains(allVariant))
			{
				GameObject value2 = BuildModel(allVariant, isStackable: true);
				_stackableShafts.AddVariant(value2, allVariant);
			}
		}
	}

	private GameObject BuildModel(ShaftVariant variant, bool isStackable)
	{
		GameObject gameObject = new GameObject(variant.GetName() + (isStackable ? "S" : string.Empty));
		gameObject.transform.SetParent(_root);
		if (isStackable)
		{
			_shaftModelFactory.BuildStackable(variant, gameObject);
		}
		else
		{
			_shaftModelFactory.BuildNonStackable(variant, gameObject);
		}
		gameObject.SetActive(value: false);
		return gameObject;
	}
}
