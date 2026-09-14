using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WaterSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WaterSourceSystem;

internal class WaterDepthStrengthModifier : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity, IPersistentEntity, IWaterStrengthModifier
{
	private static readonly float HysteresisBottomScale = 0.9f;

	private static readonly float FadeInSpeed = 0.5f;

	private static readonly ComponentKey WaterDepthStrengthModifierKey = new ComponentKey("WaterDepthStrengthModifier");

	private static readonly PropertyKey<float> CurrentModifierKey = new PropertyKey<float>("CurrentModifier");

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private WaterDepthStrengthModifierSpec _spec;

	private BlockObject _blockObject;

	private WaterSource _waterSource;

	private bool _isEnabled;

	private float _currentModifier;

	public WaterDepthStrengthModifier(IThreadSafeWaterMap threadSafeWaterMap)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void Awake()
	{
		_spec = GetComponent<WaterDepthStrengthModifierSpec>();
		_blockObject = GetComponent<BlockObject>();
		_waterSource = GetComponent<WaterSource>();
	}

	public void InitializeEntity()
	{
		_waterSource.AddWaterStrengthModifier(this);
	}

	public void DeleteEntity()
	{
		_waterSource.RemoveWaterStrengthModifier(this);
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(WaterDepthStrengthModifierKey).Set(CurrentModifierKey, _currentModifier);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(WaterDepthStrengthModifierKey);
		_currentModifier = component.Get(CurrentModifierKey);
	}

	public float GetStrengthModifier()
	{
		UpdateEnabledState();
		_currentModifier = (_isEnabled ? Mathf.MoveTowards(_currentModifier, 1f, FadeInSpeed * Time.deltaTime) : 0f);
		return _currentModifier;
	}

	private void UpdateEnabledState()
	{
		float num = _threadSafeWaterMap.WaterDepth(_blockObject.CoordinatesAtBaseZ);
		if (_isEnabled && num > _spec.DepthLimit)
		{
			_isEnabled = false;
		}
		else if (!_isEnabled && num < _spec.DepthLimit * HysteresisBottomScale)
		{
			_isEnabled = true;
		}
	}
}
