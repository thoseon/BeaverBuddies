using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Debugging;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WaterSourceSystem;

public class WaterSource : TickableComponent, IAwakableComponent, IPersistentEntity, IDuplicable<WaterSource>, IDuplicable, IRegisteredComponent, IInitializableEntity, IPostInitializableEntity, IDeletableEntity, IWaterSource
{
	private static readonly ComponentKey WaterSourceKey = new ComponentKey("WaterSource");

	private static readonly PropertyKey<float> SpecifiedStrengthKey = new PropertyKey<float>("SpecifiedStrength");

	private static readonly PropertyKey<float> CurrentStrengthKey = new PropertyKey<float>("CurrentStrength");

	private readonly IWaterService _waterService;

	private readonly WaterStrengthService _waterStrengthService;

	private readonly MapEditorMode _mapEditorMode;

	private readonly DevModeManager _devModeManager;

	private BlockObject _blockObject;

	private WaterSourceContamination _waterSourceContamination;

	private WaterSourceSpec _waterSourceSpec;

	private readonly List<IWaterStrengthModifier> _waterStrengthModifiers = new List<IWaterStrengthModifier>();

	public ImmutableArray<Vector3Int> Coordinates { get; private set; }

	public float SpecifiedStrength { get; private set; }

	public float CurrentStrength { get; private set; }

	public ReadOnlyList<IWaterStrengthModifier> WaterStrengthModifiers => _waterStrengthModifiers.AsReadOnlyList();

	public float Contamination => _waterSourceContamination.Contamination;

	public bool IsDuplicable
	{
		get
		{
			if (!_mapEditorMode.IsMapEditor)
			{
				return _devModeManager.Enabled;
			}
			return true;
		}
	}

	public event EventHandler WaterStrengthModifierAdded;

	public WaterSource(IWaterService waterService, WaterStrengthService waterStrengthService, MapEditorMode mapEditorMode, DevModeManager devModeManager)
	{
		_waterService = waterService;
		_waterStrengthService = waterStrengthService;
		_mapEditorMode = mapEditorMode;
		_devModeManager = devModeManager;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_waterSourceContamination = GetComponent<WaterSourceContamination>();
		_waterSourceSpec = GetComponent<WaterSourceSpec>();
		float specifiedStrength = (CurrentStrength = _waterSourceSpec.DefaultStrength);
		SpecifiedStrength = specifiedStrength;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(WaterSourceKey);
		component.Set(SpecifiedStrengthKey, SpecifiedStrength);
		component.Set(CurrentStrengthKey, CurrentStrength);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(WaterSourceKey);
		SetCurrentStrength(LimitStrength(component.Get(CurrentStrengthKey)));
		SpecifiedStrength = LimitStrength(component.Get(SpecifiedStrengthKey));
	}

	public void DuplicateFrom(WaterSource source)
	{
		SetSpecifiedStrength(source.SpecifiedStrength);
	}

	public void InitializeEntity()
	{
		Coordinates = (from coords in _waterSourceSpec.Coordinates.Select(_blockObject.TransformTile)
			select new Vector3Int(coords.x, coords.y, _blockObject.Coordinates.z + _blockObject.BaseZ)).ToImmutableArray();
		_waterService.RegisterWaterSource(this);
	}

	public void PostInitializeEntity()
	{
		UpdateCurrentStrength();
	}

	public void DeleteEntity()
	{
		_waterService.UnregisterWaterSource(this);
	}

	public override void Tick()
	{
		UpdateCurrentStrength();
	}

	public void SetSpecifiedStrength(float strength)
	{
		float num = LimitStrength(strength);
		if (!Mathf.Approximately(num, SpecifiedStrength))
		{
			SpecifiedStrength = num;
			UpdateCurrentStrength();
		}
	}

	public void AddWaterStrengthModifier(IWaterStrengthModifier waterStrengthModifier)
	{
		_waterStrengthModifiers.Add(waterStrengthModifier);
		UpdateCurrentStrength();
		WaterStrengthModifierAdded?.Invoke(this, EventArgs.Empty);
	}

	public void RemoveWaterStrengthModifier(IWaterStrengthModifier waterStrengthModifier)
	{
		_waterStrengthModifiers.Remove(waterStrengthModifier);
		UpdateCurrentStrength();
	}

	private void UpdateCurrentStrength()
	{
		float num = SpecifiedStrength;
		foreach (IWaterStrengthModifier waterStrengthModifier in _waterStrengthModifiers)
		{
			num *= waterStrengthModifier.GetStrengthModifier();
		}
		SetCurrentStrength(num);
	}

	private float LimitStrength(float strength)
	{
		float b = _waterStrengthService.MaxWaterSourceStrength * (float)_waterSourceSpec.Coordinates.Length;
		return Mathf.Min(strength, b);
	}

	private void SetCurrentStrength(float strength)
	{
		CurrentStrength = LimitStrength(strength);
	}
}
