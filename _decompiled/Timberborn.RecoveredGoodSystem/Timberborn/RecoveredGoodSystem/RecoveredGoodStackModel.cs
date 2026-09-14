using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.RecoveredGoodSystem;

internal class RecoveredGoodStackModel : BaseComponent, IAwakableComponent, IPersistentEntity
{
	private static readonly ComponentKey RecoveredGoodStackModelKey = new ComponentKey("RecoveredGoodStackModel");

	private static readonly PropertyKey<int> RotationKey = new PropertyKey<int>("Rotation");

	private Transform _model;

	public void Awake()
	{
		string modelName = GetComponent<RecoveredGoodStackModelSpec>().ModelName;
		_model = base.GameObject.FindChildTransform(modelName);
	}

	public void SetRotation(int rotation)
	{
		_model.localRotation = Quaternion.Euler(0f, rotation, 0f);
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(RecoveredGoodStackModelKey).Set(RotationKey, (int)_model.rotation.eulerAngles.y);
	}

	public void Load(IEntityLoader entityLoader)
	{
		int rotation = entityLoader.GetComponent(RecoveredGoodStackModelKey).Get(RotationKey);
		SetRotation(rotation);
	}
}
