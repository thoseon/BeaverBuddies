using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;
using Timberborn.EnterableSystem;
using Timberborn.EntitySystem;
using Timberborn.MechanicalSystem;
using Timberborn.TickSystem;
using Timberborn.WorkSystem;
using Timberborn.WorkshopsEffects;
using UnityEngine;

namespace Timberborn.PowerGeneration;

public class WalkerPoweredGenerator : TickableComponent, IAwakableComponent, IPostLoadableEntity, IWorkshopAnimationSpeedModifier
{
	private static readonly string MovementBonusId = "MovementSpeed";

	private static readonly float MinSpeedModifier = 0.85f;

	private static readonly float MaxSpeedModifier = 2f;

	private MechanicalNode _mechanicalNode;

	private Workplace _workplace;

	private Enterable _enterable;

	private readonly List<BonusManager> _bonusManagers = new List<BonusManager>();

	public float SpeedModifier { get; private set; }

	public event EventHandler SpeedModifierChanged;

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_workplace = GetComponent<Workplace>();
		_enterable = GetComponent<Enterable>();
		_enterable.EntererAdded += delegate(object _, EntererAddedEventArgs e)
		{
			AddBonusManager(e.Enterer);
		};
		_enterable.EntererRemoved += delegate(object _, EntererRemovedEventArgs e)
		{
			RemoveBonusManager(e.Enterer);
		};
	}

	public void PostLoadEntity()
	{
		UpdateGenerator(forceUpdate: true);
	}

	public override void Tick()
	{
		UpdateGenerator();
	}

	private void AddBonusManager(BaseComponent baseComponent)
	{
		_bonusManagers.Add(baseComponent.GetComponent<BonusManager>());
	}

	private void RemoveBonusManager(BaseComponent baseComponent)
	{
		_bonusManagers.Remove(baseComponent.GetComponent<BonusManager>());
	}

	private void UpdateGenerator(bool forceUpdate = false)
	{
		float generatorStrength = GetGeneratorStrength();
		if (forceUpdate || !_mechanicalNode.OutputMultiplier.Equals(generatorStrength))
		{
			_mechanicalNode.SetOutputMultiplier(generatorStrength);
			UpdateSpeedModifier();
		}
	}

	private float GetGeneratorStrength()
	{
		if (_bonusManagers.Count <= 0)
		{
			return 0f;
		}
		return GetBonusMultiplier();
	}

	private float GetBonusMultiplier()
	{
		float num = 0f;
		foreach (BonusManager bonusManager in _bonusManagers)
		{
			num += bonusManager.Multiplier(MovementBonusId);
		}
		return num / (float)_workplace.MaxWorkers;
	}

	private void UpdateSpeedModifier()
	{
		SpeedModifier = Mathf.Lerp(MinSpeedModifier, MaxSpeedModifier, _mechanicalNode.OutputMultiplier / MaxSpeedModifier);
		SpeedModifierChanged?.Invoke(this, EventArgs.Empty);
	}
}
