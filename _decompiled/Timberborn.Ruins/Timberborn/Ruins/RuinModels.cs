using Timberborn.BaseComponentSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Ruins;

internal class RuinModels : BaseComponent, IPersistentEntity
{
	private static readonly ComponentKey RuinModelsKey = new ComponentKey("RuinModels");

	private static readonly PropertyKey<string> VariantIdKey = new PropertyKey<string>("VariantId");

	private GameObject _wetModel;

	private GameObject _dryModel;

	public string VariantId { get; private set; }

	public bool IsInitialized
	{
		get
		{
			if ((bool)_wetModel)
			{
				return _dryModel;
			}
			return false;
		}
	}

	public void Initialize(string variantId, GameObject wetModel, GameObject dryModel)
	{
		VariantId = variantId;
		_wetModel = wetModel;
		_dryModel = dryModel;
	}

	public void ShowWetModel()
	{
		_wetModel.SetActive(value: true);
		_dryModel.SetActive(value: false);
	}

	public void ShowDryModel()
	{
		_wetModel.SetActive(value: false);
		_dryModel.SetActive(value: true);
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(RuinModelsKey).Set(VariantIdKey, VariantId);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(RuinModelsKey);
		VariantId = component.Get(VariantIdKey);
	}
}
