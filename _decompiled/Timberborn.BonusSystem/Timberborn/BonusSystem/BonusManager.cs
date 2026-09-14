using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.BonusSystem;

public class BonusManager : BaseComponent, IAwakableComponent
{
	private class Bonus
	{
		private readonly float _min;

		private readonly float _max;

		private float _value;

		public float ClampedValue => Mathf.Clamp(_value, _min, _max);

		public Bonus(float value, float min, float max)
		{
			_value = value;
			_min = min;
			_max = max;
		}

		public void ModifyValue(float change)
		{
			_value += change;
		}
	}

	private readonly BonusTypeSpecService _bonusTypeSpecService;

	private readonly Dictionary<string, Bonus> _bonuses = new Dictionary<string, Bonus>();

	public event EventHandler<BonusValueChangedEventArgs> BonusValueChanged;

	public BonusManager(BonusTypeSpecService bonusTypeSpecService)
	{
		_bonusTypeSpecService = bonusTypeSpecService;
	}

	public void Awake()
	{
		InitializeBonuses();
	}

	public void AddBonuses(IEnumerable<BonusSpec> bonuses)
	{
		foreach (BonusSpec bonuse in bonuses)
		{
			AddBonus(bonuse.Id, bonuse.MultiplierDelta);
		}
	}

	public void RemoveBonuses(IEnumerable<BonusSpec> bonuses)
	{
		foreach (BonusSpec bonuse in bonuses)
		{
			RemoveBonus(bonuse.Id, bonuse.MultiplierDelta);
		}
	}

	public void AddBonus(string bonusId, float multiplierDelta)
	{
		_bonuses[bonusId].ModifyValue(multiplierDelta);
		NotifyBonusValueChanged(bonusId);
	}

	public void RemoveBonus(string bonusId, float multiplierDelta)
	{
		_bonuses[bonusId].ModifyValue(0f - multiplierDelta);
		NotifyBonusValueChanged(bonusId);
	}

	public float Multiplier(string bonusType)
	{
		return _bonuses[bonusType].ClampedValue;
	}

	private void InitializeBonuses()
	{
		foreach (string bonusId in _bonusTypeSpecService.BonusIds)
		{
			BonusTypeSpec spec = _bonusTypeSpecService.GetSpec(bonusId);
			_bonuses[bonusId] = new Bonus(1f, spec.MinimumValue, spec.MaximumValue);
		}
	}

	private void NotifyBonusValueChanged(string bonusId)
	{
		BonusValueChanged?.Invoke(this, new BonusValueChangedEventArgs(bonusId, Multiplier(bonusId)));
	}
}
