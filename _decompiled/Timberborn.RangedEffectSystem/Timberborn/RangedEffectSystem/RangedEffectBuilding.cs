using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.BuildingRange;
using Timberborn.Buildings;
using Timberborn.ConstructionMode;
using Timberborn.MechanicalSystem;
using Timberborn.NeedSpecs;
using Timberborn.TemplateSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.RangedEffectSystem;

public class RangedEffectBuilding : TickableComponent, IAwakableComponent, IFinishedStateListener, IBuildingWithRange
{
	private readonly ConstructionModeService _constructionModeService;

	private BuildingSounds _buildingSounds;

	private BlockableObject _blockableObject;

	private BlockObjectRange _blockObjectRange;

	private MechanicalBuilding _mechanicalBuilding;

	private TemplateSpec _templateSpec;

	private RangedEffectApplier _rangedEffectApplier;

	private RangedEffectBuildingSpec _rangedEffectBuildingSpec;

	private readonly List<ContinuousEffectSpec> _effects = new List<ContinuousEffectSpec>();

	private bool _wasActive;

	public int EffectRadius => _rangedEffectBuildingSpec.EffectRadius;

	public string RangeName => _templateSpec.TemplateName;

	private bool Active
	{
		get
		{
			if (base.Enabled && _blockableObject.IsUnblocked)
			{
				return MechanicalBuildingActive;
			}
			return false;
		}
	}

	private bool MechanicalBuildingActive
	{
		get
		{
			if (_mechanicalBuilding != null)
			{
				return _mechanicalBuilding.ActiveAndPowered;
			}
			return true;
		}
	}

	public RangedEffectBuilding(ConstructionModeService constructionModeService)
	{
		_constructionModeService = constructionModeService;
	}

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_blockObjectRange = GetComponent<BlockObjectRange>();
		_buildingSounds = GetComponent<BuildingSounds>();
		_mechanicalBuilding = GetComponent<MechanicalBuilding>();
		_templateSpec = GetComponent<TemplateSpec>();
		_rangedEffectApplier = GetComponent<RangedEffectApplier>();
		_rangedEffectBuildingSpec = GetComponent<RangedEffectBuildingSpec>();
		DisableComponent();
	}

	public override void Tick()
	{
		if ((bool)_mechanicalBuilding)
		{
			ToggleActiveState();
			_rangedEffectApplier.UpdateEfficiency(_mechanicalBuilding.Efficiency);
		}
	}

	public IEnumerable<BaseComponent> GetObjectsInRange()
	{
		return Enumerable.Empty<BaseComponent>();
	}

	public void OnEnterFinishedState()
	{
		_blockableObject.ObjectBlocked += OnObjectBlocked;
		_blockableObject.ObjectUnblocked += OnObjectUnblocked;
		EnableComponent();
		UpdateRangedEffectApplierState();
		ToggleActiveStateInternal(Active);
	}

	public void OnExitFinishedState()
	{
		ToggleActiveStateInternal(state: false);
		_blockableObject.ObjectBlocked -= OnObjectBlocked;
		_blockableObject.ObjectUnblocked -= OnObjectUnblocked;
		_rangedEffectApplier.Disable();
		DisableComponent();
	}

	public IEnumerable<Vector3Int> GetBlocksInRange()
	{
		bool finishedOnly = !_constructionModeService.InConstructionMode;
		return _blockObjectRange.GetBlocksOnTerrainOrStackableInRectangularRadius(EffectRadius, finishedOnly);
	}

	public void AddEffect(ContinuousEffectSpec additionalEffect)
	{
		_effects.Add(additionalEffect);
		UpdateRangedEffectApplierState();
	}

	public void RemoveEffect(ContinuousEffectSpec additionalEffect)
	{
		if (_effects.Remove(additionalEffect))
		{
			UpdateRangedEffectApplierState();
		}
	}

	private void OnObjectBlocked(object sender, EventArgs e)
	{
		ToggleActiveState();
	}

	private void OnObjectUnblocked(object sender, EventArgs e)
	{
		ToggleActiveState();
	}

	private void UpdateRangedEffectApplierState()
	{
		if (_rangedEffectApplier.Enabled)
		{
			_rangedEffectApplier.Disable();
		}
		IEnumerable<Vector2Int> blocksInRectangularRadius = _blockObjectRange.GetBlocksInRectangularRadius(EffectRadius);
		_rangedEffectApplier.Enable(_effects, blocksInRectangularRadius, Active);
	}

	private void ToggleActiveState()
	{
		bool active = Active;
		if (_wasActive != active)
		{
			ToggleActiveStateInternal(active);
			_rangedEffectApplier.UpdateActiveState(active);
		}
	}

	private void ToggleActiveStateInternal(bool state)
	{
		_buildingSounds.ToggleSound(state);
		_wasActive = state;
	}
}
