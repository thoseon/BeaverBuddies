using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Persistence;
using Timberborn.ResourceCountingSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Reproduction;

public class BreedingPod : BaseComponent, IAwakableComponent, IPersistentEntity, IFinishedStateListener, IRegisteredComponent, IFinishedPausable, IGoodProcessor
{
	private static readonly ComponentKey BreedingPodKey = new ComponentKey("BreedingPod");

	private static readonly PropertyKey<int> CyclesRemaining = new PropertyKey<int>("CyclesRemaining");

	private static readonly PropertyKey<float> GrowthProgressKey = new PropertyKey<float>("GrowthProgress");

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private readonly NewbornSpawner _newbornSpawner;

	private BlockableObject _blockableObject;

	private BreedingPodSpec _breedingPodSpec;

	private Building _building;

	private GameObject _embryo;

	private ITimeTrigger _timeTrigger;

	private int _cyclesRemaining;

	private readonly List<GoodAmount> _processedGoods = new List<GoodAmount>();

	public Inventory Inventory { get; private set; }

	public ReadOnlyList<GoodAmount> ProcessedGoods => _processedGoods.AsReadOnlyList();

	public ImmutableArray<GoodAmountSpec> NutrientsPerCycle => _breedingPodSpec.NutrientsPerCycle;

	public int CyclesUntilFullyGrown => _breedingPodSpec.CyclesUntilFullyGrown;

	public bool NeedsNutrients
	{
		get
		{
			if (_blockableObject.IsUnblocked)
			{
				return !Inventory.IsFullyReserved;
			}
			return false;
		}
	}

	public bool ProgressHalted
	{
		get
		{
			if (base.Enabled && _blockableObject.IsUnblocked)
			{
				return !_timeTrigger.InProgress;
			}
			return false;
		}
	}

	private IEnumerable<GoodAmount> Nutrients => NutrientsPerCycle.Select((GoodAmountSpec good) => good.ToGoodAmount());

	public BreedingPod(ITimeTriggerFactory timeTriggerFactory, NewbornSpawner newbornSpawner)
	{
		_timeTriggerFactory = timeTriggerFactory;
		_newbornSpawner = newbornSpawner;
	}

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_breedingPodSpec = GetComponent<BreedingPodSpec>();
		_building = GetComponent<Building>();
		_embryo = base.GameObject.FindChild(_breedingPodSpec.EmbryoName);
		_embryo.SetActive(value: false);
		_timeTrigger = _timeTriggerFactory.Create(FinishGrowthCycle, _breedingPodSpec.CycleLengthInDays);
		_timeTrigger.FastForwardProgress(1f);
		RestartGrowth();
		DisableComponent();
	}

	public void InitializeInventory(Inventory inventory)
	{
		Asserts.FieldIsNull(this, Inventory, "Inventory");
		Inventory = inventory;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(BreedingPodKey);
		component.Set(CyclesRemaining, _cyclesRemaining);
		if (!_timeTrigger.Finished)
		{
			component.Set(GrowthProgressKey, _timeTrigger.Progress);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(BreedingPodKey);
		_cyclesRemaining = component.Get(CyclesRemaining);
		if (component.Has(GrowthProgressKey))
		{
			_timeTrigger.Reset();
			_timeTrigger.FastForwardProgress(component.Get(GrowthProgressKey));
			_embryo.SetActive(value: true);
		}
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		Inventory.Enable();
		Inventory.InventoryChanged += OnInventoryChanged;
		_blockableObject.ObjectBlocked += OnObjectBlocked;
		_blockableObject.ObjectUnblocked += OnObjectUnblocked;
		if (_blockableObject.IsUnblocked)
		{
			_timeTrigger.Resume();
		}
		else
		{
			_timeTrigger.Pause();
		}
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		Inventory.Disable();
		Inventory.InventoryChanged -= OnInventoryChanged;
		_blockableObject.ObjectBlocked -= OnObjectBlocked;
		_blockableObject.ObjectUnblocked -= OnObjectUnblocked;
		_timeTrigger.Reset();
	}

	public bool HasResourcesToFinish()
	{
		foreach (GoodAmountSpec item in NutrientsPerCycle)
		{
			if (item.Amount * _cyclesRemaining > Inventory.AmountInStock(item.Id))
			{
				return false;
			}
		}
		return true;
	}

	public float CalculateProgress()
	{
		float num = 1f / (float)CyclesUntilFullyGrown;
		float num2 = 1f - num * (float)_cyclesRemaining;
		float num3 = (_timeTrigger.Finished ? 0f : (_timeTrigger.Progress * num));
		return Mathf.Clamp01(num2 + num3);
	}

	private void FinishGrowthCycle()
	{
		_cyclesRemaining--;
		_processedGoods.Clear();
		if (_cyclesRemaining == 0)
		{
			if (_breedingPodSpec.SpawnAdults)
			{
				_newbornSpawner.SpawnAdult(_building);
			}
			else
			{
				_newbornSpawner.SpawnChild(_building);
			}
			_embryo.SetActive(value: false);
			RestartGrowth();
		}
		RestartGrowthCycle();
	}

	private void RestartGrowth()
	{
		_cyclesRemaining = CyclesUntilFullyGrown;
	}

	private void RestartGrowthCycle()
	{
		if (!ShouldRestartGrowthCycle())
		{
			return;
		}
		_timeTrigger.Reset();
		_timeTrigger.Resume();
		_processedGoods.Clear();
		foreach (GoodAmount nutrient in Nutrients)
		{
			Inventory.TakeConsumed(nutrient);
			_processedGoods.Add(nutrient);
		}
		_embryo.SetActive(value: true);
	}

	private bool ShouldRestartGrowthCycle()
	{
		if (_timeTrigger.Finished)
		{
			return Nutrients.All((GoodAmount nutrient) => Inventory.HasUnreservedStock(nutrient));
		}
		return false;
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		RestartGrowthCycle();
	}

	private void OnObjectBlocked(object sender, EventArgs e)
	{
		_timeTrigger.Pause();
	}

	private void OnObjectUnblocked(object sender, EventArgs e)
	{
		_timeTrigger.Resume();
	}
}
