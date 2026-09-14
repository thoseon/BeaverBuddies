using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Persistence;
using Timberborn.ResourceCountingSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.FireworkSystem;

public class FireworkLauncher : BaseComponent, IAwakableComponent, IPersistentEntity, IInitializableEntity, IFinishedStateListener, IAutomatableNeeder, ITerminal, IDuplicable<FireworkLauncher>, IDuplicable, IGoodProcessor
{
	private static readonly ComponentKey ComponentKey = new ComponentKey("FireworkLauncher");

	private static readonly PropertyKey<string> FireworkIdKey = new PropertyKey<string>("FireworkId");

	private static readonly PropertyKey<int> HeadingKey = new PropertyKey<int>("Heading");

	private static readonly PropertyKey<int> PitchKey = new PropertyKey<int>("Pitch");

	private static readonly PropertyKey<int> FlightDistanceKey = new PropertyKey<int>("FlightDistance");

	private static readonly PropertyKey<bool> IsContinuousKey = new PropertyKey<bool>("IsContinuous");

	private static readonly PropertyKey<bool> PreviousStateKey = new PropertyKey<bool>("PreviousState");

	private readonly FireworkSpawner _fireworkSpawner;

	private readonly FireworkSpecService _fireworkSpecService;

	private readonly FireworkLaunchService _fireworkLaunchService;

	private Automatable _automatable;

	private BlockableObject _blockableObject;

	private bool _currentState;

	private bool _previousState;

	private bool _isArmed;

	public string FireworkId { get; private set; }

	public int Heading { get; private set; }

	public int Pitch { get; private set; }

	public int FlightDistance { get; private set; } = 20;

	public bool IsContinuous { get; private set; }

	public Inventory Inventory { get; private set; }

	public ReadOnlyList<GoodAmount> ProcessedGoods { get; } = new List<GoodAmount>().AsReadOnlyList();

	public bool NeedsAutomatable => true;

	public event EventHandler AnglesChanged;

	internal FireworkLauncher(FireworkSpawner fireworkSpawner, FireworkSpecService fireworkSpecService, FireworkLaunchService fireworkLaunchService)
	{
		_fireworkSpawner = fireworkSpawner;
		_fireworkSpecService = fireworkSpecService;
		_fireworkLaunchService = fireworkLaunchService;
	}

	public void Awake()
	{
		_automatable = GetComponent<Automatable>();
		_blockableObject = GetComponent<BlockableObject>();
	}

	public void InitializeEntity()
	{
		if (string.IsNullOrWhiteSpace(FireworkId) || !_fireworkSpecService.HasSpec(FireworkId))
		{
			FireworkId = _fireworkSpecService.GetFireworkIds().First();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ComponentKey);
		component.Set(FireworkIdKey, FireworkId);
		component.Set(HeadingKey, Heading);
		component.Set(PitchKey, Pitch);
		component.Set(FlightDistanceKey, FlightDistance);
		component.Set(IsContinuousKey, IsContinuous);
		component.Set(PreviousStateKey, _previousState);
	}

	[BackwardCompatible(2026, 3, 5, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(ComponentKey, out var objectLoader) && objectLoader.Has(FireworkIdKey))
		{
			FireworkId = objectLoader.Get(FireworkIdKey);
			Heading = objectLoader.Get(HeadingKey);
			Pitch = objectLoader.Get(PitchKey);
			FlightDistance = objectLoader.Get(FlightDistanceKey);
			IsContinuous = objectLoader.Get(IsContinuousKey);
			_previousState = objectLoader.Get(PreviousStateKey);
		}
	}

	public void DuplicateFrom(FireworkLauncher source)
	{
		SetFireworkId(source.FireworkId);
		SetHeading(source.Heading);
		SetPitch(source.Pitch);
		SetFlightDistance(source.FlightDistance);
		SetContinuous(source.IsContinuous);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		Inventory.Enable();
		_fireworkLaunchService.Add(this);
	}

	public void OnExitFinishedState()
	{
		Inventory.Disable();
		DisableComponent();
		_fireworkLaunchService.Remove(this);
	}

	public void SetFireworkId(string fireworkId)
	{
		FireworkId = fireworkId;
	}

	public void SetHeading(int heading)
	{
		Heading = Mathf.Clamp(heading, FireworkLimits.MinHeading, FireworkLimits.MaxHeading);
		AnglesChanged?.Invoke(this, EventArgs.Empty);
	}

	public void SetPitch(int pitch)
	{
		Pitch = Mathf.Clamp(pitch, FireworkLimits.MinPitch, FireworkLimits.MaxPitch);
		AnglesChanged?.Invoke(this, EventArgs.Empty);
	}

	public void SetFlightDistance(int distance)
	{
		FlightDistance = Mathf.Clamp(distance, FireworkLimits.MinFlightDistance, FireworkLimits.MaxFlightDistance);
	}

	public void SetContinuous(bool isContinuous)
	{
		IsContinuous = isContinuous;
		Evaluate();
	}

	public void InitializeInventory(Inventory inventory)
	{
		Asserts.FieldIsNull(this, Inventory, "Inventory");
		Inventory = inventory;
	}

	public void Evaluate()
	{
		_currentState = _automatable.State == ConnectionState.On;
		if (IsContinuous)
		{
			_isArmed = _currentState;
		}
		else if (_currentState != _previousState)
		{
			_isArmed = _currentState && !_previousState;
		}
		_previousState = _currentState;
	}

	internal void LaunchIfArmed()
	{
		if (_isArmed)
		{
			if (base.Enabled && _blockableObject.IsUnblocked && !Inventory.IsEmpty)
			{
				ConsumeGoods();
				_fireworkSpawner.SpawnFirework(this);
			}
			_isArmed = IsContinuous;
		}
	}

	private void ConsumeGoods()
	{
		foreach (StorableGoodAmount allowedGood in Inventory.AllowedGoods)
		{
			if (Inventory.UnreservedAmountInStock(allowedGood.StorableGood.GoodId) > 0)
			{
				Inventory.TakeConsumed(new GoodAmount(allowedGood.StorableGood.GoodId, 1));
			}
		}
	}
}
