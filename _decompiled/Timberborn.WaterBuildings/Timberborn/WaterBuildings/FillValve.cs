using System;
using System.Collections.Immutable;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WaterBuildings;

public class FillValve : TickableComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener, IUnfinishedStateListener, IPersistentEntity, IDuplicable<FillValve>, IDuplicable, ITerminal
{
	private static readonly float OpeningStep = 0.005f;

	private static readonly float MaxFlow = 10f;

	private static readonly ImmutableArray<float> ClosingCurve;

	private static readonly ComponentKey ComponentKey;

	private static readonly PropertyKey<bool> IsSynchronizedKey;

	private static readonly PropertyKey<bool> TargetHeightEnabledKey;

	private static readonly PropertyKey<float> TargetHeightKey;

	private static readonly PropertyKey<bool> AutomationTargetHeightEnabledKey;

	private static readonly PropertyKey<float> AutomationTargetHeightKey;

	private static readonly PropertyKey<FlowControllerState> FlowControllerStateKey;

	private static readonly PropertyKey<bool> ObstacleAddedKey;

	private static readonly PropertyKey<float> CurrentFlowKey;

	private readonly IWaterService _waterService;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly FillValveSynchronizer _fillValveSynchronizer;

	private BlockObject _blockObject;

	private Automatable _automatable;

	private WaterObstacleController _waterObstacleController;

	private FillValveSpec _fillValveSpec;

	private FlowControllerState _flowControllerState;

	private bool _obstacleAdded;

	private bool _isLoaded;

	private bool _isDuplicated;

	private float _currentFlow;

	private float? _oldHeight;

	public bool IsSynchronized { get; private set; } = true;

	public bool TargetHeightEnabled { get; private set; }

	public float TargetHeight { get; private set; }

	public bool AutomationTargetHeightEnabled { get; private set; }

	public float AutomationTargetHeight { get; private set; }

	public Vector3Int OutputCoordinates { get; private set; }

	public bool IsAutomated => _automatable.IsAutomated;

	public bool IsInputOn => _automatable.State == ConnectionState.On;

	public int MinTargetHeight
	{
		get
		{
			if (!_threadSafeWaterMap.TryGetColumnFloor(OutputCoordinates, out var floor))
			{
				return MaxTargetHeight;
			}
			return floor;
		}
	}

	public int MaxTargetHeight => _blockObject.Coordinates.z + 1;

	public float ActualHeight => _threadSafeWaterMap.WaterHeightOrFloor(OutputCoordinates);

	public float ClampedTargetHeight => Mathf.Clamp(TargetHeight, MinTargetHeight, MaxTargetHeight);

	public float ClampedAutomationTargetHeight => Mathf.Clamp(AutomationTargetHeight, MinTargetHeight, MaxTargetHeight);

	public float TargetDepth => ClampedTargetHeight - (float)MinTargetHeight;

	public float AutomationTargetDepth => ClampedAutomationTargetHeight - (float)MinTargetHeight;

	private float EffectiveTargetHeight
	{
		get
		{
			if (!IsAutomated || !IsInputOn)
			{
				if (!TargetHeightEnabled)
				{
					return float.PositiveInfinity;
				}
				return TargetHeight;
			}
			if (!AutomationTargetHeightEnabled)
			{
				return float.PositiveInfinity;
			}
			return AutomationTargetHeight;
		}
	}

	internal FillValve(IWaterService waterService, IThreadSafeWaterMap threadSafeWaterMap, FillValveSynchronizer fillValveSynchronizer)
	{
		_waterService = waterService;
		_threadSafeWaterMap = threadSafeWaterMap;
		_fillValveSynchronizer = fillValveSynchronizer;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_automatable = GetComponent<Automatable>();
		_waterObstacleController = GetComponent<WaterObstacleController>();
		_fillValveSpec = GetComponent<FillValveSpec>();
		DisableComponent();
		_automatable.InputReconnected += OnAutomatableInputReconnected;
	}

	public void InitializeEntity()
	{
		InitializeOutputCoordinates();
		if (!_isLoaded && !_isDuplicated)
		{
			TargetHeightEnabled = _fillValveSpec.DefaultTargetHeightEnabled;
			TargetHeight = (float)MinTargetHeight + _fillValveSpec.DefaultTargetHeightOffset;
			AutomationTargetHeightEnabled = _fillValveSpec.DefaultAutomationTargetHeightEnabled;
			AutomationTargetHeight = (float)MinTargetHeight + _fillValveSpec.DefaultAutomationTargetHeightOffset;
		}
		if (_oldHeight.HasValue)
		{
			TargetHeight = (float)MaxTargetHeight + _oldHeight.Value;
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ComponentKey);
		component.Set(IsSynchronizedKey, IsSynchronized);
		component.Set(TargetHeightEnabledKey, TargetHeightEnabled);
		component.Set(TargetHeightKey, TargetHeight);
		component.Set(AutomationTargetHeightEnabledKey, AutomationTargetHeightEnabled);
		component.Set(AutomationTargetHeightKey, AutomationTargetHeight);
		component.Set(FlowControllerStateKey, _flowControllerState);
		component.Set(ObstacleAddedKey, _obstacleAdded);
		component.Set(CurrentFlowKey, _currentFlow);
	}

	[BackwardCompatible(2026, 4, 3, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader objectLoader2;
		if (entityLoader.TryGetComponent(ComponentKey, out var objectLoader))
		{
			IsSynchronized = objectLoader.Get(IsSynchronizedKey);
			SetTargetHeightEnabled(objectLoader.Get(TargetHeightEnabledKey));
			SetTargetHeight(objectLoader.Get(TargetHeightKey));
			SetAutomationTargetHeightEnabled(objectLoader.Get(AutomationTargetHeightEnabledKey));
			SetAutomationTargetHeight(objectLoader.Get(AutomationTargetHeightKey));
			_flowControllerState = objectLoader.Get(FlowControllerStateKey);
			_obstacleAdded = objectLoader.Has(ObstacleAddedKey) && objectLoader.Get(ObstacleAddedKey);
			_currentFlow = (objectLoader.Has(CurrentFlowKey) ? objectLoader.Get(CurrentFlowKey) : 0f);
			_isLoaded = true;
		}
		else if (entityLoader.TryGetComponent(new ComponentKey("SluiceState"), out objectLoader2))
		{
			IsSynchronized = objectLoader2.Get(new PropertyKey<bool>("IsSynchronized"));
			SetTargetHeightEnabled(objectLoader2.Get(new PropertyKey<bool>("AutoCloseOnOutflow")));
			_oldHeight = objectLoader2.Get(new PropertyKey<float>("OutflowLimit"));
			_isLoaded = true;
		}
	}

	public void DuplicateFrom(FillValve source)
	{
		InitializeOutputCoordinates();
		IsSynchronized = source.IsSynchronized;
		SetTargetHeightEnabled(source.TargetHeightEnabled);
		SetTargetHeight((float)MinTargetHeight + source.TargetDepth);
		SetAutomationTargetHeightEnabled(source.AutomationTargetHeightEnabled);
		SetAutomationTargetHeight((float)MinTargetHeight + source.AutomationTargetDepth);
		_isDuplicated = true;
	}

	public void OnEnterUnfinishedState()
	{
		if (_isDuplicated)
		{
			SynchronizeNeighbors();
		}
		else
		{
			_fillValveSynchronizer.SynchronizeWithUnfinishedNeighbors(this);
		}
	}

	public void OnExitUnfinishedState()
	{
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		_waterService.AddDirectionLimiter(_blockObject.Coordinates, _blockObject.Orientation.ToFlowDirection());
		_waterObstacleController.UpdateState(_obstacleAdded);
		if (_currentFlow > 0f)
		{
			_waterService.SetInflowLimit(_blockObject.Coordinates, _currentFlow);
		}
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		_waterService.RemoveDirectionLimiter(_blockObject.Coordinates);
		_waterService.RemoveInflowLimit(_blockObject.Coordinates);
	}

	public override void Tick()
	{
		if (TargetHeight < (float)MinTargetHeight)
		{
			SetTargetHeight(MinTargetHeight);
		}
		if (AutomationTargetHeight < (float)MinTargetHeight)
		{
			SetAutomationTargetHeight(MinTargetHeight);
		}
		if (EffectiveTargetHeight > (float)MinTargetHeight)
		{
			UpdateObstacle(add: false);
			if (EffectiveTargetHeight < float.PositiveInfinity || _flowControllerState != FlowControllerState.NoControl)
			{
				TickOutflowLimit();
			}
			else
			{
				RemoveFlowController();
			}
		}
		else
		{
			RemoveFlowController();
			UpdateObstacle(add: true);
		}
	}

	public void SetTargetHeightEnabledAndSynchronize(bool value)
	{
		SetTargetHeightEnabled(value);
		SynchronizeNeighbors();
	}

	public void SetTargetHeightEnabled(bool value)
	{
		TargetHeightEnabled = value;
	}

	public void SetTargetHeightAndSynchronize(float value)
	{
		SetTargetHeight(value);
		SynchronizeNeighbors();
	}

	public void SetTargetHeight(float value)
	{
		TargetHeight = value;
	}

	public void SetAutomationTargetHeightEnabledAndSynchronize(bool value)
	{
		SetAutomationTargetHeightEnabled(value);
		SynchronizeNeighbors();
	}

	public void SetAutomationTargetHeightEnabled(bool value)
	{
		AutomationTargetHeightEnabled = value;
	}

	public void SetAutomationTargetHeightAndSynchronize(float value)
	{
		SetAutomationTargetHeight(value);
		SynchronizeNeighbors();
	}

	public void SetAutomationTargetHeight(float value)
	{
		AutomationTargetHeight = value;
	}

	public void ToggleSynchronization(bool value)
	{
		IsSynchronized = value;
		_fillValveSynchronizer.SynchronizeWithAllNeighbors(this);
	}

	public void Evaluate()
	{
	}

	private void InitializeOutputCoordinates()
	{
		OutputCoordinates = _blockObject.TransformCoordinates(_fillValveSpec.OutputCoordinates);
	}

	private void TickOutflowLimit()
	{
		float num = _threadSafeWaterMap.WaterHeightOrFloor(OutputCoordinates) - EffectiveTargetHeight;
		bool flag = ((_flowControllerState == FlowControllerState.IncreaseFlow) ? (num < _fillValveSpec.OverflowLimit) : (num < 0f));
		if (flag && _currentFlow < float.MaxValue)
		{
			_currentFlow = ((_currentFlow > MaxFlow) ? float.MaxValue : (_currentFlow + OpeningStep));
			_waterService.SetInflowLimit(_blockObject.Coordinates, _currentFlow);
			_flowControllerState = FlowControllerState.IncreaseFlow;
		}
		else if (!flag && _currentFlow > 0f)
		{
			_currentFlow = GetPreviousClosingFlow(_currentFlow);
			_waterService.SetInflowLimit(_blockObject.Coordinates, _currentFlow);
			_flowControllerState = FlowControllerState.DecreaseFlow;
		}
		else if (!flag && _currentFlow == 0f)
		{
			RemoveFlowController();
			UpdateObstacle(add: true);
		}
	}

	private void OnAutomatableInputReconnected(object sender, EventArgs e)
	{
		if (IsSynchronized)
		{
			SynchronizeNeighbors();
		}
	}

	private void SynchronizeNeighbors()
	{
		_fillValveSynchronizer.SynchronizeAllNeighbors(this);
	}

	private void RemoveFlowController()
	{
		if (_flowControllerState != FlowControllerState.NoControl)
		{
			_waterService.RemoveInflowLimit(_blockObject.Coordinates);
		}
		_flowControllerState = FlowControllerState.NoControl;
		_currentFlow = 0f;
	}

	private void UpdateObstacle(bool add)
	{
		_waterObstacleController.UpdateState(add);
		_obstacleAdded = add;
	}

	private static float GetPreviousClosingFlow(float currentFlow)
	{
		for (int num = ClosingCurve.Length - 1; num >= 0; num--)
		{
			float num2 = ClosingCurve[num];
			if (num2 < currentFlow)
			{
				return num2;
			}
		}
		return 0f;
	}

	static FillValve()
	{
		float[] obj = new float[35]
		{
			0f,
			0.005f,
			0.01f,
			0.015f,
			0.02f,
			0.025f,
			0.03f,
			0.035f,
			0.04f,
			0.045f,
			0.05f,
			0.1f,
			0.2f,
			0.3f,
			0.4f,
			0.5f,
			0.6f,
			0.7f,
			0.8f,
			0.9f,
			1f,
			1.5f,
			2f,
			2.5f,
			3f,
			3.5f,
			4f,
			4.5f,
			5f,
			6f,
			7f,
			8f,
			9f,
			0f,
			float.MaxValue
		};
		obj[33] = MaxFlow;
		ClosingCurve = ImmutableArray.Create(obj);
		ComponentKey = new ComponentKey("FillValve");
		IsSynchronizedKey = new PropertyKey<bool>("IsSynchronized");
		TargetHeightEnabledKey = new PropertyKey<bool>("TargetHeightEnabled");
		TargetHeightKey = new PropertyKey<float>("TargetHeight");
		AutomationTargetHeightEnabledKey = new PropertyKey<bool>("AutomationTargetHeightEnabled");
		AutomationTargetHeightKey = new PropertyKey<float>("AutomationTargetHeight");
		FlowControllerStateKey = new PropertyKey<FlowControllerState>("FlowControllerState");
		ObstacleAddedKey = new PropertyKey<bool>("ObstacleAdded");
		CurrentFlowKey = new PropertyKey<float>("CurrentFlow");
	}
}
