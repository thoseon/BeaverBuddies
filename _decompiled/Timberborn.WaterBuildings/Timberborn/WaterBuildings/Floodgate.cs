using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.WaterObjects;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WaterBuildings;

public class Floodgate : BaseComponent, IAwakableComponent, IInitializablePreview, IFinishedStateListener, IUnfinishedStateListener, IPersistentEntity, IDuplicable<Floodgate>, IDuplicable, ITerminal
{
	private static readonly ComponentKey FloodgateKey = new ComponentKey("Floodgate");

	private static readonly PropertyKey<bool> IsSynchronizedKey = new PropertyKey<bool>("IsSynchronized");

	private static readonly PropertyKey<float> HeightKey = new PropertyKey<float>("Height");

	private static readonly PropertyKey<float> AutomationHeightKey = new PropertyKey<float>("AutomationHeight");

	private static readonly float DefaultHeightOffset = 0.35f;

	private readonly FloodgateSynchronizer _floodgateSynchronizer;

	private BlockObject _blockObject;

	private WaterObstacle _waterObstacle;

	private Automatable _automatable;

	private FloodgateAnimationController _animationController;

	private FloodgateSpec _floodgateSpec;

	private float? _lastEffectiveHeight;

	private bool _isDuplicated;

	public bool IsSynchronized { get; private set; } = true;

	public float Height { get; private set; }

	public float AutomationHeight { get; private set; }

	public int MaxHeight => _floodgateSpec.MaxHeight;

	public float PositionedHeight => (float)_blockObject.Coordinates.z + Height;

	public float PositionedAutomationHeight => (float)_blockObject.Coordinates.z + AutomationHeight;

	public bool IsAutomated => _automatable.IsAutomated;

	public bool IsInputOn => _automatable.State == ConnectionState.On;

	internal Floodgate(FloodgateSynchronizer floodgateSynchronizer)
	{
		_floodgateSynchronizer = floodgateSynchronizer;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_waterObstacle = GetComponent<WaterObstacle>();
		_automatable = GetComponent<Automatable>();
		_animationController = GetComponent<FloodgateAnimationController>();
		_floodgateSpec = GetComponent<FloodgateSpec>();
		Height = (float)MaxHeight - DefaultHeightOffset;
		AutomationHeight = MaxHeight;
		DisableComponent();
		_automatable.InputReconnected += OnAutomatableInputReconnected;
	}

	public void InitializePreview()
	{
		UpdateEffectiveHeight(forceInstant: true);
	}

	public void SetHeightAndSynchronize(float newHeight)
	{
		SetHeight(newHeight);
		SynchronizeAllNeighbors();
	}

	public void SetAutomationHeightAndSynchronize(float newAutomationHeight)
	{
		SetAutomationHeight(newAutomationHeight);
		SynchronizeAllNeighbors();
	}

	public void SetHeight(float newHeight)
	{
		Height = ClampHeight(newHeight);
		UpdateEffectiveHeight(forceInstant: false);
	}

	public void SetAutomationHeight(float newAutomationHeight)
	{
		AutomationHeight = ClampHeight(newAutomationHeight);
		UpdateEffectiveHeight(forceInstant: false);
	}

	public void ToggleSynchronization(bool newValue)
	{
		IsSynchronized = newValue;
		_floodgateSynchronizer.SynchronizeWithAllNeighbors(this);
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(FloodgateKey);
		component.Set(HeightKey, Height);
		component.Set(AutomationHeightKey, AutomationHeight);
		component.Set(IsSynchronizedKey, IsSynchronized);
	}

	[BackwardCompatible(2025, 12, 15, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(FloodgateKey);
		Height = component.Get(HeightKey);
		if (component.Has(AutomationHeightKey))
		{
			AutomationHeight = component.Get(AutomationHeightKey);
		}
		IsSynchronized = component.Get(IsSynchronizedKey);
	}

	public void DuplicateFrom(Floodgate source)
	{
		IsSynchronized = source.IsSynchronized;
		Height = ClampHeight(source.Height);
		AutomationHeight = ClampHeight(source.AutomationHeight);
		UpdateEffectiveHeight(forceInstant: false);
		_isDuplicated = true;
	}

	public void OnEnterUnfinishedState()
	{
		if (_isDuplicated)
		{
			SynchronizeAllNeighbors();
		}
		else
		{
			_floodgateSynchronizer.SynchronizeWithUnfinishedNeighbors(this);
		}
		UpdateEffectiveHeight(forceInstant: true);
	}

	public void OnExitUnfinishedState()
	{
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		UpdateEffectiveHeight(forceInstant: true);
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		_waterObstacle.RemoveFromWaterService();
	}

	public void Evaluate()
	{
		UpdateEffectiveHeight(forceInstant: false);
	}

	private void OnAutomatableInputReconnected(object sender, EventArgs e)
	{
		SynchronizeAllNeighbors();
	}

	private void UpdateEffectiveHeight(bool forceInstant)
	{
		float num = ((_automatable.State == ConnectionState.On) ? AutomationHeight : Height);
		if (!_lastEffectiveHeight.Equals(num))
		{
			SetVisualHeight(num, forceInstant);
			if (_blockObject.IsFinished)
			{
				SetObstacleHeight(num);
				_lastEffectiveHeight = num;
			}
		}
	}

	private void SetVisualHeight(float effectiveHeight, bool forceInstant)
	{
		if (forceInstant || !_blockObject.IsFinished)
		{
			_animationController.MoveGateInstantly(effectiveHeight);
		}
		else
		{
			_animationController.MoveGateSmoothly(effectiveHeight);
		}
	}

	private void SetObstacleHeight(float effectiveHeight)
	{
		_waterObstacle.RemoveFromWaterService();
		if (effectiveHeight > 0f)
		{
			_waterObstacle.AddToWaterService(effectiveHeight);
		}
	}

	private void SynchronizeAllNeighbors()
	{
		_floodgateSynchronizer.SynchronizeAllNeighbors(this);
	}

	private float ClampHeight(float newHeight)
	{
		return Mathf.Clamp(newHeight, 0f, MaxHeight);
	}
}
