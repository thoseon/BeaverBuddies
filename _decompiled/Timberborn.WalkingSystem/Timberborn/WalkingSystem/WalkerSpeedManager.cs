using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;
using Timberborn.CharacterMovementSystem;
using Timberborn.Navigation;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public class WalkerSpeedManager : TickableComponent, IAwakableComponent
{
	private static readonly string MovementBonusId = "MovementSpeed";

	private static readonly float SwimmingPenalty = 0.3f;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly BonusTypeSpecService _bonusTypeSpecService;

	private BonusManager _bonusManager;

	private IMovementSpeedAffector _movementSpeedAffector;

	private readonly List<IWaterPenaltyModifier> _waterPenaltyModifiers = new List<IWaterPenaltyModifier>();

	private WalkerSpeedManagerSpec _walkerSpeedManagerSpec;

	private float _minWalkingSpeedMultiplier;

	private float _baseSpeed;

	private float _minimumSpeed;

	private float _bonusMultiplier;

	public WalkerSpeedManager(IThreadSafeWaterMap threadSafeWaterMap, BonusTypeSpecService bonusTypeSpecService)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
		_bonusTypeSpecService = bonusTypeSpecService;
	}

	public void Awake()
	{
		_bonusManager = GetComponent<BonusManager>();
		_movementSpeedAffector = GetComponent<IMovementSpeedAffector>();
		GetComponents(_waterPenaltyModifiers);
		_walkerSpeedManagerSpec = GetComponent<WalkerSpeedManagerSpec>();
		_minWalkingSpeedMultiplier = _bonusTypeSpecService.GetSpec(MovementBonusId).MinimumValue;
		_minimumSpeed = _walkerSpeedManagerSpec.BaseSlowedSpeed * _minWalkingSpeedMultiplier;
	}

	public override void Tick()
	{
		bool flag = _movementSpeedAffector?.IsMovementSlowed ?? false;
		_baseSpeed = (flag ? _walkerSpeedManagerSpec.BaseSlowedSpeed : _walkerSpeedManagerSpec.BaseWalkingSpeed);
		_bonusMultiplier = _bonusManager.Multiplier(MovementBonusId);
	}

	public float GetWalkerBaseSpeed()
	{
		return _baseSpeed * _bonusMultiplier;
	}

	public float GetWalkerSpeedAtCurrentPosition()
	{
		Vector3Int coordinates = NavigationCoordinateSystem.WorldToGridInt(base.Transform.position);
		if (_threadSafeWaterMap.CellIsUnderwater(coordinates))
		{
			float num = 1f;
			foreach (IWaterPenaltyModifier waterPenaltyModifier in _waterPenaltyModifiers)
			{
				num *= waterPenaltyModifier.WaterPenaltyModifier;
			}
			if (num > 0f)
			{
				float num2 = Mathf.Max(_bonusMultiplier - SwimmingPenalty, _minWalkingSpeedMultiplier);
				return Mathf.Max(_baseSpeed * num2 * num, _minimumSpeed);
			}
		}
		return GetWalkerBaseSpeed();
	}
}
